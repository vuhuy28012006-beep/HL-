using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelSession
{
    public static LevelData SelectedLevel;

    // Reset màn sẽ đặt true để bỏ qua hướng dẫn đúng một lần.
    public static bool SkipTutorialOnce = false;

    public const string GameplaySceneName = "GamePlay";

    private static void SaveSelectedLevel()
    {
        if (SelectedLevel == null)
            return;

        SaveManager.SetLastPlayedLevel(
            SelectedLevel.levelNumber
        );
    }

    public static void LoadGameplayScene()
    {
        SaveSelectedLevel();
        SceneManager.LoadScene(GameplaySceneName);
    }

    public static void LoadGameplayScene(string sceneName)
    {
        SaveSelectedLevel();

        if (string.IsNullOrEmpty(sceneName))
            sceneName = GameplaySceneName;

        SceneManager.LoadScene(sceneName);
    }
}