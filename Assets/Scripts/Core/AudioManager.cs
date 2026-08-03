using UnityEngine;

// Ghi de vao: Assets/Scripts/Core/AudioManager.cs

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Clips (keo file am thanh vao day, co the de trong neu chua co)")]
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip swapSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;

    private AudioSource source;
    private const string VolumeKey = "SoundVolume";

    private void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject); // giu AudioManager song xuyen suot moi scene
    }
    else
    {
        Destroy(gameObject);
        return;
    }

    source = gameObject.AddComponent<AudioSource>();
    source.playOnAwake = false;
    source.volume = PlayerPrefs.GetFloat(VolumeKey, 1f);
}

    public void PlayClick() => Play(clickSound);
    public void PlaySwap() => Play(swapSound);
    public void PlayWin() => Play(winSound);
    public void PlayLose() => Play(loseSound);

    private void Play(AudioClip clip)
    {
        if (clip == null) return;
        source.PlayOneShot(clip);
    }

    // Goi tu Slider trong Settings (0 = tat, 1 = full)
    public void SetVolume(float value)
    {
        source.volume = value;
        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return PlayerPrefs.GetFloat(VolumeKey, 1f);
    }
}