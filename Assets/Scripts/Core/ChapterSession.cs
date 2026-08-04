using UnityEngine.SceneManagement;

// Dat vao: Assets/Scripts/Core/ChapterSession.cs

public static class ChapterSession
{
    public static ChapterData SelectedChapter;

    public const string LevelListSceneName = "LevelSelect";

    public static void LoadLevelListScene()
    {
        SceneManager.LoadScene(LevelListSceneName);
    }
}
