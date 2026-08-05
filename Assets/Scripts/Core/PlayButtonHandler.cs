using UnityEngine;
using UnityEngine.SceneManagement;

// Dat vao: Assets/Scripts/Core/PlayButtonHandler.cs
// Gan vao nut "Choi ngay" trong scene MainMenu, THAY THE cho LoadSceneButton

public class PlayButtonHandler : MonoBehaviour
{
    // Goi tu Button > On Click ()
    public void Play()
    {
        if (SaveManager.HasCompletedTutorial())
            SceneManager.LoadScene("Map");
        else
            SceneManager.LoadScene("Tutorial_Swap");
    }
}
