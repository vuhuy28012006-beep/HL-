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
    private const string LastPlayedLevelKey = "LastPlayedLevel";

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
    public static void SetLastPlayedLevel(int levelNumber)
    {
        if (levelNumber < 1)
            return;

        PlayerPrefs.SetInt(LastPlayedLevelKey, levelNumber);
        PlayerPrefs.Save();
    }

    public static int GetLastPlayedLevel()
    {
        // Với người chơi cũ chưa có dữ liệu LastPlayedLevel,
        // sử dụng level cao nhất đã mở khóa.
        return PlayerPrefs.GetInt(
            LastPlayedLevelKey,
            GetHighestUnlockedLevel()
        );
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

    public static void ResetLevelProgress(
        int maxLevelToClear = 200
    )
    {
        // Xóa level cao nhất đã mở.
        PlayerPrefs.DeleteKey(HighestUnlockedKey);
        PlayerPrefs.DeleteKey(LastPlayedLevelKey);

        // Xóa số sao của các level.
        for (int level = 1;
            level <= maxLevelToClear;
            level++)
        {
            PlayerPrefs.DeleteKey(
                StarsKeyPrefix + level
            );
        }

        PlayerPrefs.Save();

        Debug.Log(
            "Đã reset tiến trình level. " +
            "Level 1 mở, các level sau bị khóa."
        );
    }
    // Dung khi test, xoa het du lieu da luu
    public static void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
