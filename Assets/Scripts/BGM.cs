using UnityEngine;

/// <summary>
/// 게임의 배경 음악(BGM)을 관리하는 싱글톤
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class BGM : MonoBehaviour
{
    public static BGM instance;

    [Header("BGM Settings")]
    [Tooltip("재생할 배경 음악 클립")]
    [SerializeField] private AudioClip bgmClip;

    [Tooltip("음악을 반복 재생할지 여부")]
    [SerializeField] private bool loopMusic = true;

    private AudioSource audioSource;

    private void Awake()
    {
        // --- 싱글톤 패턴 구현 ---
        if (instance == null)
        {
            instance = this;
            // 씬이 변경되어도 이 오브젝트가 파괴되지 않도록 설정
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            // 이미 인스턴스가 존재하면 이 오브젝트는 파괴
            Destroy(gameObject);
            return;
        }

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
    /// 현재 재생 중인 BGM을 정지합니다.
    /// </summary>
    public void StopMusic()
    {
        audioSource.Stop();
    }
}
