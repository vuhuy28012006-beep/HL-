using TMPro;
using UnityEngine;

// Dat vao: Assets/Scripts/Core/LevelListPopulator.cs
// Gan vao 1 GameObject rong trong scene LevelSelect (vd ten "LevelListPopulator")

public class LevelListPopulator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform levelButtonContainer; // object co Grid Layout Group
    [SerializeField] private GameObject levelButtonPrefab;   // prefab co component LevelSelectButton

    [Header("UI hien thi thong tin chuong (khong bat buoc)")]
    [SerializeField] private TMP_Text chapterNameText;
    [SerializeField] private TMP_Text chapterDescriptionText;

    private void Start()
    {
        ChapterData chapter = ChapterSession.SelectedChapter;

        if (chapter == null)
        {
            Debug.LogError("LevelListPopulator: chua co Chapter nao duoc chon! " +
                "Vao scene Map bam 1 chuong truoc, dung Play truc tiep trong scene nay.");
            return;
        }

        if (chapterNameText != null) chapterNameText.text = chapter.chapterName;
        if (chapterDescriptionText != null) chapterDescriptionText.text = chapter.description;

        // Xoa nut cu (neu co) truoc khi sinh moi, tranh nhan doi khi load lai scene
        for (int i = levelButtonContainer.childCount - 1; i >= 0; i--)
            Destroy(levelButtonContainer.GetChild(i).gameObject);

        foreach (LevelData level in chapter.levels)
        {
            GameObject go = Instantiate(levelButtonPrefab, levelButtonContainer);
            LevelSelectButton btn = go.GetComponent<LevelSelectButton>();
            btn.Setup(level);
        }
    }
}
