using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial UI")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private Image tutorialImage;
    [SerializeField] private TMP_Text continueButtonText;

    [Header("Huong dan chung")]
    [SerializeField] private string generalTitle = "HƯỚNG DẪN CHUNG";

    [TextArea(3, 10)]
    [SerializeField] private string generalText =
        "Chọn các thẻ để đổi vị trí.\n" +
        "Sắp xếp các sự kiện theo đúng thứ tự thời gian.";

    [SerializeField] private Sprite generalImage;

    [Header("Luu trang thai")]
    [SerializeField] private string generalTutorialPrefsKey =
        "General_Tutorial_Seen";

    private LevelData currentLevel;

    // True khi đang hiển thị hướng dẫn chung.
    private bool isShowingGeneralTutorial;

    /// <summary>
    /// Gọi khi bắt đầu một màn chơi.
    /// </summary>
    public void StartTutorial(LevelData level)
    {
        currentLevel = level;

        bool hasSeenGeneralTutorial =
            PlayerPrefs.GetInt(generalTutorialPrefsKey, 0) == 1;

        if (!hasSeenGeneralTutorial)
        {
            ShowGeneralTutorial();
            return;
        }

        ShowCurrentLevelTutorial();
    }

    private void ShowGeneralTutorial()
    {
        isShowingGeneralTutorial = true;

        SetTutorialContent(
            generalTitle,
            generalText,
            generalImage,
            "TIẾP TỤC"
        );

        ShowPanel();
    }

    private void ShowCurrentLevelTutorial()
    {
        isShowingGeneralTutorial = false;

        if (currentLevel == null || !currentLevel.showLevelTutorial)
        {
            HideTutorial();
            return;
        }

        SetTutorialContent(
            currentLevel.tutorialTitle,
            currentLevel.tutorialText,
            currentLevel.tutorialImage,
            "BẮT ĐẦU"
        );

        ShowPanel();
    }

    /// <summary>
    /// Gắn hàm này vào nút BtnContinue.
    /// </summary>
    public void ContinueTutorial()
    {
        if (isShowingGeneralTutorial)
        {
            PlayerPrefs.SetInt(generalTutorialPrefsKey, 1);
            PlayerPrefs.Save();

            ShowCurrentLevelTutorial();
            return;
        }

        HideTutorial();
    }

    /// <summary>
    /// Dùng cho nút chấm than để xem lại hướng dẫn màn hiện tại.
    /// </summary>
    public void OpenTutorialManually()
    {
        if (currentLevel == null)
        {
            Debug.LogWarning("Chua co LevelData de hien thi huong dan.");
            return;
        }

        isShowingGeneralTutorial = false;

        SetTutorialContent(
            currentLevel.tutorialTitle,
            currentLevel.tutorialText,
            currentLevel.tutorialImage,
            "ĐÓNG"
        );

        ShowPanel();
    }

    public void CloseTutorial()
    {
        HideTutorial();
    }

    private void SetTutorialContent(
        string title,
        string content,
        Sprite image,
        string buttonText)
    {
        if (titleText != null)
            titleText.text = title;

        if (tutorialText != null)
            tutorialText.text = content;

        if (continueButtonText != null)
            continueButtonText.text = buttonText;

        if (tutorialImage != null)
        {
            bool hasImage = image != null;

            tutorialImage.gameObject.SetActive(hasImage);

            if (hasImage)
            {
                tutorialImage.sprite = image;
                tutorialImage.preserveAspect = true;
            }
        }
    }

    private void ShowPanel()
    {
        Time.timeScale = 0f;    // Dừng đếm giờ khi đọc hướng dẫn

        if (tutorialPanel != null)
            tutorialPanel.SetActive(true);
    }

    private void HideTutorial()
    {
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    // Dùng để thử lại hướng dẫn chung trong Unity.
    [ContextMenu("Reset General Tutorial")]
    private void ResetGeneralTutorial()
    {
        PlayerPrefs.DeleteKey(generalTutorialPrefsKey);
        PlayerPrefs.Save();

        Debug.Log("Da reset huong dan chung.");
    }
    private void OnDestroy(){   Time.timeScale = 1f;       } 
}