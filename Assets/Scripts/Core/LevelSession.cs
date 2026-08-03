using UnityEngine;
using UnityEngine.SceneManagement;

// Dat file nay vao: Assets/Scripts/Core/LevelSession.cs
//
// Muc dich: giu du lieu "dang choi level nao" xuyen suot luc chuyen scene,
// vi ScriptableObject reference khong the truyen truc tiep qua SceneManager.LoadScene.
//
// Cach dung tu scene Level Select (sau nay):
//   LevelSession.SelectedLevel = levelData;
//   LevelSession.LoadGameplayScene();
//
// Neu SelectedLevel dang trong (vd: ban dang test truc tiep trong scene Gameplay
// bang cach bam Play, chua di qua Level Select), BoardManager se tu dung
// currentLevel da gan san trong Inspector nhu binh thuong - khong bi gay.

public static class LevelSession
{
    public static LevelData SelectedLevel;

    public const string GameplaySceneName = "Gameplay";

    public static void LoadGameplayScene()
    {
        SceneManager.LoadScene(GameplaySceneName);
    }
}
