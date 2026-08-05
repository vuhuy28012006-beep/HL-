using UnityEngine;
using UnityEngine.SceneManagement;

// Dat vao: Assets/Scripts/Core/LoadSceneButton.cs
// Gan vao BAT KY nut nao can chuyen scene theo ten co dinh (khong can them ham
// rieng trong GameManager cho tung truong hop nhu Tutorial).

public class LoadSceneButton : MonoBehaviour
{
    [SerializeField] private string sceneName;

    // Goi tu Button > On Click ()
    public void Load()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("LoadSceneButton: chua dien Scene Name!");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}
