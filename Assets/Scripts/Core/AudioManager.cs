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

    [Header("Nhac nen mac dinh (dung khi 1 level khong gan backgroundMusic rieng)")]
    [SerializeField] private AudioClip defaultMusic;

    private AudioSource source;      // Kenh SFX (click, swap, win, lose)
    private AudioSource musicSource; // Kenh nhac nen rieng, phat lap (loop)
    private const string MutedKey = "SoundMuted";
    private const string MusicMutedKey = "MusicMuted";
    private bool isMuted;
    private bool isMusicMuted;
    public bool IsMuted => isMuted;
    public bool IsMusicMuted => isMusicMuted;

    // Nho lai clip nhac dang phat, de tranh restart lai tu dau neu goi PlayMusic
    // nhieu lan voi cung 1 clip (vd moi lan Restart level).
    private AudioClip currentMusicClip;

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
        Instance.defaultMusic = Resources.Load<AudioClip>("Audio/music_default");

        // Kenh SFX (khong loop, dung PlayOneShot).
        Instance.source = go.AddComponent<AudioSource>();
        Instance.source.playOnAwake = false;

        // Kenh nhac nen rieng (loop, chi 1 bai phat tai 1 thoi diem).
        Instance.musicSource = go.AddComponent<AudioSource>();
        Instance.musicSource.playOnAwake = false;
        Instance.musicSource.loop = true;

        Instance.isMuted = PlayerPrefs.GetInt(MutedKey, 0) == 1;
        Instance.isMusicMuted = PlayerPrefs.GetInt(MusicMutedKey, 0) == 1;
        Instance.musicSource.mute = Instance.isMusicMuted;
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

    // Phat nhac nen rieng cho 1 level. Neu clip == null, se fallback ve
    // defaultMusic (neu co gan san). Nhac cu dung ngay lap tuc, nhac moi
    // phat ngay (khong fade), dung theo yeu cau.
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null) return;

        AudioClip clipToPlay = clip != null ? clip : defaultMusic;

        // Khong lam gi neu dung đung bai dang phat (tranh giat/restart khi Restart level).
        if (currentMusicClip == clipToPlay && musicSource.isPlaying)
            return;

        musicSource.Stop();
        currentMusicClip = clipToPlay;

        if (clipToPlay == null)
            return;

        musicSource.clip = clipToPlay;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource == null) return;
        musicSource.Stop();
        currentMusicClip = null;
    }

    public void SetMuted(bool muted)
    {
        isMuted = muted;
        PlayerPrefs.SetInt(MutedKey, muted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetMusicMuted(bool muted)
    {
        isMusicMuted = muted;
        if (musicSource != null)
            musicSource.mute = muted;

        PlayerPrefs.SetInt(MusicMutedKey, muted ? 1 : 0);
        PlayerPrefs.Save();
    }
}