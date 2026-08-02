using System.Collections.Generic;
using UnityEngine;

// Dat file nay vao: Assets/Scripts/Data/LevelData.cs
// Dinh nghia du lieu cho 1 man choi: danh sach 5 EventData (sinh vat) + so luot doi toi da

[CreateAssetMenu(fileName = "New Level", menuName = "Timeline Sort/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    public int levelNumber;
    public string levelName;

    [Header("Cards")]
    public List<EventData> events = new List<EventData>();

    [Header("Rules")]
    public int maxMoves = 5;
}