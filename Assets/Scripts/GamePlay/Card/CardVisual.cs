using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text cardName;

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
}