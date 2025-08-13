using UnityEngine;

/// <summary>
/// 게임의 배경 음악(BGM)을 관리
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class BGM : MonoBehaviour
{
    [Header("BGM Settings")]
    [Tooltip("재생할 배경 음악 클립")]
    [SerializeField] private AudioClip bgmClip;

    [Tooltip("음악을 반복 재생할지 여부")]
    [SerializeField] private bool loopMusic = true;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // 게임이 시작될 때 BGM 재생
        PlayMusic();
    }

    /// <summary>
    /// BGM을 재생합니다.
    /// </summary>
    public void PlayMusic()
    {
        if (bgmClip == null)
        {
            Debug.LogWarning("BGM 클립이 할당되지 않았습니다.", this);
            return;
        }

        audioSource.clip = bgmClip;
        audioSource.loop = loopMusic;
        audioSource.Play();

        Debug.Log($"BGM 재생 시작: {bgmClip.name}", this);
    }

    /// <summary>
    /// 현재 재생 중인 BGM을 정지
    /// </summary>
    public void StopMusic()
    {
        audioSource.Stop();
    }
}
