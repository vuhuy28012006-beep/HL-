using UnityEngine;
using UnityEngine.UI;

// Ghi de vao: Assets/Scripts/Core/SettingsController.cs
// Gan vao Panel Settings

public class SettingsController : MonoBehaviour
{
    [Header("Am thanh (that, noi voi AudioManager)")]
    [SerializeField] private Toggle soundToggle; // tick = BAT am thanh, bo tick = TAT

    [Header("Ngon ngu (placeholder, chua co logic that)")]
    [SerializeField] private TMPro.TMP_Dropdown languageDropdown;

    private void OnEnable()
    {
        // Moi lan mo Settings, hien dung trang thai dang bat/tat
        if (soundToggle != null && AudioManager.Instance != null)
            soundToggle.isOn = !AudioManager.Instance.IsMuted;
    }

    // Goi tu Toggle > On Value Changed
    // isOn = true nghia la BAT am thanh -> muted = false
    public void OnSoundToggleChanged(bool isOn)
    {
        AudioManager.Instance?.SetMuted(!isOn);
    }

    // Goi tu Dropdown > On Value Changed (chua lam gi, de danh cho sau)
    public void OnLanguageChanged(int index)
    {
        Debug.Log("Chon ngon ngu index " + index + " - chua co logic doi ngon ngu that");
    }
}