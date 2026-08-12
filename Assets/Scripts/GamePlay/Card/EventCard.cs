using UnityEngine;
using UnityEngine.EventSystems;

// Ghi de vao: Assets/Scripts/GamePlay/Card/EventCard.cs
// (thay the toan bo file cu, giu nguyen cac ham co san, chi them phan nhan click)

public class EventCard : MonoBehaviour, IPointerClickHandler
{
    [Header("Card Data")]
    public EventData Data;

    [Header("Board")]
    public int CurrentIndex;

    public bool IsSelected;

    private CardVisual visual;
    [Header("Memory Mode")]
    [SerializeField] private GameObject frontSide;
    [SerializeField] private GameObject backSide;

    public bool IsFaceUp { get; private set; } = true;

    // Thẻ bị khóa (vd: thẻ đầu tiên bị khóa cố định) sẽ không thể chọn/đổi chỗ.
    public bool IsLocked { get; private set; }

    private void Awake()
    {
        visual = GetComponent<CardVisual>();
    }

    public void Initialize(EventData data, int index)
    {
        Data = data;
        CurrentIndex = index;
        IsSelected = false;
        IsLocked = false;

        if (visual == null)
            visual = GetComponent<CardVisual>();

        visual?.Refresh();
    }

    // Khóa/mở khóa thẻ. Thẻ bị khóa sẽ không phản hồi khi người chơi bấm/cham vao
    // (xem BoardManager.OnCardClicked), va duoc CardVisual hien thi khac di (vd lam xam).
    public void SetLocked(bool locked)
    {
        IsLocked = locked;

        // Neu dang khoa thi bo chon (khong the vua chon vua khoa).
        if (locked && IsSelected)
            Deselect();

        visual?.SetLocked(locked);
    }

    public void Select()
    {
        IsSelected = true;
        visual?.SetSelected(true);
    }

    public void Deselect()
    {
        IsSelected = false;
        visual?.SetSelected(false);
    }

    public void SetIndex(int newIndex)
    {
        CurrentIndex = newIndex;
    }
    // không thay đổi onpointerclick
    public void SetMemoryMarked(bool marked)
    {
        visual?.SetMemoryMarked(marked);
    }

    // Duoc goi tu dong khi nguoi choi bam/cham vao card (nho co EventSystem + Graphic Raycaster tren Canvas)
    public void OnPointerClick(PointerEventData eventData)
    {
        if (BoardManager.Instance != null)
            BoardManager.Instance.OnCardClicked(this);
    }
    // Úp mở các thẻ trong cách chơi chọn mù
    public void FlipUp()
    {
        IsFaceUp = true;

        if (frontSide != null)
            frontSide.SetActive(true);

        if (backSide != null)
            backSide.SetActive(false);
    }

    public void FlipDown()
    {
        IsFaceUp = false;

        if (frontSide != null)
            frontSide.SetActive(false);

        if (backSide != null)
            backSide.SetActive(true);
    }
}