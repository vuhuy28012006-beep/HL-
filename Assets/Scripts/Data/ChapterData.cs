using System.Collections.Generic;
using UnityEngine;

// Dat vao: Assets/Scripts/Data/ChapterData.cs

[CreateAssetMenu(
    fileName = "New Chapter",
    menuName = "Timeline Sort/Chapter Data"
)]
public class ChapterData : ScriptableObject
{
    [Header("Thong tin chuong")]
    public int chapterNumber = 1;

    public string chapterName;

    [TextArea(2, 5)]
    public string description;

    [Tooltip("Icon dai dien cua chapter tren scene Map")]
    public Sprite chapterIcon;

    [Header("Level Select")]
    [Tooltip("Background rieng cua ban do chon level trong chapter nay")]
    public Sprite levelSelectBackground;

    [Header("Cac man thuoc chuong nay")]
    [Tooltip("So nut level se duoc tao dua tren danh sach nay")]
    public List<LevelData> levels = new List<LevelData>();
}