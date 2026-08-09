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

    private int currentLevelNumber;

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
    }

    public void StartGame()
    {
        CurrentState = GameState.Playing;

        if (winPanel != null)
            winPanel.SetActive(false);

        if (losePanel != null)
            losePanel.SetActive(false);
    }

    public void WinGame(int movesLeft, int maxMoves, int levelNumber)
    {
        CurrentState = GameState.Win;
        currentLevelNumber = levelNumber;

        int stars = CalculateStars(movesLeft, maxMoves);

        SaveManager.SetStars(levelNumber, stars);
        SaveManager.UnlockLevel(levelNumber + 1);

        // Neu scene hien tai la scene Tutorial_Swap thi danh dau da hoc xong
        // tutorial, de lan sau bam Play se vao thang Map, khong hien lai nua.
        if (SceneManager.GetActiveScene().name == "Tutorial_Swap")
        {
            SaveManager.SetTutorialCompleted();
        }

        if (starsText != null)
            starsText.text = stars + " sao";

        // Bật panel trước để các Image ngôi sao hoạt động.
        if (winPanel != null)
            winPanel.SetActive(true);

        ShowStars(stars);
    }

    private int CalculateStars(int movesLeft, int maxMoves)
    {
        if (maxMoves <= 0)
            return 3;

        if (movesLeft >= maxMoves / 2f)
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

        SceneManager.LoadScene("LevelSelect");
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

    public void GoToNextLevel()
    {
        int nextLevelNumber = currentLevelNumber + 1;

        if (allLevels == null || allLevels.Length == 0)
        {
            Debug.LogError(
                "Chua gan All Levels trong Inspector cua GameManager."
            );
            return;
        }

        foreach (LevelData level in allLevels)
        {
            if (level == null)
                continue;

            if (level.levelNumber == nextLevelNumber)
            {
                Time.timeScale = 1f;
                LevelSession.SkipTutorialOnce = false;
                LevelSession.SelectedLevel = level;
                LevelSession.LoadGameplayScene(level.gameplaySceneName);
                return;
            }
        }

        Debug.LogWarning(
            "Khong tim thay LevelData cua level " + nextLevelNumber
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