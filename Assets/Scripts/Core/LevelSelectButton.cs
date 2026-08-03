using UnityEngine;

// Dat file nay vao: Assets/Scripts/Core/LevelSelectButton.cs
//
// Gan script nay vao tung nut chon man trong scene Level Select.
// Moi nut chi can keo dung 1 LevelData tuong ung vao field ben duoi,
// khong can sua code gi them khi them nut cho man khac sau nay.

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] private LevelData levelData;

    // Goi ham nay tu Button > On Click ()
    public void LoadLevel()
    {
        if (levelData == null)
        {
            Debug.LogError("LevelSelectButton: chua gan LevelData cho nut nay!");
            return;
        }

        LevelSession.SelectedLevel = levelData;
        LevelSession.LoadGameplayScene();
    }
}