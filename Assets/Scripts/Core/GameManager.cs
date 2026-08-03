using UnityEngine;
using UnityEngine.SceneManagement;

// Ghi de vao: Assets/Scripts/Core/GameManager.cs

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState CurrentState { get; private set; }

    [Header("Panels (keo Panel Thang/Thua vao day)")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Win Panel - hien thi ket qua (khong bat buoc)")]
    [SerializeField] private TMPro.TMP_Text starsText; // vd: hien "3 sao", co the de trong

    private int lastStarsEarned;

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

    // movesLeft: so luot con du luc thang, dung de tinh sao
    public void WinGame(int movesLeft, int maxMoves, int levelNumber)
    {
        CurrentState = GameState.Win;

        lastStarsEarned = CalculateStars(movesLeft, maxMoves);

        SaveManager.SetStars(levelNumber, lastStarsEarned);
        SaveManager.UnlockLevel(levelNumber + 1);

        if (starsText != null)
            starsText.text = lastStarsEarned + " sao";

        if (winPanel != null) winPanel.SetActive(true);
    }

    private int CalculateStars(int movesLeft, int maxMoves)
    {
        if (maxMoves <= 0) return 3;

        if (movesLeft >= maxMoves / 2f) return 3;
        if (movesLeft >= 1) return 2;
        return 1;
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
        SceneManager.LoadScene("LevelSelect");
    }
}