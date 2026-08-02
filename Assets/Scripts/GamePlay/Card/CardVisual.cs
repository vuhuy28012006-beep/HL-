using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Ghi de vao: Assets/Scripts/GamePlay/Card/CardVisual.cs

public class CardVisual : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text cardName;

    [Header("Selection Highlight")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(1f, 0.85f, 0.3f); // vang nhat khi duoc chon

    private EventCard eventCard;

    private void Awake()
    {
        eventCard = GetComponent<EventCard>();
    }

    public void Refresh()
    {
        if (eventCard == null || eventCard.Data == null)
            return;

        cardName.text = eventCard.Data.eventName;
        cardImage.sprite = eventCard.Data.image;
    }

    public void SetSelected(bool selected)
    {
        if (cardImage == null) return;
        cardImage.color = selected ? selectedColor : normalColor;
    }
}