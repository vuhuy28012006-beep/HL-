using UnityEngine;

public static class SaveManager
{
    private const string LastPlayedChapterKey =
        "LastPlayedChapter";

    private const string LastPlayedLevelKey =
        "LastPlayedLevel";

    private const string TutorialCompletedKey =
        "TutorialCompleted";

    // Khóa lưu level cao nhất của từng chapter.
    private static string GetHighestUnlockedKey(
        int chapterNumber
    )
    {
        return "Chapter_" + chapterNumber +
               "_HighestUnlockedLevel";
    }

    // Khóa lưu sao của từng level trong từng chapter.
    private static string GetStarsKey(
        int chapterNumber,
        int levelNumber
    )
    {
        return "Chapter_" + chapterNumber +
               "_Level_Stars_" + levelNumber;
    }

    // Mỗi chapter mặc định chỉ mở Level 1.
    public static int GetHighestUnlockedLevel(
        int chapterNumber
    )
    {
        if (chapterNumber < 1)
            chapterNumber = 1;

        return PlayerPrefs.GetInt(
            GetHighestUnlockedKey(chapterNumber),
            1
        );
    }

    public static bool IsLevelUnlocked(
        int chapterNumber,
        int levelNumber
    )
    {
        if (levelNumber < 1)
            return false;

        return levelNumber <=
               GetHighestUnlockedLevel(chapterNumber);
    }

    // Chỉ mở level trong đúng chapter được truyền vào.
    public static void UnlockLevel(
        int chapterNumber,
        int levelNumber
    )
    {
        if (chapterNumber < 1 || levelNumber < 1)
            return;

        int currentHighest =
            GetHighestUnlockedLevel(chapterNumber);

        if (levelNumber > currentHighest)
        {
            PlayerPrefs.SetInt(
                GetHighestUnlockedKey(chapterNumber),
                levelNumber
            );

            PlayerPrefs.Save();
        }
    }

    public static void SetLastPlayedLevel(
        int chapterNumber,
        int levelNumber
    )
    {
        if (chapterNumber < 1 || levelNumber < 1)
            return;

        PlayerPrefs.SetInt(
            LastPlayedChapterKey,
            chapterNumber
        );

        PlayerPrefs.SetInt(
            LastPlayedLevelKey,
            levelNumber
        );

        PlayerPrefs.Save();
    }

    public static int GetLastPlayedChapter()
    {
        return PlayerPrefs.GetInt(
            LastPlayedChapterKey,
            1
        );
    }

    public static int GetLastPlayedLevel()
    {
        int chapterNumber =
            GetLastPlayedChapter();

        return PlayerPrefs.GetInt(
            LastPlayedLevelKey,
            GetHighestUnlockedLevel(chapterNumber)
        );
    }

    public static bool HasCompletedTutorial()
    {
        return PlayerPrefs.GetInt(
            TutorialCompletedKey,
            0
        ) == 1;
    }

    public static void SetTutorialCompleted()
    {
        PlayerPrefs.SetInt(
            TutorialCompletedKey,
            1
        );

        PlayerPrefs.Save();
    }

    public static int GetStars(
        int chapterNumber,
        int levelNumber
    )
    {
        return PlayerPrefs.GetInt(
            GetStarsKey(
                chapterNumber,
                levelNumber
            ),
            0
        );
    }

    public static void SetStars(
        int chapterNumber,
        int levelNumber,
        int stars
    )
    {
        if (chapterNumber < 1 || levelNumber < 1)
            return;

        stars = Mathf.Clamp(stars, 0, 3);

        int currentStars = GetStars(
            chapterNumber,
            levelNumber
        );

        // Chỉ lưu khi số sao mới cao hơn số sao cũ.
        if (stars > currentStars)
        {
            PlayerPrefs.SetInt(
                GetStarsKey(
                    chapterNumber,
                    levelNumber
                ),
                stars
            );

            PlayerPrefs.Save();
        }
    }

    // Reset sao và tiến trình nhưng giữ âm thanh,
    // tutorial và các cài đặt khác.
    public static void ResetLevelProgress(
        int maxChapterToClear = 50,
        int maxLevelToClear = 200
    )
    {
        PlayerPrefs.DeleteKey(
            LastPlayedChapterKey
        );

        PlayerPrefs.DeleteKey(
            LastPlayedLevelKey
        );

        // Xóa khóa cũ trước khi chia theo chapter.
        PlayerPrefs.DeleteKey(
            "HighestUnlockedLevel"
        );

        for (int level = 1;
             level <= maxLevelToClear;
             level++)
        {
            PlayerPrefs.DeleteKey(
                "Level_Stars_" + level
            );
        }

        // Xóa các khóa mới theo chapter.
        for (int chapter = 1;
             chapter <= maxChapterToClear;
             chapter++)
        {
            PlayerPrefs.DeleteKey(
                GetHighestUnlockedKey(chapter)
            );

            for (int level = 1;
                 level <= maxLevelToClear;
                 level++)
            {
                PlayerPrefs.DeleteKey(
                    GetStarsKey(
                        chapter,
                        level
                    )
                );
            }
        }

        PlayerPrefs.Save();

        Debug.Log(
            "Đã reset: mỗi chapter chỉ mở Level 1."
        );
    }

    // Chỉ dùng để test vì xóa toàn bộ PlayerPrefs.
    public static void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}