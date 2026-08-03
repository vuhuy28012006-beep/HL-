using UnityEngine;
using UnityEngine.UI;

// Ghi de vao: Assets/Scripts/Core/LevelSelectButton.cs

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] private LevelData levelData;

    [Header("Hien thi trang thai (khong bat buoc, co the de trong)")]
    [SerializeField] private GameObject lockIcon;       // hien khi level bi khoa
    [SerializeField] private TMPro.TMP_Text starsText;  // hien so sao da dat, vd "2/3"

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        RefreshState();
    }

    private void RefreshState()
    {
        if (levelData == null) return;

        bool unlocked = SaveManager.IsLevelUnlocked(levelData.levelNumber);

        if (button != null)
            button.interactable = unlocked;

        if (lockIcon != null)
            lockIcon.SetActive(!unlocked);

        if (starsText != null)
        {
            int stars = SaveManager.GetStars(levelData.levelNumber);
            starsText.text = unlocked ? stars + "/3" : "";
        }
    }

    public void LoadLevel()
    {
        if (levelData == null)
        {
            Debug.LogError("LevelSelectButton: chua gan LevelData cho nut nay!");
            return;
        }

        if (!SaveManager.IsLevelUnlocked(levelData.levelNumber))
        {
            Debug.Log("Level nay dang bi khoa!");
            return;
        }

        LevelSession.SelectedLevel = levelData;
        LevelSession.LoadGameplayScene();
    }
}