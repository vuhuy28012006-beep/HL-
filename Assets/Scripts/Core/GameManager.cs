using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Ghi de vao: Assets/Scripts/Core/GameManager.cs

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState CurrentState { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Win Panel - hien thi ket qua")]
    [SerializeField] private TMPro.TMP_Text starsText;

    [Header("Win Panel - Stars")]
    [SerializeField] private Image[] starImages;
    [SerializeField] private Sprite filledStar;
    [SerializeField] private Sprite emptyStar;

    [Header("Danh sach TOAN BO LevelData trong game")]
    [SerializeField] private LevelData[] allLevels;
    [Header("Header HUD")]
    [SerializeField] private TMPro.TMP_Text chapterTitleText;
    [SerializeField] private TMPro.TMP_Text levelTitleText;

    private int currentLevelNumber;
    private int currentChapterNumber;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartGame();
        RefreshHeaderHUD();

    }

    public void StartGame()
    {
        CurrentState = GameState.Playing;

        if (winPanel != null)
            winPanel.SetActive(false);

        if (losePanel != null)
            losePanel.SetActive(false);
    }

    public void WinGame(
        int movesLeft,
        int maxMoves,
        int levelNumber
    )
    {
        CurrentState = GameState.Win;

        currentLevelNumber =
            levelNumber;

        currentChapterNumber =
            GetCurrentChapterNumber();

        int stars = CalculateStars(
            movesLeft,
            maxMoves
        );

        SaveManager.SetStars(
            currentChapterNumber,
            levelNumber,
            stars
        );

        // Chỉ mở level tiếp theo
        // trong chính chapter hiện tại.
        SaveManager.UnlockLevel(
            currentChapterNumber,
            levelNumber + 1
        );

        if (SceneManager
                .GetActiveScene()
                .name == "Tutorial_Swap")
        {
            SaveManager.SetTutorialCompleted();
        }

        if (starsText != null)
            starsText.text = stars + " sao";

        if (winPanel != null)
            winPanel.SetActive(true);

        ShowStars(stars);
    }
    private int GetCurrentChapterNumber()
    {
        if (LevelSession.SelectedChapterNumber > 0)
        {
            return LevelSession
                .SelectedChapterNumber;
        }

        if (ChapterSession.SelectedChapter != null &&
            ChapterSession
                .SelectedChapter
                .chapterNumber > 0)
        {
            return ChapterSession
                .SelectedChapter
                .chapterNumber;
        }

        if (LevelSession.SelectedLevel != null &&
            LevelSession
                .SelectedLevel
                .chapterNumber > 0)
        {
            return LevelSession
                .SelectedLevel
                .chapterNumber;
        }

        Debug.LogWarning(
            "Không xác định được chapter. " +
            "Tạm sử dụng Chapter 1."
        );

        return 1;
    }
    private void RefreshHeaderHUD()
    {
        ChapterData selectedChapter =
            ChapterSession.SelectedChapter;

        LevelData selectedLevel =
            LevelSession.SelectedLevel;

        if (chapterTitleText != null)
        {
            if (selectedChapter != null)
            {
                chapterTitleText.text =
                    $"Chương {selectedChapter.chapterNumber}: " +
                    selectedChapter.chapterName;
            }
            else if (selectedLevel != null)
            {
                // Trường hợp vào bằng nút Chơi tiếp.
                chapterTitleText.text =
                    $"Chương {GetCurrentChapterNumber()}: " +
                    selectedLevel.chapterName;
            }
            else
            {
                Debug.LogWarning(
                    "Chưa có chương hoặc level được chọn."
                );
            }
        }

        if (levelTitleText != null && selectedLevel != null)
        {
            levelTitleText.text =
                $"Level {selectedLevel.levelNumber}";
        }
    }

    private int CalculateStars(int movesLeft, int maxMoves)
    {
        // if (maxMoves <= 0)
        //     return 3;

        if (movesLeft >= 2)
            return 3;

        if (movesLeft >= 1)
            return 2;

        return 1;
    }

    private void ShowStars(int earnedStars)
    {
        if (starImages == null || starImages.Length == 0)
        {
            Debug.LogWarning("Chua gan Star Images trong GameManager.");
            return;
        }

        if (filledStar == null || emptyStar == null)
        {
            Debug.LogWarning("Chua gan Filled Star hoac Empty Star.");
            return;
        }

        earnedStars = Mathf.Clamp(earnedStars, 0, starImages.Length);

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

    public void LoseGame()
    {
        CurrentState = GameState.Lose;

        if (losePanel != null)
            losePanel.SetActive(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        LevelSession.SkipTutorialOnce = true;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        LevelSession.SkipTutorialOnce = false;

        if (ChapterSession.SelectedChapter != null)
        {
            SceneManager.LoadScene("LevelSelect");
        }
        else
        {
            // Nếu vào game bằng nút Chơi Tiếp sau khi mở lại ứng dụng,
            // ChapterSession chưa có dữ liệu nên quay về Map an toàn hơn.
            SceneManager.LoadScene("Map");
        }
    }

    public void GoToMap()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Map");
    }
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    public void ContinueLastLevel()
    {
        Time.timeScale = 1f;
        LevelSession.SkipTutorialOnce = false;

        if (allLevels == null ||
            allLevels.Length == 0)
        {
            Debug.LogWarning(
                "Chưa gán All Levels. " +
                "Chuyển người chơi đến Map."
            );

            GoToMap();
            return;
        }

        int lastChapterNumber =
            SaveManager.GetLastPlayedChapter();

        int lastLevelNumber =
            SaveManager.GetLastPlayedLevel();

        if (!SaveManager.IsLevelUnlocked(
                lastChapterNumber,
                lastLevelNumber
            ))
        {
            lastLevelNumber =
                SaveManager
                    .GetHighestUnlockedLevel(
                        lastChapterNumber
                    );
        }

        foreach (LevelData level in allLevels)
        {
            if (level == null)
                continue;

            bool correctChapter =
                level.chapterNumber ==
                lastChapterNumber;

            bool correctLevel =
                level.levelNumber ==
                lastLevelNumber;

            if (!correctChapter ||
                !correctLevel)
            {
                continue;
            }

            ChapterSession.SelectedChapter =
                null;

            LevelSession.SelectedLevel =
                level;

            LevelSession.SelectedChapterNumber =
                lastChapterNumber;

            LevelSession.LoadGameplayScene(
                level.gameplaySceneName
            );

            return;
        }

        Debug.LogWarning(
            "Không tìm thấy Chapter " +
            lastChapterNumber +
            ", Level " +
            lastLevelNumber +
            ". Chuyển đến Map."
        );

        GoToMap();
    }

    public void GoToNextLevel()
    {
        int nextLevelNumber =
            currentLevelNumber + 1;

        if (currentChapterNumber < 1)
        {
            currentChapterNumber =
                GetCurrentChapterNumber();
        }

        if (allLevels == null ||
            allLevels.Length == 0)
        {
            Debug.LogError(
                "Chưa gán All Levels " +
                "trong GameManager."
            );

            return;
        }

        foreach (LevelData level in allLevels)
        {
            if (level == null)
                continue;

            bool correctChapter =
                level.chapterNumber ==
                currentChapterNumber;

            bool correctLevel =
                level.levelNumber ==
                nextLevelNumber;

            if (!correctChapter ||
                !correctLevel)
            {
                continue;
            }

            Time.timeScale = 1f;

            LevelSession.SkipTutorialOnce =
                false;

            LevelSession.SelectedLevel =
                level;

            LevelSession.SelectedChapterNumber =
                currentChapterNumber;

            LevelSession.LoadGameplayScene(
                level.gameplaySceneName
            );

            return;
        }

        Debug.LogWarning(
            "Không tìm thấy Chapter " +
            currentChapterNumber +
            ", Level " +
            nextLevelNumber
        );
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}