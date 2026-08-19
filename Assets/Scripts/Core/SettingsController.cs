using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    [Header("Âm thanh")]
    [SerializeField] private Toggle soundToggle;

    [Header("Ngôn ngữ")]
    [SerializeField] private TMP_Dropdown languageDropdown;

    private void OnEnable()
    {
        RefreshSoundToggle();
    }

    private void RefreshSoundToggle()
    {
        if (soundToggle == null)
        {
            Debug.LogWarning(
                "SettingsController chưa được gán Sound Toggle.",
                this
            );

            return;
        }

        if (AudioManager.Instance == null)
            return;

        // Cập nhật dấu tích mà không gọi lại On Value Changed.
        soundToggle.SetIsOnWithoutNotify(
            !AudioManager.Instance.IsMuted
        );
    }

    public void OnSoundToggleChanged(bool isOn)
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning(
                "Không tìm thấy AudioManager."
            );

            return;
        }

        // isOn = true: bật tiếng.
        // isOn = false: tắt tiếng.
        AudioManager.Instance.SetMuted(!isOn);
    }

    public void OnLanguageChanged(int index)
    {
        Debug.Log(
            "Chọn ngôn ngữ index " +
            index +
            " - chưa có logic đổi ngôn ngữ."
        );
    }
}