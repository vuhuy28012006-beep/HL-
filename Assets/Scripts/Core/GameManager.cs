using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

// Ghi de vao: Assets/Scripts/Core/GameManager.cs

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState CurrentState { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [Header("Stars")]
    [SerializeField] private Image[] starImages;

    [SerializeField] private Sprite filledStar;

    [SerializeField] private Sprite emptyStar;

    [Header("Win Panel - hien thi ket qua (khong bat buoc)")]
    [SerializeField] private TMPro.TMP_Text starsText;

    [Header("Danh sach TOAN BO LevelData trong game (keo het vao day, thu tu tuy y)")]
    [SerializeField] private LevelData[] allLevels;

    private int currentLevelNumber;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        CurrentState = GameState.Playing;
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }

    public void WinGame(int movesLeft, int maxMoves, int levelNumber)
    {
        CurrentState = GameState.Win;
        currentLevelNumber = levelNumber;

        BoardManager board = BoardManager.Instance;

        int stars = StarCalculator.CalculateStars(
                                    board.MovesLeft,
                                    board.MaxMoves,
                                    board.HintsUsed,
                                    board.UndosUsed);

        UpdateStarUI(stars);

        SaveManager.SetStars(levelNumber, stars);
        SaveManager.UnlockLevel(levelNumber + 1);

        if (starsText != null)
            starsText.text = stars + " sao";

        if (winPanel != null) winPanel.SetActive(true);
    }
    public void LoseGame()
    {
        CurrentState = GameState.Lose;
        if (losePanel != null) losePanel.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Map");
    }


    
    // Goi tu nut "Man tiep theo" tren Win Panel
    public void GoToNextLevel()
    {
        SceneManager.LoadScene("LevelSelect");
    }
        public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
 
        public void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // de test duoc trong Editor
    #else
        Application.Quit(); // chi hoat dong tren may that (APK), khong co tac dung trong Editor
    #endif
    }
        private void UpdateStarUI(int stars)
    {
        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].sprite =
                i < stars ? filledStar : emptyStar;
        }
    }
}