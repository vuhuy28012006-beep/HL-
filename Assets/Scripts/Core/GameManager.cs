using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState CurrentState { get; private set; }

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
        Debug.Log("Game Started");
    }

    public void WinGame()
    {
        CurrentState = GameState.Win;
        Debug.Log("You Win!");
    }

    public void LoseGame()
    {
        CurrentState = GameState.Lose;
        Debug.Log("You Lose!");
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}