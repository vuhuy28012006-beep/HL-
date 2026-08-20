using UnityEngine;
using TMPro;

// Gan vao tung nut Chapter tren scene Map.
public class ChapterSelectButton : MonoBehaviour
{
    [SerializeField]
    private ChapterData chapter;

    [Header("Cac man thuoc chuong")]
    [SerializeField]
    private LevelData[] levelsInChapter;

    [Header("Text tong sao")]
    [SerializeField]
    private TMP_Text starsText;

    // Goi tu Button > On Click()
    public void SelectChapter()
    {
        if (chapter == null)
        {
            Debug.LogError(
                "ChapterSelectButton: chua gan ChapterData!"
            );

            return;
        }

        ChapterSession.SelectedChapter =
            chapter;

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

        if (chapter == null)
        {
            Debug.LogWarning(
                "ChapterSelectButton: chua gan ChapterData.",
                this
            );

            if (starsText != null)
                starsText.text = "0/0";

            return;
        }

        if (levelsInChapter != null)
        {
            foreach (
                LevelData level in levelsInChapter
            )
            {
                if (level == null)
                    continue;

                earnedStars += Mathf.Clamp(
                    SaveManager.GetStars(
                        chapter.chapterNumber,
                        level.levelNumber
                    ),
                    0,
                    3
                );

                levelCount++;
            }
        }

        int maximumStars =
            levelCount * 3;

        if (starsText != null)
        {
            starsText.text =
                earnedStars +
                "/" +
                maximumStars;
        }
    }
}