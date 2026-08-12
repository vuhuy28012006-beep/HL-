using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] private LevelData levelData;

    [Header("Hien thi trang thai")]
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private TMP_Text starsText;
    [SerializeField] private TMP_Text levelNumberText;

    [Header("Ba ngoi sao tren Level")]
    [SerializeField] private Image[] starImages;
    [SerializeField] private Sprite filledStar;
    [SerializeField] private Sprite emptyStar;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        // Dung cho nut duoc gan LevelData san trong Inspector
        if (levelData != null)
            RefreshState();
    }

    // Duoc LevelListPopulator goi khi tao nut
    public void Setup(LevelData data)
    {
        levelData = data;

        if (levelNumberText != null)
            levelNumberText.text = data.levelNumber.ToString();

        RefreshState();
    }

    private void RefreshState()
    {
        if (levelData == null)
            return;

        if (button == null)
            button = GetComponent<Button>();

        bool unlocked =
            SaveManager.IsLevelUnlocked(levelData.levelNumber);

        if (button != null)
            button.interactable = unlocked;

        if (lockIcon != null)
            lockIcon.SetActive(!unlocked);

        int earnedStars =
            SaveManager.GetStars(levelData.levelNumber);

        // Neu van muon hien chu 2/3
        if (starsText != null)
            starsText.text = unlocked ? earnedStars + "/3" : "";

        RefreshStarImages(earnedStars);
    }

    private void RefreshStarImages(int earnedStars)
    {
        if (starImages == null || starImages.Length == 0)
            return;

        if (filledStar == null || emptyStar == null)
        {
            Debug.LogWarning(
                "LevelSelectButton: Chua gan Filled Star hoac Empty Star.",
                this
            );

            return;
        }

        earnedStars = Mathf.Clamp(earnedStars, 0, 3);

        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] == null)
                continue;

            starImages[i].sprite =
                i < earnedStars ? filledStar : emptyStar;

            starImages[i].color = Color.white;
            starImages[i].enabled = true;
        }
    }

    public void LoadLevel()
    {
        if (levelData == null)
        {
            Debug.LogError(
                "LevelSelectButton: chua co LevelData!"
            );

            return;
        }

        if (!SaveManager.IsLevelUnlocked(levelData.levelNumber))
        {
            Debug.Log("Level nay dang bi khoa!");
            return;
        }

        LevelSession.SelectedLevel = levelData;
        LevelSession.LoadGameplayScene(
            levelData.gameplaySceneName
        );
    }
}