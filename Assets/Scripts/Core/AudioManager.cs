using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Clips trong Resources/Audio")]
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip swapSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;

    [Header("Nhạc nền mặc định")]
    [SerializeField] private AudioClip defaultMusic;

    private AudioSource source;
    private AudioSource musicSource;

    private const string MutedKey = "SoundMuted";
    private const string MusicMutedKey = "MusicMuted";

    private bool isMuted;
    private bool isMusicMuted;

    public bool IsMuted => isMuted;
    public bool IsMusicMuted => isMusicMuted;

    private AudioClip currentMusicClip;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticData()
    {
        Instance = null;
    }

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        if (Instance != null)
            return;

        GameObject audioObject = new GameObject("AudioManager");
        audioObject.AddComponent<AudioManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Kênh hiệu ứng âm thanh
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f;
        source.ignoreListenerPause = false;

        // Kênh nhạc nền riêng
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        musicSource.ignoreListenerPause = false;

        LoadAudioClips();

        isMuted = PlayerPrefs.GetInt(MutedKey, 0) == 1;
        isMusicMuted = PlayerPrefs.GetInt(MusicMutedKey, 0) == 1;

        musicSource.mute = isMusicMuted;
        ApplyMuteState();
    }

    private void LoadAudioClips()
    {
        if (clickSound == null)
            clickSound = Resources.Load<AudioClip>("Audio/click");

        if (swapSound == null)
            swapSound = Resources.Load<AudioClip>("Audio/swap");

        if (winSound == null)
            winSound = Resources.Load<AudioClip>("Audio/win");

        if (loseSound == null)
            loseSound = Resources.Load<AudioClip>("Audio/lose");

        if (defaultMusic == null)
            defaultMusic =
                Resources.Load<AudioClip>("Audio/music_default");
    }

    private void ApplyMuteState()
    {
        // Tắt cả AudioManager và nhạc từ MusicManager của các Scene.
        AudioListener.pause = isMuted;
    }

    public void PlayClick()
    {
        Play(clickSound);
    }

    public void PlaySwap()
    {
        Play(swapSound);
    }

    public void PlayWin()
    {
        Play(winSound);
    }

    public void PlayLose()
    {
        Play(loseSound);
    }

    private void Play(AudioClip clip)
    {
        if (isMuted || clip == null || source == null)
            return;

        source.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null)
            return;

        AudioClip clipToPlay =
            clip != null ? clip : defaultMusic;

        if (currentMusicClip == clipToPlay &&
            musicSource.isPlaying)
        {
            return;
        }

        musicSource.Stop();
        currentMusicClip = clipToPlay;

        if (clipToPlay == null)
            return;

        musicSource.clip = clipToPlay;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource == null)
            return;

        musicSource.Stop();
        musicSource.clip = null;
        currentMusicClip = null;
    }

    public void SetMuted(bool muted)
    {
        isMuted = muted;
        ApplyMuteState();

        PlayerPrefs.SetInt(MutedKey, muted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetMusicMuted(bool muted)
    {
        isMusicMuted = muted;

        if (musicSource != null)
            musicSource.mute = muted;

        PlayerPrefs.SetInt(
            MusicMutedKey,
            muted ? 1 : 0
        );

        PlayerPrefs.Save();
    }
}