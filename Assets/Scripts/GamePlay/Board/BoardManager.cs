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
    private int movesLeft;
    private int hintsUsed;
    private int undosUsed;

    public int MovesLeft => movesLeft;
    public int HintsUsed => hintsUsed;
    public int UndosUsed => undosUsed;
    public int MaxMoves => currentLevel.maxMoves;

    private bool gameEnded;
    private bool isAnimating;

    private Stack<(int indexA, int indexB)> history = new Stack<(int, int)>();

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

    // ---------------- SETUP ----------------

    public void SetupLevel(LevelData level)
    {
        currentLevel = level;
        gameEnded = false;
        isAnimating = false;
        history.Clear();
        firstSelected = null;
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

        movesLeft = level.maxMoves;
        UpdateMovesUI();
        UpdateLimitsUI();
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
        if (gameEnded || isAnimating) return;

        if (firstSelected == null)
        {
            firstSelected = card;
            card.Select();
            AudioManager.Instance?.PlayClick();
            return;
        }

        if (firstSelected == card)
        {
            card.Deselect();
            firstSelected = null;
            return;
        }

        EventCard a = firstSelected;
        EventCard b = card;
        firstSelected = null;

        a.Deselect();
        b.Deselect();

        StartCoroutine(AnimateSwap(a, b));
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

        movesLeft--;
        UpdateMovesUI();

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
        if (gameEnded || history.Count == 0 || isAnimating) return;
        if (undosUsed >= maxUndos) return;

        var (ia, ib) = history.Pop();
        (boardCards[ia], boardCards[ib]) = (boardCards[ib], boardCards[ia]);
        RefreshOrder();

        movesLeft++;
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

        if (movesLeft <= 0)
        {
            gameEnded = true;
            AudioManager.Instance?.PlayLose();
            GameManager.Instance?.LoseGame();
        }
    }

    // ---------------- UI ----------------

    private void UpdateMovesUI()
    {
        if (movesLeftText != null)
            movesLeftText.text = movesLeft.ToString();
    }

    private void UpdateLimitsUI()
    {
        if (hintsLeftText != null)
            hintsLeftText.text = (maxHints - hintsUsed).ToString();

        if (undosLeftText != null)
            undosLeftText.text = (maxUndos - undosUsed).ToString();
    }
}