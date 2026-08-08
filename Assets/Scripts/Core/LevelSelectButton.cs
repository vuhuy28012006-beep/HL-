using UnityEngine;
using UnityEngine.UI;

// Ghi de vao: Assets/Scripts/Core/LevelSelectButton.cs

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] private LevelData levelData; // co the gan tay (cach cu) hoac de trong, cho Setup() gan luc chay

    [Header("Hien thi trang thai (khong bat buoc)")]
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private TMPro.TMP_Text starsText;
    [SerializeField] private TMPro.TMP_Text levelNumberText; // hien so thu tu man, dung khi sinh dong

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        // Neu da gan san levelData qua Inspector (cach cu, nut tao tay) -> chay luon
        if (levelData != null)
            RefreshState();
    }

    // Goi tu LevelListPopulator khi tao nut luc chay (cach moi, cho danh sach dong)
    public void Setup(LevelData data)
    {
        levelData = data;

        if (levelNumberText != null)
            levelNumberText.text = data.levelNumber.ToString();

        RefreshState();
    }

    private void RefreshState()
    {
        if (levelData == null) return;
        if (button == null) button = GetComponent<Button>();

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

    // Goi tu Button > On Click ()
    public void LoadLevel()
    {
        if (levelData == null)
        {
            Debug.LogError("LevelSelectButton: chua co LevelData!");
            return;
        }

        if (!SaveManager.IsLevelUnlocked(levelData.levelNumber))
        {
            Debug.Log("Level nay dang bi khoa!");
            return;
        }

        LevelSession.SelectedLevel = levelData;
        LevelSession.LoadGameplayScene(levelData.gameplaySceneName);
    }
}