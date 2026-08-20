using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] private LevelData levelData;

    [Header("Hien thi trang thai")]
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private TMP_Text starsText;
    [SerializeField] private TMP_Text levelNumberText;
    [SerializeField] private GameObject currentMarker;

    [Header("Ba ngoi sao tren Level")]
    [SerializeField] private Image[] starImages;
    [SerializeField] private Sprite filledStar;
    [SerializeField] private Sprite emptyStar;

    private Button button;
    private int chapterNumber;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        // Dùng cho button được gán LevelData
        // trực tiếp trong Inspector.
        if (levelData != null)
        {
            chapterNumber =
                ResolveChapterNumber();

            RefreshState();
        }
    }

    public void Setup(
        LevelData data,
        int owningChapterNumber
    )
    {
        levelData = data;

        chapterNumber = Mathf.Max(
            1,
            owningChapterNumber
        );

        if (levelNumberText != null)
        {
            levelNumberText.text =
                data.levelNumber.ToString();
        }

        RefreshState();
    }

    // Giữ lại để các prefab hoặc code cũ
    // chưa bị lỗi ngay lập tức.
    public void Setup(LevelData data)
    {
        int fallbackChapter =
            data != null
                ? data.chapterNumber
                : 1;

        Setup(
            data,
            fallbackChapter
        );
    }

    private int ResolveChapterNumber()
    {
        if (chapterNumber > 0)
            return chapterNumber;

        if (ChapterSession.SelectedChapter != null &&
            ChapterSession
                .SelectedChapter
                .chapterNumber > 0)
        {
            return ChapterSession
                .SelectedChapter
                .chapterNumber;
        }

        if (levelData != null &&
            levelData.chapterNumber > 0)
        {
            return levelData.chapterNumber;
        }

        return 1;
    }
    private void RefreshState()
{
    if (levelData == null)
        return;

    if (button == null)
        button = GetComponent<Button>();

    chapterNumber =
        ResolveChapterNumber();

    bool unlocked =
        SaveManager.IsLevelUnlocked(
            chapterNumber,
            levelData.levelNumber
        );

    if (button != null)
        button.interactable = unlocked;

    if (lockIcon != null)
        lockIcon.SetActive(!unlocked);

    int earnedStars =
        SaveManager.GetStars(
            chapterNumber,
            levelData.levelNumber
        );

    bool isCurrentLevel =
        unlocked &&
        earnedStars == 0 &&
        levelData.levelNumber ==
            SaveManager.GetHighestUnlockedLevel(
                chapterNumber
            );

    if (currentMarker != null)
    {
        currentMarker.SetActive(
            isCurrentLevel
        );
    }

    if (starsText != null)
    {
        starsText.text =
            unlocked
                ? earnedStars + "/3"
                : "";
    }

    RefreshStarImages(earnedStars);
}

    private void RefreshStarImages(int earnedStars)
    {
        if (starImages == null || starImages.Length == 0)
            return;

        if (filledStar == null || emptyStar == null)
        {
            Debug.LogWarning(
                "LevelSelectButton: Chua gan Filled Star hoac Empty Star.",
                this
            );

            return;
        }

        earnedStars = Mathf.Clamp(earnedStars, 0, 3);

        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] == null)
                continue;

            starImages[i].sprite =
                i < earnedStars ? filledStar : emptyStar;

            starImages[i].color = Color.white;
            starImages[i].enabled = true;
        }
    }

    public void LoadLevel()
{
    if (levelData == null)
    {
        Debug.LogError(
            "LevelSelectButton: chưa có LevelData!"
        );

        return;
    }

    chapterNumber =
        ResolveChapterNumber();

    if (!SaveManager.IsLevelUnlocked(
            chapterNumber,
            levelData.levelNumber
        ))
    {
        Debug.Log(
            "Level này đang bị khóa!"
        );

        return;
    }

    LevelSession.SelectedLevel =
        levelData;

    LevelSession.SelectedChapterNumber =
        chapterNumber;

    LevelSession.LoadGameplayScene(
        levelData.gameplaySceneName
    );
}
}