using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Ghi de toan bo file: Assets/Scripts/GamePlay/Board/BoardManager.cs

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    [Header("References")]
    [SerializeField] private Transform cardRow;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private LevelData currentLevel;
    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private TMP_Text chapterTitleText;
    [SerializeField] private TMP_Text levelTitleText;

    [Header("Vi tri tuy chinh cho tung the (thay cho HorizontalLayoutGroup)")]
    [Tooltip("QUAN TRONG: object nay PHAI la mot GameObject rieng, KHONG nam trong cardRow. " +
             "Vi moi lan Restart, code se xoa toan bo con cua cardRow (de xoa the cu), " +
             "neu Slot nam trong cardRow no se bi xoa theo va mat vi tri tuy chinh.")]
    [SerializeField] private Transform slotsContainer;

    [Tooltip("Keo cac Empty GameObject (RectTransform) danh dau vi tri vao day, theo dung thu tu tu trai sang phai. " +
             "So luong slot phai >= so the toi da trong level. Neu de trong, code se fallback ve HorizontalLayoutGroup (neu co).")]
    [SerializeField] private RectTransform[] cardSlots;

    [Header("UI (khong bat buoc, co the de trong)")]
    [SerializeField] private TMP_Text movesLeftText;
    [SerializeField] private TMP_Text limitModeText;

    [Header("Background")]
    [Tooltip("Keo Image nen (GameObject 'Background' trong Canvas) vao day. " +
        "Neu LevelData.backgroundImage co gan anh, no se duoc dung thay cho " +
        "sprite mac dinh dang gan san trong Inspector cua Image nay.")]
    [SerializeField] private Image background;
    private Sprite defaultBackgroundSprite;

    [Header("Animation")]
    [SerializeField] private float swapDuration = 0.25f;

    [Header("Gioi han (theo mockup: Goi y (2), Hoan tac (1))")]
    [SerializeField] private int maxHints = 2;
    [SerializeField] private int maxUndos = 1;
    [SerializeField] private TMP_Text hintsLeftText;  // khong bat buoc
    [SerializeField] private TMP_Text undosLeftText;  // khong bat buoc

    [Header("Popup lua chon Hint (khong bat buoc)")]
    [SerializeField] private GameObject hintOptionsPanel; // panel co 2 nut: "Goi y 1 cap" / "Xem thu tu toan bo"

    // The dang bi khoa co dinh hien tai (thuong la boardCards[0] luc setup).
    // KHONG con la cong tac chung nua: viec co khoa hay khong do LevelData.lockFirstCard
    // cua tung man quyet dinh (xem SetupLevelInternal), nen man nao bat thi khoa, man khac thi khong.
    private EventCard lockedCard;

    private List<EventCard> boardCards = new List<EventCard>();
    // Lưu thứ tự thẻ lúc màn chơi bắt đầu.
    // Reset sẽ khôi phục đúng thứ tự này, không random lại.
    private List<EventData> initialCardOrder = new List<EventData>();
    private EventCard firstSelected;
    private EventCard pivotCard;    //thẻ chốt
    private EventCard primedCard;   //thẻ được đánh dấu đã mở trong MemoryMode
    private EventCard insertionCard; //thẻ được rút trong insertionSort
    private int movesLeft;
    private float timeLeft;
    private bool timerRunning;

    private int hintsUsed;
    private int undosUsed;

    public int MovesLeft => movesLeft;
    public float TimeLeft => timeLeft;
    public int HintsUsed => hintsUsed;
    public int UndosUsed => undosUsed;
    public int MaxMoves => currentLevel.maxMoves;

    private bool gameEnded;
    private bool isAnimating;
    private bool memoryCardsHidden;

    private Stack<(int indexA, int indexB)> history = new Stack<(int, int)>();
    private Stack<(int oldIndex, int insertedIndex)> insertionHistory = new Stack<(int, int)>();

    private void Awake()
    {
        Instance = this;

        // Dam bao popup Hint luon an luc bat dau, khong phu thuoc trang thai Active
        // duoc set san trong Scene/Inspector.
        if (hintOptionsPanel != null)
            hintOptionsPanel.SetActive(false);

        // Nho lai sprite nen mac dinh (dang gan san trong Inspector) de dung
        // lam fallback cho nhung level khong co backgroundImage rieng.
        if (background != null)
            defaultBackgroundSprite = background.sprite;
    }

    private void Start()
    {
        if (cardRow == null)
        {
            Debug.LogError("BoardManager: chua gan Card Row trong Inspector!", this);
            return;
        }

        if (cardPrefab == null)
        {
            Debug.LogError("BoardManager: chua gan Card Prefab trong Inspector!", this);
            return;
        }

        if (LevelSession.SelectedLevel != null)
            currentLevel = LevelSession.SelectedLevel;

        if (currentLevel != null)
            SetupLevel(currentLevel);
        else
            Debug.LogError("BoardManager: chua co Current Level nao duoc gan!");
    }
    private void Update()
    {
        if (!timerRunning || gameEnded)
            return;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            timerRunning = false;
            gameEnded = true;

            UpdateMovesUI();

            AudioManager.Instance?.PlayLose();
            GameManager.Instance?.LoseGame();

            return;
        }

        UpdateMovesUI();
    }

    // ---------------- SETUP ----------------

    public void SetupLevel(LevelData level)
    {
        // Khi bắt đầu màn lần đầu: tạo thứ tự ngẫu nhiên mới.
        SetupLevelInternal(level, true);
    }

    private void SetupLevelInternal(LevelData level, bool createNewOrder)
    {
            StopAllCoroutines();

        // Tránh trạng thái pause còn sót lại.
        Time.timeScale = 1f;

        // Nếu Reset xảy ra giữa animation thì Layout có thể đang bị tắt.
        // HorizontalLayoutGroup activeLayout =
        //     cardRow != null
        //     ? cardRow.GetComponent<HorizontalLayoutGroup>()
        //     : null;

        // if (activeLayout != null)
        //     activeLayout.enabled = true;

        currentLevel = level;
        UpdateChapterTitle();

        gameEnded = false;
        isAnimating = false;
        memoryCardsHidden = false;

        ApplyBackground(level);
        ApplyMusic(level);

        history.Clear();
        insertionHistory.Clear();

        firstSelected = null;
        pivotCard = null;
        primedCard = null;
        insertionCard = null;
        lockedCard = null;

        boardCards.Clear();

        hintsUsed = 0;
        undosUsed = 0;

        CloseHintOptions();

        // Xóa các thẻ hiện tại.
        for (int i = cardRow.childCount - 1; i >= 0; i--)
        {
            cardRow.GetChild(i).gameObject.SetActive(false);
            Destroy(cardRow.GetChild(i).gameObject);
        }

        List<EventData> cardOrder;

        if (createNewOrder || initialCardOrder.Count == 0)
        {
            // Khong random nua: luon dung dung thu tu the da khai bao san
            // trong danh sach "events" cua LevelData (co dinh moi lan choi).
            cardOrder = new List<EventData>(level.events);

            // Ghi nhớ thứ tự ban đầu.
            initialCardOrder = new List<EventData>(cardOrder);
        }
        else
        {
            // Reset: sử dụng lại đúng thứ tự ban đầu.
            cardOrder = new List<EventData>(initialCardOrder);
        }

        for (int i = 0; i < cardOrder.Count; i++)
        {
            GameObject go = Instantiate(cardPrefab, cardRow);
            EventCard card = go.GetComponent<EventCard>();

            card.Initialize(cardOrder[i], i);
            boardCards.Add(card);
        }

        // Khoa co dinh the dau tien (neu LevelData cua man nay bat tuy chon nay).
        // Moi man co the bat/tat rieng: man nao lockFirstCard = true thi khoa, man khac thi khong.
        if (level.lockFirstCard && boardCards.Count > 0)
        {
            lockedCard = boardCards[0];
            lockedCard.SetLocked(true);
        }

        PositionCards();

        // // Bắt Unity sắp xếp lại các thẻ vừa được tạo.
        // Canvas.ForceUpdateCanvases();

        // RectTransform rowRect = cardRow as RectTransform;

        // if (rowRect != null)
        //     LayoutRebuilder.ForceRebuildLayoutImmediate(rowRect);

        movesLeft = level.maxMoves;
        timeLeft = level.timeLimitSeconds;

        if (currentLevel.useMemoryMode)
        {
            StartCoroutine(PreviewThenHideCards());
        }
        else
        {
            foreach (EventCard card in boardCards)
            {
                card.Deselect();
                card.FlipUp();
            }
        }

        timerRunning =
            level.limitType == LevelLimitType.Time &&
            !level.useMemoryMode;

        UpdateMovesUI();
        UpdateLimitsUI();

        // Chỉ hiện hướng dẫn khi bắt đầu màn lần đầu.
        // Reset sẽ không mở lại hướng dẫn.
        if (createNewOrder && tutorialManager != null)
        {
            tutorialManager.StartTutorial(currentLevel);
        }
    }
    public void ResetLevel()
    {
        if (currentLevel == null || cardRow == null)
            return;

        // Nếu Tutorial đang làm game dừng thì đóng đúng cách.
        if (tutorialManager != null)
            tutorialManager.CloseTutorial();

        Time.timeScale = 1f;

        // Dừng animation cũ và giải phóng trạng thái đang khóa thao tác.
        StopAllCoroutines();

        gameEnded = false;
        isAnimating = false;
        timerRunning = false;

        // Animation đổi thẻ có thể đã tắt Layout.
        // Phải bật lại trước khi tạo thẻ mới.
        // HorizontalLayoutGroup layout =
        //     cardRow.GetComponent<HorizontalLayoutGroup>();

        // if (layout != null)
        //     layout.enabled = true;

        SetupLevelInternal(currentLevel, false);

        // Canvas.ForceUpdateCanvases();

        // RectTransform rowRect = cardRow as RectTransform;

        // if (rowRect != null)
        //     LayoutRebuilder.ForceRebuildLayoutImmediate(rowRect);
}
    // Hàm đặt ngửa thẻ sau x(s) thì úp xuống
    private IEnumerator PreviewThenHideCards()
    {
        isAnimating = true;
        memoryCardsHidden = false;

        // Ban đầu cho tất cả thẻ ngửa
        foreach (EventCard card in boardCards)
        {
            card.Deselect();
            card.FlipUp();
        }

        // Chờ 3–5 giây theo thiết lập của LevelData
        yield return new WaitForSeconds(currentLevel.previewTime);

        // Hết thời gian: úp tất cả thẻ xuống
        foreach (EventCard card in boardCards)
        {
            card.Deselect();
            card.FlipDown();
        }

        memoryCardsHidden = true;
        isAnimating = false;
        if (currentLevel.limitType == LevelLimitType.Time)
        {
            timerRunning = true;
        }
    }
    private void Shuffle(List<EventData> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private bool IsSorted(List<EventData> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
            if (list[i].year > list[i + 1].year)
                return false;
        return true;
    }

    // ---------------- CHON / DOI CHO ----------------

    public void OnCardClicked(EventCard card)
    {
        if (gameEnded || isAnimating)
            return;

        // The bi khoa co dinh (vd: the dau tien): khong phan hoi click o bat ky che do nao.
        if (card.IsLocked)
        {
            AudioManager.Instance?.PlayClick();
            return;
        }

        // Level bình thường: chọn thẻ giống như trước
        if (!currentLevel.useMemoryMode || !memoryCardsHidden)
        {
            HandleNormalCardClick(card);
            return;
        }

        // Level thẻ ẩn: phải bấm hai lần
        HandleMemoryCardClick(card);
    }

    // Xử lý khi chơi level thẻ ẩn
    private void HandleMemoryCardClick(EventCard card)
    {
        // Lần bấm thứ nhất: đánh dấu thẻ
        if (primedCard == null)
        {
            primedCard = card;
            primedCard.SetMemoryMarked(true);

            AudioManager.Instance?.PlayClick();
            return;
        }

        // Bấm sang một thẻ khác: chuyển dấu sang thẻ mới
        if (primedCard != card)
        {
            primedCard.SetMemoryMarked(false);

            primedCard = card;
            primedCard.SetMemoryMarked(true);

            AudioManager.Instance?.PlayClick();
            return;
        }

        // Bấm lần thứ hai vào đúng thẻ đang được đánh dấu
        EventCard confirmedCard = primedCard;
        primedCard = null;

        StartCoroutine(RevealAndConfirmCard(confirmedCard));
    }

    // Lật thẻ lên, chờ 1.2 giây, sau đó úp và cố định
    private IEnumerator RevealAndConfirmCard(EventCard card)
    {
        isAnimating = true;

        // Bỏ màu đánh dấu tạm thời
        card.SetMemoryMarked(false);

        // Lật thẻ lên
        card.FlipUp();

        AudioManager.Instance?.PlayClick();

        // Chờ theo Reveal Time trong LevelData
        yield return new WaitForSeconds(currentLevel.revealTime);

        // Úp thẻ lại
        card.FlipDown();

        isAnimating = false;

        // Cố định thẻ vào lượt đổi chỗ
        HandleNormalCardClick(card);
    }

    // Logic chọn và đổi thẻ bình thường
    private void HandleNormalCardClick(EventCard card)
    {
        // Insertion Sort có luật chèn thẻ riêng
        if (currentLevel.sortMode == SortMode.InsertionSort)
        {
            HandleInsertionSort(card);
            return;
        }
        // Selection Sort có luật riêng
        if (currentLevel.sortMode == SortMode.SelectionSort)
        {
            HandleSelectionSort(card);
            return;
        }

        // Chưa chọn thẻ thứ nhất
        if (firstSelected == null)
        {
            firstSelected = card;
            card.Select();

            AudioManager.Instance?.PlayClick();
            return;
        }

        // Bấm lại thẻ đã cố định: bỏ chọn
        if (firstSelected == card)
        {
            card.Deselect();
            firstSelected = null;
            return;
        }

        EventCard a = firstSelected;
        EventCard b = card;

        firstSelected = null;

        // Kiểm tra luật Bubble Sort
        if (currentLevel.sortMode == SortMode.BubbleSort)
        {
            int indexA = boardCards.IndexOf(a);
            int indexB = boardCards.IndexOf(b);

            // Chỉ cho phép đổi hai thẻ liền kề
            if (Mathf.Abs(indexA - indexB) != 1)
            {
                a.Deselect();
                b.Deselect();

                AudioManager.Instance?.PlayClick();
                return;
            }
        }

        a.Deselect();
        b.Deselect();

        StartCoroutine(AnimateSwap(a, b));
    }
    private void HandleInsertionSort(EventCard card)
    {
        // Chưa chọn thẻ cần rút
        if (insertionCard == null)
        {
            insertionCard = card;
            insertionCard.Select();

            AudioManager.Instance?.PlayClick();
            return;
        }

        // Bấm lại chính thẻ đã chọn: hủy chọn, không mất lượt
        if (insertionCard == card)
        {
            insertionCard.Deselect();
            insertionCard = null;

            AudioManager.Instance?.PlayClick();
            return;
        }

        EventCard movingCard = insertionCard;
        EventCard targetCard = card;

        int sourceIndex = boardCards.IndexOf(movingCard);
        int targetIndex = boardCards.IndexOf(targetCard);

        // Xóa đánh dấu và kết thúc lượt chọn
        movingCard.Deselect();
        insertionCard = null;

        /*
        * Rút thẻ khỏi vị trí cũ.
        * Sau khi rút, nếu nó vốn nằm trước thẻ đích
        * thì chỉ số của thẻ đích sẽ giảm đi 1.
        */
        boardCards.RemoveAt(sourceIndex);

        if (sourceIndex < targetIndex)
        {
            targetIndex--;
        }

        // Chèn thẻ được rút ngay trước thẻ đích
        boardCards.Insert(targetIndex, movingCard);
        // Lưu vị trí cũ và vị trí sau khi chèn để Undo
        insertionHistory.Push((sourceIndex, targetIndex));
        // Cập nhật thứ tự hiển thị và CurrentIndex
        RefreshOrder();

        // Chèn đúng hoặc sai đều mất 1 lượt
        ConsumeMove();

        AudioManager.Instance?.PlaySwap();

        // Kiểm tra thắng hoặc hết lượt
        CheckGameEnd();
    }
    private IEnumerator AnimateSwap(EventCard a, EventCard b)
    {
        isAnimating = true;
        AudioManager.Instance?.PlaySwap();

        RectTransform ra = a.GetComponent<RectTransform>();
        RectTransform rb = b.GetComponent<RectTransform>();

        // Neu cardRow van con HorizontalLayoutGroup (chua go trong Inspector) thi tam tat
        // trong luc dang tween, tranh no tu dong keo the ve vi tri "giao deu".
        HorizontalLayoutGroup layout = cardRow.GetComponent<HorizontalLayoutGroup>();
        if (layout != null) layout.enabled = false;

        Vector3 posA = ra.position;
        Vector3 posB = rb.position;

        float t = 0f;
        while (t < swapDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / swapDuration);
            k = k * k * (3f - 2f * k); // ease in-out

            ra.position = Vector3.Lerp(posA, posB, k);
            rb.position = Vector3.Lerp(posB, posA, k);
            yield return null;
        }

        int ia = boardCards.IndexOf(a);
        int ib = boardCards.IndexOf(b);
        (boardCards[ia], boardCards[ib]) = (boardCards[ib], boardCards[ia]);
        RefreshOrder();

        if (layout != null) layout.enabled = true;

        history.Push((ia, ib));

        ConsumeMove();

        isAnimating = false;

        CheckGameEnd();
    }

    private void RefreshOrder()
    {
        for (int i = 0; i < boardCards.Count; i++)
        {
            boardCards[i].transform.SetSiblingIndex(i);
            boardCards[i].SetIndex(i);
        }

        // Dat lai vi tri theo slot tu chinh (khong con giao cho HorizontalLayoutGroup tu giai đeu).
        PositionCards();
    }

    // Gan vi tri tung the theo dung slot (RectTransform) da duoc keo tay trong Inspector,
    // theo thu tu hien tai trong boardCards. Neu khong co du slot, giu nguyen vi tri hien tai.
    private void PositionCards()
    {
        if (boardCards == null || boardCards.Count == 0)
            return;

        // Uu tien dung slot thu cong (cardSlots) neu da gan du trong Inspector.
        // Vi tri tung the se lay dung theo RectTransform cua slot tuong ung
        // (theo thu tu hien tai trong boardCards), khong tu dong can giua nua.
        if (cardSlots != null && cardSlots.Length >= boardCards.Count)
        {
            for (int i = 0; i < boardCards.Count; i++)
            {
                if (boardCards[i] == null || cardSlots[i] == null)
                    continue;

                RectTransform rt = boardCards[i].GetComponent<RectTransform>();
                if (rt == null)
                    continue;

                // Neu the va slot co chung cha (parent), dung lai vi tri/kich thuoc cua slot.
                rt.sizeDelta = cardSlots[i].sizeDelta;
                rt.anchoredPosition = cardSlots[i].anchoredPosition;
            }

            return;
        }

        // ---- Fallback: khong du slot thu cong -> tu dong can giua nhu cu ----
        if (cardRow == null)
            return;

        RectTransform rowRect = cardRow.GetComponent<RectTransform>();

        float rowWidth = rowRect.rect.width;

        // Khoảng cách giữa các card
        float spacing = 15f;

        // Chiều rộng card
        float cardWidth = 170f;

        // Nếu nhiều card thì tự giảm kích thước
        int count = boardCards.Count;

        if (count > 1)
        {
            float maxWidth = (rowWidth - spacing * (count - 1)) / count;

            if (maxWidth < cardWidth)
                cardWidth = maxWidth;
        }

        for (int i = 0; i < boardCards.Count; i++)
        {
            if (boardCards[i] == null)
                continue;

            RectTransform rt = boardCards[i].GetComponent<RectTransform>();

            if (rt == null)
                continue;

            // Kích thước card
            rt.sizeDelta = new Vector2(cardWidth, rt.sizeDelta.y);

            // Tính vị trí X
            float totalWidth =
                count * cardWidth +
                (count - 1) * spacing;

            float startX = -totalWidth / 2f + cardWidth / 2f;

            float x = startX + i * (cardWidth + spacing);

            rt.anchoredPosition = new Vector2(
                x,
                rt.anchoredPosition.y
            );
        }
    }
    // === TIEN ICH: click phai vao component BoardManager trong Inspector -> chon muc nay ===
    // Tu dong tao san N slot (Empty GameObject co RectTransform) duoi cardRow, xep deu
    // giong nhu HorizontalLayoutGroup dang lam, de ban co diem bat dau roi tu keo chinh lai.
    // So luong slot = so the toi da trong cac LevelData da gan (hoac tu nhap slotCountToGenerate).
    [Header("Tao slot tu dong (chi dung trong Editor)")]
    [SerializeField] private int slotCountToGenerate = 10;
    [SerializeField] private float slotSpacingX = 190f;
    [SerializeField] private float slotWidth = 170f;
    [SerializeField] private float slotHeight = 240f;

    [ContextMenu("Auto Tao Slot Vi Tri (deu nhau, roi tu chinh tay)")]
    private void AutoGenerateSlots()
    {
        if (cardRow == null)
        {
            Debug.LogError("Chua gan Card Row!", this);
            return;
        }

        if (slotsContainer == null)
        {
            Debug.LogError("Chua gan Slots Container! Tao 1 GameObject rieng (VD ten 'CardSlots'), " +
                            "de NGANG HANG voi cardRow (KHONG nam trong cardRow), roi gan vao field 'Slots Container'.", this);
            return;
        }

        // Xoa cac slot cu (neu co) truoc khi tao lai, tranh trung lap.
        var oldSlots = new List<Transform>();
        foreach (Transform child in slotsContainer)
        {
            if (child.name.StartsWith("Slot_"))
                oldSlots.Add(child);
        }
        foreach (var t in oldSlots)
        {
#if UNITY_EDITOR
            DestroyImmediate(t.gameObject);
#else
            Destroy(t.gameObject);
#endif
        }

        cardSlots = new RectTransform[slotCountToGenerate];

        // Tinh diem bat dau de ca day the nam giua (can giua theo truc X).
        float totalWidth = (slotCountToGenerate - 1) * slotSpacingX;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < slotCountToGenerate; i++)
        {
            GameObject slotGO = new GameObject($"Slot_{i}", typeof(RectTransform));
            RectTransform rt = slotGO.GetComponent<RectTransform>();
            rt.SetParent(slotsContainer, false);
            rt.sizeDelta = new Vector2(slotWidth, slotHeight);
            rt.anchoredPosition = new Vector2(startX + i * slotSpacingX, 0f);

            cardSlots[i] = rt;
        }

        Debug.Log($"Da tao {slotCountToGenerate} slot trong '{slotsContainer.name}'. Mo object do trong Hierarchy, " +
                   "keo tung Slot_i toi vi tri ban muon (Scene view), roi luu Scene.", this);
    }

    // ---------------- KHOA THE ----------------

    // Goi ham nay tu Inspector/UI Button neu muon bat khoa the dau tien ngay trong luc dang choi
    // (khong can Reset). The dang o vi tri index 0 tai thoi diem goi se bi khoa.
    public void LockFirstCard()
    {
        if (boardCards.Count == 0)
            return;

        lockedCard = boardCards[0];
        lockedCard.SetLocked(true);
    }

    // Mo khoa the dang bi khoa (neu co).
    public void UnlockFirstCard()
    {
        if (lockedCard == null)
            return;

        lockedCard.SetLocked(false);
        lockedCard = null;
    }

    // ---------------- HOAN TAC ----------------

    public void Undo()
    {
        if (gameEnded || isAnimating)
            return;

        if (undosUsed >= maxUndos)
            return;

        bool isInsertionMode =
            currentLevel.sortMode == SortMode.InsertionSort;

        // Không có thao tác để Undo
        if (isInsertionMode)
        {
            if (insertionHistory.Count == 0)
                return;
        }
        else
        {
            if (history.Count == 0)
                return;
        }

        // Hủy mọi thao tác chọn thẻ đang làm dở
        if (firstSelected != null)
        {
            firstSelected.Deselect();
            firstSelected = null;
        }

        if (pivotCard != null)
        {
            pivotCard.Deselect();
            pivotCard = null;
        }

        if (insertionCard != null)
        {
            insertionCard.Deselect();
            insertionCard = null;
        }

        if (primedCard != null)
        {
            primedCard.SetMemoryMarked(false);
            primedCard = null;
        }

        if (isInsertionMode)
        {
            var action = insertionHistory.Pop();

            int oldIndex = action.oldIndex;
            int insertedIndex = action.insertedIndex;

            EventCard movedCard = boardCards[insertedIndex];

            // Rút khỏi vị trí sau khi chèn
            boardCards.RemoveAt(insertedIndex);

            // Trả lại vị trí ban đầu
            boardCards.Insert(oldIndex, movedCard);

            RefreshOrder();
        }
        else
        {
            var action = history.Pop();

            int indexA = action.indexA;
            int indexB = action.indexB;

            (boardCards[indexA], boardCards[indexB]) =
                (boardCards[indexB], boardCards[indexA]);

            RefreshOrder();
        }

        // Hoàn lại lượt
        // Chỉ hoàn lại lượt đối với level giới hạn Moves
        if (currentLevel.limitType == LevelLimitType.Moves)
        {
            movesLeft++;
        }

        // Tính một lần sử dụng Undo
        undosUsed++;

        UpdateMovesUI();
        UpdateLimitsUI();
    }
    // ---------------- GOI Y ----------------

    // Nut Hint ngoai man hinh nen goi ham nay (thay vi goi thang ShowHint).
    // Mo popup cho nguoi choi chon: "Goi y 1 cap" hoac "Xem thu tu toan bo".
    public void OpenHintOptions()
    {
        if (gameEnded || isAnimating) return;
        if (hintsUsed >= maxHints) return; // het luot hint thi khong mo popup

        if (hintOptionsPanel != null)
            hintOptionsPanel.SetActive(true);
    }

    public void CloseHintOptions()
    {
        if (hintOptionsPanel != null)
            hintOptionsPanel.SetActive(false);
    }

    // Hint loai 2: goi y dung 1 cap the sai gan nhat (nhap nhay)
    public void ShowHint()
    {
        CloseHintOptions();

        if (gameEnded || isAnimating) return;
        if (hintsUsed >= maxHints) return;

        for (int i = 0; i < boardCards.Count - 1; i++)
        {
            if (boardCards[i].Data.year > boardCards[i + 1].Data.year)
            {
                hintsUsed++;
                UpdateLimitsUI();
                StartCoroutine(FlashHint(boardCards[i], boardCards[i + 1]));
                return;
            }
        }
    }

    private IEnumerator FlashHint(EventCard a, EventCard b)
    {
        for (int i = 0; i < 3; i++)
        {
            a.Select(); b.Select();
            yield return new WaitForSeconds(0.25f);
            a.Deselect(); b.Deselect();
            yield return new WaitForSeconds(0.25f);
        }
    }

    // Hint loai 1: xem thu tu dung toan bo (ton het so hint con lai).
    // Khong tu sap xep lai the: chi chop lan luot tung the theo dung thu tu nam,
    // nguoi choi van phai tu keo/doi cho de hoan thanh.
    public void ShowFullOrderHint()
    {
        CloseHintOptions();

        if (gameEnded || isAnimating) return;
        if (hintsUsed >= maxHints) return; // het luot hint, khong the xem toan bo

        hintsUsed = maxHints; // tieu toan bo so hint con lai
        UpdateLimitsUI();

        StartCoroutine(FlashFullOrder());
    }

    private IEnumerator FlashFullOrder()
    {
        isAnimating = true;

        List<EventCard> sortedByYear = new List<EventCard>(boardCards);
        sortedByYear.Sort((x, y) => x.Data.year.CompareTo(y.Data.year));

        foreach (EventCard card in sortedByYear)
        {
            card.Select();
            yield return new WaitForSeconds(0.5f);
            card.Deselect();
            yield return new WaitForSeconds(0.15f);
        }

        isAnimating = false;
    }

    // ---------------- THANG / THUA ----------------

    private void CheckGameEnd()
    {
        bool correct = true;
        for (int i = 0; i < boardCards.Count - 1; i++)
        {
            if (boardCards[i].Data.year > boardCards[i + 1].Data.year)
            {
                correct = false;
                break;
            }
        }

        if (correct)
        {
            gameEnded = true;
            AudioManager.Instance?.PlayWin();
            GameManager.Instance?.WinGame(movesLeft, currentLevel.maxMoves, currentLevel.levelNumber);
            return;
        }

        if (currentLevel.limitType == LevelLimitType.Moves &&
            movesLeft <= 0)
        {
            gameEnded = true;
            AudioManager.Instance?.PlayLose();
            GameManager.Instance?.LoseGame();
        }
    }

    // ---------------- BACKGROUND ----------------

    // Ap dung anh nen rieng cua LevelData (neu co). Neu LevelData khong gan
    // backgroundImage, tu dong quay lai sprite mac dinh cua scene GamePlay
    // (khong lam vo cac man cu chua khai bao anh nen rieng).
    private void ApplyBackground(LevelData level)
    {
        if (background == null || level == null)
            return;

        background.sprite = level.backgroundImage != null
            ? level.backgroundImage
            : defaultBackgroundSprite;
    }

    // ---------------- MUSIC ----------------

    // Phat nhac nen rieng cua LevelData (neu co gan). Neu khong gan,
    // AudioManager se tu fallback ve nhac mac dinh (neu co).
    private void ApplyMusic(LevelData level)
    {
        if (level == null) return;

        AudioManager.Instance?.PlayMusic(level.backgroundMusic);
    }

    // ---------------- UI ----------------
    private void ConsumeMove()
    {
        if (currentLevel.limitType == LevelLimitType.Moves)
        {
            movesLeft--;
        }

        UpdateMovesUI();
    }
    private void UpdateMovesUI()
    {
        if (currentLevel == null)
            return;

        bool isTimeLevel =
            currentLevel.limitType == LevelLimitType.Time;

        if (limitModeText != null)
        {
            limitModeText.text = isTimeLevel
                ? "THỜI GIAN"
                : "LƯỢT CÒN";
        }

        if (movesLeftText == null)
            return;

        if (isTimeLevel)
        {
            int totalSeconds = Mathf.CeilToInt(timeLeft);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            movesLeftText.text =
                minutes.ToString("00") + ":" +
                seconds.ToString("00");
        }
        else
        {
            movesLeftText.text = movesLeft.ToString();
        }
    }
    private void UpdateLimitsUI()
    {
        if (hintsLeftText != null)
            hintsLeftText.text = (maxHints - hintsUsed).ToString();

        if (undosLeftText != null)
            undosLeftText.text = (maxUndos - undosUsed).ToString();
    }
    // ----- Luật chơi SelectionSort -----
    private void HandleSelectionSort(EventCard card)
    {
        // Chưa chọn chốt
        if (pivotCard == null)
        {
            pivotCard = card;
            pivotCard.Select();

            AudioManager.Instance?.PlayClick();
            return;
        }

        // Bấm lại thẻ chốt: bỏ chọn, không mất lượt
        if (pivotCard == card)
        {
            pivotCard.Deselect();
            pivotCard = null;

            AudioManager.Instance?.PlayClick();
            return;
        }

        int pivotIndex = boardCards.IndexOf(pivotCard);
        int minIndex = pivotIndex;

        // Tìm thẻ có năm nhỏ nhất từ vị trí chốt đến cuối
        // (bo qua the dang bi khoa co dinh, vi nguoi choi khong the chon no).
        for (int i = pivotIndex + 1; i < boardCards.Count; i++)
        {
            if (boardCards[i].IsLocked)
                continue;

            if (boardCards[i].Data.year <
                boardCards[minIndex].Data.year)
            {
                minIndex = i;
            }
        }

        bool isWrongSelection =
            minIndex == pivotIndex ||
            boardCards[minIndex] != card;

        if (isWrongSelection)
        {
            Debug.Log("Sai! Bạn đã mất 1 lượt.");

            AudioManager.Instance?.PlayClick();

            pivotCard.Deselect();
            pivotCard = null;

            ConsumeMove();

            CheckGameEnd();
            return;
        }

        // Người chơi chọn đúng thẻ nhỏ nhất
        EventCard a = pivotCard;
        EventCard b = card;

        pivotCard.Deselect();
        pivotCard = null;

        // AnimateSwap sẽ tự trừ 1 lượt và gọi CheckGameEnd()
        StartCoroutine(AnimateSwap(a, b));
    }   
    private void UpdateChapterTitle()
    {
        if (currentLevel == null)
            return;

        if (chapterTitleText != null)
        {
            string chapterName =
                string.IsNullOrWhiteSpace(currentLevel.chapterName)
                    ? ""
                    : currentLevel.chapterName.ToUpperInvariant();

            chapterTitleText.text =
                $"CHƯƠNG {currentLevel.chapterNumber} • {chapterName}";
        }

        if (levelTitleText != null)
        {
            levelTitleText.text =
                $"MÀN {currentLevel.levelNumber}";
        }
    }
}