using UnityEngine;


// Dat file nay vao: Assets/Scripts/Core/SaveManager.cs
//
// Dung PlayerPrefs (luu tren may, khong can server) de nho:
// - Level cao nhat da mo khoa
// - So sao dat duoc o tung level
//
// Quy uoc: Level 1 luon mo san. Qua Level N se tu mo Level N+1.

public static class SaveManager
{
    private const string HighestUnlockedKey = "HighestUnlockedLevel";
    private const string StarsKeyPrefix = "Level_Stars_";

    // Level cao nhat nguoi choi da mo khoa (mac dinh = 1, vi Level 1 luon mo san)
    public static int GetHighestUnlockedLevel()
    {
        return PlayerPrefs.GetInt(HighestUnlockedKey, 1);
    }

    public static bool IsLevelUnlocked(int levelNumber)
    {
        return levelNumber <= GetHighestUnlockedLevel();
    }

    public static void UnlockLevel(int levelNumber)
    {
        if (levelNumber > GetHighestUnlockedLevel())
        {
            PlayerPrefs.SetInt(HighestUnlockedKey, levelNumber);
            PlayerPrefs.Save();
        }
    }
    // Them vao SaveManager.cs (dat canh cac ham khac, vd sau UnlockLevel)
 
    private const string TutorialCompletedKey = "TutorialCompleted";
    
    public static bool HasCompletedTutorial()
    {
        return PlayerPrefs.GetInt(TutorialCompletedKey, 0) == 1;
    }
    
    public static void SetTutorialCompleted()
    {
        PlayerPrefs.SetInt(TutorialCompletedKey, 1);
        PlayerPrefs.Save();
    }
    

    // Sao dat duoc o 1 level (0 neu chua choi lan nao)
    public static int GetStars(int levelNumber)
    {
        return PlayerPrefs.GetInt(StarsKeyPrefix + levelNumber, 0);
    }

    // Chi ghi de neu sao moi cao hon sao cu (choi lai diem thap hon khong bi mat thanh tich cu)
    public static void SetStars(int levelNumber, int stars)
    {
        stars = Mathf.Clamp(stars, 0, 3);
        int current = GetStars(levelNumber);

        if (stars > current)
        {
        PlayerPrefs.SetInt(StarsKeyPrefix + levelNumber, stars);
        PlayerPrefs.Save();
        }
    }

    // Dung khi test, xoa het du lieu da luu
    public static void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
