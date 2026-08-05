using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text cardName;

    [Header("Memory Mode")]
    [SerializeField] private Image backImage;

    [Header("Selection Highlight")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor =
        new Color(1f, 0.85f, 0.3f);

    private EventCard eventCard;
    private bool memoryMarked;

    private void Awake()
    {
        eventCard = GetComponent<EventCard>();
    }

    public void Refresh()
    {
        if (eventCard == null || eventCard.Data == null)
            return;

        if (cardName != null)
            cardName.text = eventCard.Data.eventName;

        if (cardImage != null)
            cardImage.sprite = eventCard.Data.image;

        memoryMarked = false;
        UpdateHighlight();
    }

    public void SetSelected(bool selected)
    {
        UpdateHighlight();
    }

    // Đánh dấu khi người chơi bấm lần đầu
    public void SetMemoryMarked(bool marked)
    {
        memoryMarked = marked;
        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        bool highlighted =
            memoryMarked ||
            (eventCard != null && eventCard.IsSelected);

        Color targetColor = highlighted
            ? selectedColor
            : normalColor;

        if (cardImage != null)
            cardImage.color = targetColor;

        if (backImage != null)
            backImage.color = targetColor;
    }
}