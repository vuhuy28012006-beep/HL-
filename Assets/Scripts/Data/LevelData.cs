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
    [Header("Chapter Information")]
    public int chapterNumber = 1;
    public string chapterName = "Sinh vật";

    [Header("Background")]
    [Tooltip("Anh nen rieng cho man nay. Neu de trong (null), se dung " +
        "anh nen mac dinh dang duoc gan san trong scene GamePlay.")]
    public Sprite backgroundImage;

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

    [Header("Scene GamePlay rieng cho man nay")]
    [Tooltip("Ten scene GamePlay rieng cho man nay (phai duoc them vao " +
        "File > Build Settings > Scenes In Build). De trong hoac giu " +
        "\"GamePlay\" se dung scene GamePlay chung mac dinh, khong pha vo " +
        "cac man cu.")]
    public string gameplaySceneName = "GamePlay";
}