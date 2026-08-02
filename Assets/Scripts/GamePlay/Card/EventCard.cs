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

    private void Awake()
    {
        visual = GetComponent<CardVisual>();
    }

    public void Initialize(EventData data, int index)
    {
        Data = data;
        CurrentIndex = index;
        IsSelected = false;

        if (visual == null)
            visual = GetComponent<CardVisual>();

        visual?.Refresh();
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

    // Duoc goi tu dong khi nguoi choi bam/cham vao card (nho co EventSystem + Graphic Raycaster tren Canvas)
    public void OnPointerClick(PointerEventData eventData)
    {
        if (BoardManager.Instance != null)
            BoardManager.Instance.OnCardClicked(this);
    }
}