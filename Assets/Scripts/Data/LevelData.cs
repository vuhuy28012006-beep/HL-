using System.Collections.Generic;
using UnityEngine;

public enum SortMode
{
    FreeSwap,
    BubbleSort,
    SelectionSort,
    InsertionSort
}
public enum LevelLimitType
{
    Moves,
    Time
}

[CreateAssetMenu(
    fileName = "New Level",
    menuName = "Timeline Sort/Level Data"
)]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    public int levelNumber;
    public string levelName;

    [Header("Cards")]
    public List<EventData> events = new List<EventData>();

    [Header("Rules")]
    public SortMode sortMode = SortMode.FreeSwap;

    [Header("Level Limit")]
    public LevelLimitType limitType = LevelLimitType.Moves;

    [Tooltip("Số lượt tối đa nếu Limit Type là Moves")]
    public int maxMoves = 5;

    [Tooltip("Thời gian tối đa tính bằng giây nếu Limit Type là Time")]
    [Min(1f)]
    public float timeLimitSeconds = 60f;

    [Header("Memory Mode")]
    public bool useMemoryMode = false;

    [Tooltip("Thời gian xem tất cả thẻ trước khi úp")]
    [Range(3f, 5f)]
    public float previewTime = 4f;

    [Tooltip("Thời gian xem một thẻ sau khi lật")]
    [Range(0.5f, 3f)]
    public float revealTime = 2f;
    
    [Header("Level Tutorial")]
    public bool showLevelTutorial = true;

    [Tooltip("Tiêu đề hướng dẫn riêng của màn")]
    public string tutorialTitle = "HƯỚNG DẪN";

    [TextArea(3, 10)]
    [Tooltip("Nội dung hướng dẫn riêng của màn")]
    public string tutorialText;

    [Tooltip("Ảnh minh họa hướng dẫn riêng của màn")]
    public Sprite tutorialImage;
}