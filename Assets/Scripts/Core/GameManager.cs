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

    public void WinGame()
    {
        CurrentState = GameState.Win;

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

    // Goi tam thoi khi chua co Chapter Map / Level Select that
    public void BackToMenu()
    {
        Debug.Log("Back to menu - chua co scene menu, can lam sau");
        // Sau nay thay bang: SceneManager.LoadScene("MainMenu");
    }
}
