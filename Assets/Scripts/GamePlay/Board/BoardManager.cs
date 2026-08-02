using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Dat file nay vao: Assets/Scripts/GamePlay/Board/BoardManager.cs
//
// QUY UOC QUAN TRONG: field "year" trong EventData phai nhap la SO AM ung voi
// "so nam truoc hien tai", cang co xua cang am nhieu.
// Vi du: LUCA (4.2 ty nam truoc) -> year = -4200000000
//        Homo sapiens (300 nghin nam truoc) -> year = -300000
// Nhu vay sap xep TANG DAN theo year se ra dung thu tu tien hoa (co -> moi).

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    [Header("References")]
    [SerializeField] private Transform cardRow;      // Keo object CardRow vao day
    [SerializeField] private GameObject cardPrefab;  // Keo Card.prefab vao day
    [SerializeField] private LevelData currentLevel; // Keo LevelData cua man nay vao day

    [Header("UI (khong bat buoc, co the de trong)")]
    [SerializeField] private TMP_Text movesLeftText;

    private List<EventCard> boardCards = new List<EventCard>();
    private EventCard firstSelected;
    private int movesLeft;
    private bool gameEnded;

    private Stack<(int indexA, int indexB)> history = new Stack<(int, int)>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (currentLevel != null)
            SetupLevel(currentLevel);
    }

    // ---------------- SETUP ----------------

    public void SetupLevel(LevelData level)
    {
        currentLevel = level;
        gameEnded = false;
        history.Clear();
        firstSelected = null;
        boardCards.Clear();

        // Xoa card cu (neu co) trong CardRow
        for (int i = cardRow.childCount - 1; i >= 0; i--)
            Destroy(cardRow.GetChild(i).gameObject);

        // Xao tron danh sach EventData (khong sua list goc trong LevelData)
        List<EventData> shuffled = new List<EventData>(level.events);
        Shuffle(shuffled);

        // Neu vo tinh xao ra dung thu tu -> xao lai (tranh man vo lam da thang san)
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
        if (gameEnded) return;

        if (firstSelected == null)
        {
            firstSelected = card;
            card.Select();
            return;
        }

        if (firstSelected == card)
        {
            card.Deselect();
            firstSelected = null;
            return;
        }

        SwapCards(firstSelected, card);
        firstSelected.Deselect();
        card.Deselect();
        firstSelected = null;
    }

    private void SwapCards(EventCard a, EventCard b)
    {
        int ia = boardCards.IndexOf(a);
        int ib = boardCards.IndexOf(b);
        if (ia < 0 || ib < 0) return;

        (boardCards[ia], boardCards[ib]) = (boardCards[ib], boardCards[ia]);
        RefreshOrder();

        history.Push((ia, ib));

        movesLeft--;
        UpdateMovesUI();

        CheckGameEnd();
    }

    // Dat lai vi tri hien thi (sibling index) khop voi thu tu trong list boardCards
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
        if (gameEnded || history.Count == 0) return;

        var (ia, ib) = history.Pop();
        (boardCards[ia], boardCards[ib]) = (boardCards[ib], boardCards[ia]);
        RefreshOrder();

        movesLeft++; // tra lai 1 luot vi vua hoan tac
        UpdateMovesUI();
    }

    // ---------------- GOI Y ----------------

    public void ShowHint()
    {
        if (gameEnded) return;

        for (int i = 0; i < boardCards.Count - 1; i++)
        {
            if (boardCards[i].Data.year > boardCards[i + 1].Data.year)
            {
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
            GameManager.Instance?.WinGame();
            return;
        }

        if (movesLeft <= 0)
        {
            gameEnded = true;
            GameManager.Instance?.LoseGame();
        }
    }

    // ---------------- UI ----------------

    private void UpdateMovesUI()
    {
        if (movesLeftText != null)
            movesLeftText.text = movesLeft.ToString();
    }
}