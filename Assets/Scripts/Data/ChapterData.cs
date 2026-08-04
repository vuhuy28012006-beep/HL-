using System.Collections.Generic;
using UnityEngine;

// Dat vao: Assets/Scripts/Data/ChapterData.cs

[CreateAssetMenu(fileName = "New Chapter", menuName = "Timeline Sort/Chapter Data")]
public class ChapterData : ScriptableObject
{
    [Header("Thong tin chuong")]
    public string chapterName;       // vd: "Tien Hoa Sinh Vat", "Lich Su The Gioi", "Bang Tuan Hoan"
    [TextArea] public string description;
    public Sprite chapterIcon;       // anh dai dien chuong, khong bat buoc

    [Header("Cac man thuoc chuong nay")]
    public List<LevelData> levels = new List<LevelData>();
}
