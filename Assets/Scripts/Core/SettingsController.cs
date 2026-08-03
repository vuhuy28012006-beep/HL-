using UnityEngine;
using UnityEngine.UI;

// Dat file nay vao: Assets/Scripts/Core/SettingsController.cs
// Gan vao Panel Settings

public class SettingsController : MonoBehaviour
{
    [Header("Am luong (that, da noi voi AudioManager)")]
    [SerializeField] private Slider volumeSlider;

    [Header("Ngon ngu (placeholder, chua co logic that)")]
    [SerializeField] private TMPro.TMP_Dropdown languageDropdown; // de trong option cung duoc, chua xu ly gi

    private void OnEnable()
    {
        // Moi lan mo Settings, hien dung gia tri am luong dang luu
        if (volumeSlider != null && AudioManager.Instance != null)
            volumeSlider.value = AudioManager.Instance.GetVolume();
    }

    // Goi tu Slider > On Value Changed
    public void OnVolumeChanged(float value)
    {
        AudioManager.Instance?.SetVolume(value);
    }

    // Goi tu Dropdown > On Value Changed (hien tai chua lam gi, de danh cho sau)
    public void OnLanguageChanged(int index)
    {
        Debug.Log("Chon ngon ngu index " + index + " - chua co logic doi ngon ngu that");
        // Sau nay: luu PlayerPrefs.SetInt("Language", index), doi text theo bang dich...
    }
}