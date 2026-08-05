using System.Collections.Generic;
using UnityEngine;

public enum SortMode
{
    FreeSwap,
    BubbleSort,
    SelectionSort,
    InsertionSort
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
    public int maxMoves = 5;
    public SortMode sortMode = SortMode.FreeSwap;

    [Header("Memory Mode")]
    public bool useMemoryMode = false;

    [Tooltip("Thời gian xem tất cả thẻ trước khi úp")]
    [Range(3f, 5f)]
    public float previewTime = 4f;

    [Tooltip("Thời gian xem một thẻ sau khi lật")]
    [Range(0.5f, 3f)]
    public float revealTime = 2f;
}