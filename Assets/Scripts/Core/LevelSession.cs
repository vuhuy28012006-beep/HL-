using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelSession
{
    public static LevelData SelectedLevel;

    // Lưu chapter của level đang được chọn.
    // Nhờ đó phân biệt được Level 1 của các chapter.
    public static int SelectedChapterNumber = 0;

    // Reset màn sẽ bỏ qua hướng dẫn đúng một lần.
    public static bool SkipTutorialOnce = false;

    public const string GameplaySceneName =
        "GamePlay";

    private static void SaveSelectedLevel()
    {
        if (SelectedLevel == null)
            return;

        int chapterNumber =
            SelectedChapterNumber;

        // Trường hợp vào màn từ Level Select.
        if (chapterNumber < 1 &&
            ChapterSession.SelectedChapter != null)
        {
            chapterNumber =
                ChapterSession
                    .SelectedChapter
                    .chapterNumber;
        }

        // Trường hợp vào màn bằng nút Chơi tiếp.
        if (chapterNumber < 1)
        {
            chapterNumber =
                SelectedLevel.chapterNumber;
        }

        // Giá trị dự phòng.
        if (chapterNumber < 1)
            chapterNumber = 1;

        SelectedChapterNumber =
            chapterNumber;

        SaveManager.SetLastPlayedLevel(
            chapterNumber,
            SelectedLevel.levelNumber
        );
    }

    public static void LoadGameplayScene()
    {
        SaveSelectedLevel();

        SceneManager.LoadScene(
            GameplaySceneName
        );
    }

    public static void LoadGameplayScene(
        string sceneName
    )
    {
        SaveSelectedLevel();

        if (string.IsNullOrEmpty(sceneName))
        {
            sceneName =
                GameplaySceneName;
        }

        SceneManager.LoadScene(sceneName);
    }
}