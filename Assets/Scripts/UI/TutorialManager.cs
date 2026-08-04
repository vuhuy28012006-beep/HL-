using UnityEngine;

// Dat file nay vao: Assets/Scripts/UI/TutorialManager.cs
//
// CACH GAN TRONG UNITY EDITOR:
// 1. Trong Canvas cua scene GamePlay, tao 1 Panel full-man-hinh, dat ten "Panel_Tutorial"
//    (nen la 1 Image mau den mo, Raycast Target = true, de chan click xuyen xuong board o duoi).
//    Panel nay nen nam duoi cung cua Canvas (sibling index cuoi) de luon ve tren het.
// 2. Ben trong Panel_Tutorial, tao 2 panel con:
//      - "Page1_CachSapXep"  : anh/text huong dan cach chon 2 the de doi cho, muc tieu sap
//                              xep theo dung thu tu. Co 1 nut "Tiep theo".
//      - "Page2_CachDungHint": anh/text huong dan nut Hint + popup 2 lua chon
//                              (Goi y 1 cap / Xem thu tu toan bo). Co 1 nut "Bat dau choi".
// 3. Tao 1 GameObject rong (hoac gan thang vao BoardManager/Canvas), them component
//    TutorialManager nay vao, keo:
//      - Tutorial Panel   = Panel_Tutorial
//      - Page 1           = Page1_CachSapXep
//      - Page 2           = Page2_CachDungHint
// 4. Nut "Tiep theo" (o Page1) -> OnClick goi TutorialManager.NextPage
//    Nut "Bat dau choi" (o Page2) -> OnClick goi TutorialManager.CloseTutorial
//    (Neu muon co nut "Bo qua" o Page1 -> goi TutorialManager.CloseTutorial luon cung duoc)
//
// Neu sau nay muon them nut "?" o man GamePlay de nguoi choi xem lai huong dan bat ky luc nao,
// gan nut do OnClick -> TutorialManager.OpenTutorialManually.

public class TutorialManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject page1;
    [SerializeField] private GameObject page2;

    [Header("Chi hien 1 lan duy nhat")]
    [SerializeField] private string prefsKey = "Tutorial_GamePlay_Seen";

    private void Start()
    {
        bool daXem = PlayerPrefs.GetInt(prefsKey, 0) == 1;

        if (!daXem)
            ShowFromStart();
        else
            HideTutorial();
    }

    // Goi ham nay tu nut "?" (neu co) de xem lai huong dan bat ky luc nao,
    // khong quan tam da xem hay chua.
    public void OpenTutorialManually()
    {
        ShowFromStart();
    }

    private void ShowFromStart()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
        if (page1 != null) page1.SetActive(true);
        if (page2 != null) page2.SetActive(false);
    }

    public void NextPage()
    {
        if (page1 != null) page1.SetActive(false);
        if (page2 != null) page2.SetActive(true);
    }

    public void PrevPage()
    {
        if (page2 != null) page2.SetActive(false);
        if (page1 != null) page1.SetActive(true);
    }

    public void CloseTutorial()
    {
        HideTutorial();

        PlayerPrefs.SetInt(prefsKey, 1);
        PlayerPrefs.Save();
    }

    private void HideTutorial()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
    }
}
