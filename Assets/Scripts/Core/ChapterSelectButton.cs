using UnityEngine;

// Dat vao: Assets/Scripts/Core/ChapterSelectButton.cs
// Gan vao tung nut Chuong tren scene Map

public class ChapterSelectButton : MonoBehaviour
{
    [SerializeField] private ChapterData chapter;

    // Goi tu Button > On Click ()
    public void SelectChapter()
    {
        if (chapter == null)
        {
            Debug.LogError("ChapterSelectButton: chua gan ChapterData!");
            return;
        }

        ChapterSession.SelectedChapter = chapter;
        ChapterSession.LoadLevelListScene();
    }
}
