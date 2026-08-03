using UnityEngine;

// Dat file nay vao: Assets/Scripts/Core/AudioManager.cs
// Tao 1 GameObject rong ten "AudioManager" trong scene Gameplay, gan script nay vao

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Clips (keo file am thanh vao day, co the de trong neu chua co)")]
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip swapSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;

    private AudioSource source;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
    }

    public void PlayClick() => Play(clickSound);
    public void PlaySwap() => Play(swapSound);
    public void PlayWin() => Play(winSound);
    public void PlayLose() => Play(loseSound);

    private void Play(AudioClip clip)
    {
        if (clip == null) return; // chua gan am thanh -> bo qua, khong loi
        source.PlayOneShot(clip);
    }
}