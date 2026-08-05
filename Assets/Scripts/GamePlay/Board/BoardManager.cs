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

    [Header("UI (khong bat buoc, co the de trong)")]
    [SerializeField] private TMP_Text movesLeftText;

    [Header("Animation")]
    [SerializeField] private float swapDuration = 0.25f;

    [Header("Gioi han (theo mockup: Goi y (2), Hoan tac (1))")]
    [SerializeField] private int maxHints = 2;
    [SerializeField] private int maxUndos = 1;
    [SerializeField] private TMP_Text hintsLeftText;  // khong bat buoc
    [SerializeField] private TMP_Text undosLeftText;  // khong bat buoc

    [Header("Popup lua chon Hint (khong bat buoc)")]
    [SerializeField] private GameObject hintOptionsPanel; // panel co 2 nut: "Goi y 1 cap" / "Xem thu tu toan bo"

    private List<EventCard> boardCards = new List<EventCard>();
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
        StopAllCoroutines();

        currentLevel = level;
        gameEnded = false;
        isAnimating = false;
        memoryCardsHidden = false;

        history.Clear();
        insertionHistory.Clear();

        firstSelected = null;
        pivotCard = null;
        primedCard = null;
        insertionCard = null;

        boardCards.Clear();

        hintsUsed = 0;
        undosUsed = 0;

        for (int i = cardRow.childCount - 1; i >= 0; i--)
            Destroy(cardRow.GetChild(i).gameObject);

        List<EventData> shuffled = new List<EventData>(level.events);
        Shuffle(shuffled);

        int safety = 0;
        while (IsSorted(shuffled) && safety < 10)
        {
            Shuffle(shuffled);
            safety++;
        }

        for (int i = 0; i < shuffled.Count; i++)
        {
            GameObject go = Instantiate(cardPrefab, cardRow);
            EventCard card = go.GetComponent<EventCard>();
            card.Initialize(shuffled[i], i);
            boardCards.Add(card);
        }
            // Chỉ úp thẻ ở những level bật Memory Mode
        if (currentLevel.useMemoryMode)
        {
            StartCoroutine(PreviewThenHideCards());
        }
        else
        {
            // Các level bình thường luôn để thẻ ngửa
            foreach (EventCard card in boardCards)
            {
                card.FlipUp();
            }
        }
        movesLeft = level.maxMoves;
        timeLeft = level.timeLimitSeconds;
        /*
        * Level thường: đồng hồ chạy ngay.
        * Level Memory: đợi xem trước và úp thẻ xong mới chạy.
        */
        timerRunning =
            level.limitType == LevelLimitType.Time &&
            !level.useMemoryMode;

        UpdateMovesUI();
        UpdateLimitsUI();
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

        HorizontalLayoutGroup layout = cardRow.GetComponent<HorizontalLayoutGroup>();
        if (layout != null) layout.enabled = false;

        Vector3 posA = ra.position;
        Vector3 posB = rb.position;

        float t = 0f;
        while (t < swapDuration)
        {
            t += Time.deltaTime;
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
        if (movesLeftText == null || currentLevel == null)
            return;

        if (currentLevel.limitType == LevelLimitType.Time)
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
        for (int i = pivotIndex + 1; i < boardCards.Count; i++)
        {
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
}