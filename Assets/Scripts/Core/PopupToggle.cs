using UnityEngine;

// Dat file nay vao: Assets/Scripts/Core/PopupToggle.cs
//
// Gan script nay vao 1 GameObject rong bat ky trong scene (hoac vao Canvas),
// keo Panel Help / Panel Settings vao field tuong ung.
// Nut "?" goi Open(), nut dong popup ("X" hoac "Dong") goi Close().
// 1 script nay dung duoc cho ca Help lan Settings (2 instance rieng, moi cai 1 panel).

public class PopupToggle : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private void Awake()
    {
        if (panel != null) panel.SetActive(false);
    }

    public void Open()
    {
        if (panel != null) panel.SetActive(true);
    }

    public void Close()
    {
        if (panel != null) panel.SetActive(false);
    }
}