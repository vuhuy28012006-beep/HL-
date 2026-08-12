using System.Collections;
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

    [Header("Selection Zoom")]
    [Tooltip("Thẻ phóng to thêm bao nhiêu khi được chọn. 0.2 = to hơn 20%.")]
    [SerializeField] private float selectedZoomAmount = 0.2f;
    [SerializeField] private float zoomAnimDuration = 0.15f;

    [Header("Locked (the bi khoa co dinh)")]
    [Tooltip("Mau phu len the khi bi khoa (mac dinh: xam nhat).")]
    [SerializeField] private Color lockedColor = new Color(0.55f, 0.55f, 0.55f);
    [Tooltip("Icon o khoa hien thi tren the khi bi khoa. Co the de trong neu khong can.")]
    [SerializeField] private GameObject lockIcon;

    private EventCard eventCard;
    private RectTransform rectTransform;
    private Coroutine zoomRoutine;
    private bool memoryMarked;
    private bool isLocked;

    private void Awake()
    {
        eventCard = GetComponent<EventCard>();
        rectTransform = GetComponent<RectTransform>();
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
        isLocked = false;

        if (lockIcon != null)
            lockIcon.SetActive(false);

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

    // Hien thi trang thai khoa co dinh (vd: thẻ đầu tiên bị khóa).
    public void SetLocked(bool locked)
    {
        isLocked = locked;

        if (lockIcon != null)
            lockIcon.SetActive(locked);

        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        // The bi khoa luon uu tien hien mau khoa, khong bi de len boi mau chon/mau thuong.
        if (isLocked)
        {
            if (cardImage != null)
                cardImage.color = lockedColor;

            if (backImage != null)
                backImage.color = lockedColor;

            AnimateZoom(1f);
            return;
        }

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

        float targetScale = highlighted
            ? 1f + selectedZoomAmount
            : 1f;

        AnimateZoom(targetScale);
    }

    private void AnimateZoom(float targetScale)
    {
        if (rectTransform == null)
            return;

        if (!gameObject.activeInHierarchy)
        {
            // Object đang tắt (vd đang bị disable) thì set thẳng, khỏi chạy coroutine
            rectTransform.localScale = Vector3.one * targetScale;
            return;
        }

        if (zoomRoutine != null)
            StopCoroutine(zoomRoutine);

        zoomRoutine = StartCoroutine(ZoomTo(targetScale));
    }

    private IEnumerator ZoomTo(float targetScale)
    {
        Vector3 startScale = rectTransform.localScale;
        Vector3 endScale = Vector3.one * targetScale;

        float t = 0f;
        while (t < zoomAnimDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / zoomAnimDuration);
            k = k * k * (3f - 2f * k); // ease in-out, đồng bộ style với AnimateSwap

            rectTransform.localScale = Vector3.Lerp(startScale, endScale, k);
            yield return null;
        }

        rectTransform.localScale = endScale;
        zoomRoutine = null;
    }
}