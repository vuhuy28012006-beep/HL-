using UnityEngine;
using TMPro;

// Dat vao: Assets/Scripts/Core/ChapterSelectButton.cs
// Gan vao tung nut Chuong tren scene Map
// Mỗi lần tạo chương hãy thêm vào level trong btnchapter.... -> cac man thuoc chuong -> levels in chapter +

public class ChapterSelectButton : MonoBehaviour
{
    [SerializeField] private ChapterData chapter;
    [Header("Cac man thuoc chuong")]
    [SerializeField] private LevelData[] levelsInChapter;

    [Header("Text tong sao")]
    [SerializeField] private TMP_Text starsText;


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
    private void OnEnable()
    {
        RefreshStars();
    }

    public void RefreshStars()
    {
        int earnedStars = 0;
        int levelCount = 0;

        if (levelsInChapter != null)
        {
            foreach (LevelData level in levelsInChapter)
            {
                if (level == null)
                    continue;

                earnedStars += Mathf.Clamp(
                    SaveManager.GetStars(level.levelNumber),
                    0,
                    3
                );

                levelCount++;
            }
        }

        int maximumStars = levelCount * 3;

        if (starsText != null)
        {
            starsText.text = earnedStars + "/" + maximumStars ;
        }
    }

}
