using UnityEngine;

[CreateAssetMenu(fileName = "New Event", menuName = "Timeline Sort/Event Data")]
public class EventData : ScriptableObject
{
    public int id;
    public string eventName;
    public long year;
    public Sprite image;
    [Header("Card Image Layout")]
    [Tooltip("Bật để sử dụng vị trí và độ phóng riêng cho ảnh này.")]
    public bool useCustomImageLayout = false;

    [Tooltip("1 = kích thước mặc định; 1.2 = phóng lớn 20%.")]
    [Range(0.5f, 2f)]
    public float imageZoom = 1f;

    [Tooltip("Độ dịch chuyển so với vị trí mặc định của EventImage.")]
    public Vector2 imageOffset = Vector2.zero;
}