using UnityEngine;

// Ghi de vao: Assets/Scripts/Core/AudioManager.cs
//
// Thay doi quan trong: dung [RuntimeInitializeOnLoadMethod] de TU DONG tao ra
// AudioManager ngay khi game chay, bat ke ban bam Play tu scene nao (MainMenu,
// LevelSelect hay thang vao Gameplay de test nhanh). Khong can dat san object
// AudioManager trong tung scene nua.

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Clips - se tu tim trong Resources/Audio (xem huong dan wiring)")]
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip swapSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;

    private AudioSource source;
    private const string MutedKey = "SoundMuted";
    private bool isMuted;
    public bool IsMuted => isMuted;

    // Tu dong chay 1 lan duy nhat truoc ca scene dau tien duoc load
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        if (Instance != null) return;

        GameObject go = new GameObject("AudioManager");
        Instance = go.AddComponent<AudioManager>();
        DontDestroyOnLoad(go);

        // Tu load am thanh tu Resources (xem huong dan wiring ben duoi)
        Instance.clickSound = Resources.Load<AudioClip>("Audio/click");
        Instance.swapSound = Resources.Load<AudioClip>("Audio/swap");
        Instance.winSound = Resources.Load<AudioClip>("Audio/win");
        Instance.loseSound = Resources.Load<AudioClip>("Audio/lose");

        Instance.source = go.AddComponent<AudioSource>();
        Instance.source.playOnAwake = false;
        Instance.isMuted = PlayerPrefs.GetInt(MutedKey, 0) == 1;
    }

    public void PlayClick() => Play(clickSound);
    public void PlaySwap() => Play(swapSound);
    public void PlayWin() => Play(winSound);
    public void PlayLose() => Play(loseSound);

    private void Play(AudioClip clip)
    {
        if (isMuted || clip == null || source == null) return;
        source.PlayOneShot(clip);
    }

    public void SetMuted(bool muted)
    {
        isMuted = muted;
        PlayerPrefs.SetInt(MutedKey, muted ? 1 : 0);
        PlayerPrefs.Save();
    }
}