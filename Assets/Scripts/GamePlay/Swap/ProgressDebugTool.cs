using UnityEngine;

public class ProgressDebugTool : MonoBehaviour
{
    [ContextMenu("TEST - Reset Level Progress")]
    private void ResetLevelProgress()
    {
        SaveManager.ResetLevelProgress();

        UnityEngine.Debug.Log(
            "RESET XONG: Level 1 mở, " +
            "các level khác khóa, tất cả sao = 0."
        );
    }

    [ContextMenu("TEST - Reset Everything")]
    private void ResetEverything()
    {
        SaveManager.ResetAllProgress();

        UnityEngine.Debug.Log(
            "Đã xóa toàn bộ PlayerPrefs."
        );
    }
}