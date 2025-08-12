using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UltSkill : MonoBehaviour
{
    public GameObject skillEffectPrefab; // 이펙트 게임오브젝트를 여기에 드래그해서 넣어주세요.
    public int skillDamage = 50;
    public float skillCooldown = 30f;

    [Header("Effects & Sounds")]
    public Image whiteImage; // 화면 번쩍임 효과에 사용할 하얀색 이미지
    public Image darkenOverlayImage; // 화면을 어둡게 할 검은색 이미지
    public float flashDuration = 0.3f;
    public AudioClip skillStartSound; // 스킬 시전 시 사운드
    public AudioClip skillCooldownSound; // 쿨타임일 때 버튼 클릭 시 사운드

    [Tooltip("스킬 효과음의 볼륨 (0.0 ~ 1.0)")]
    [Range(0f, 1f)]
    public float skillSoundVolume = 1.0f;
    [Tooltip("스킬 쿨다운 효과음의 볼륨 (0.0 ~ 1.0)")]
    [Range(0f, 1f)]
    public float cooldownSoundVolume = 1.0f;

    private AudioSource audioSource;
    private float nextAvailableTime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (whiteImage != null)
        {
            // 시작 시에는 이미지를 비활성화하고 투명도를 0으로 설정
            whiteImage.gameObject.SetActive(false);
            Color initialColor = whiteImage.color;
            initialColor.a = 0f;
            whiteImage.color = initialColor;
        }
        else
        {
            Debug.LogError("White Image가 연결되지 않았습니다!");
        }

        if (darkenOverlayImage != null)
        {
            darkenOverlayImage.gameObject.SetActive(false);
            Color initialColor = darkenOverlayImage.color;
            initialColor.a = 0f;
            darkenOverlayImage.color = initialColor;
        }
        else
        {
            Debug.LogError("Darken Overlay Image가 연결되지 않았습니다!");
        }
    }

    public void OnUltSkillButton()
    {
        // 쿨다운 시간이 지났을 때만 코루틴을 시작
        if (Time.time >= nextAvailableTime)
        {
            StartCoroutine(UseSkillCoroutine());
        }
        else
        {
            Debug.Log("궁극기 쿨타임입니다.");
            // 쿨타임일 때 사운드 재생
            if (audioSource != null && skillCooldownSound != null)
            {
                audioSource.PlayOneShot(skillCooldownSound, cooldownSoundVolume);
            }
        }
    }

    // 코루틴 함수는 IEnumerator 타입을 반환해야 합니다.
    IEnumerator UseSkillCoroutine()
    {
        // 쿨다운 시간 설정
        nextAvailableTime = Time.time + skillCooldown;

        // 스킬 시전 사운드 재생
        if (audioSource != null && skillStartSound != null)
        {
            audioSource.PlayOneShot(skillStartSound, skillSoundVolume);
        }

        // --- 화면 어두워지는 효과 (3초간) ---
        if (darkenOverlayImage != null)
        {
            darkenOverlayImage.gameObject.SetActive(true);
            float darkenDuration = 3.0f;
            float timer = 0f;
            Color overlayColor = darkenOverlayImage.color;
            float targetAlpha = 0.95f; // 95% 어둡게

            while (timer < darkenDuration)
            {
                timer += Time.deltaTime;
                overlayColor.a = Mathf.Lerp(0f, targetAlpha, timer / darkenDuration);
                darkenOverlayImage.color = overlayColor;
                yield return null;
            }
            overlayColor.a = targetAlpha;
            darkenOverlayImage.color = overlayColor;
        }
        else
        {
            yield return new WaitForSeconds(3.0f);
        }

        // --- 3초 후, 번쩍이는 효과와 스킬 발동 ---
        if (darkenOverlayImage != null)
        {
            darkenOverlayImage.gameObject.SetActive(false);
        }

        // 스킬 이펙트 생성 (번쩍임과 동시에)
        if (skillEffectPrefab != null)
        {
            GameObject effectInstance = Instantiate(skillEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effectInstance, 2f); // 이펙트 지속시간은 적절히 조절
        }

        // 하얀 번쩍 효과 (즉시 번쩍 -> 페이드 아웃)
        if (whiteImage != null)
        {
            whiteImage.gameObject.SetActive(true);
            Color flashColor = whiteImage.color;

            flashColor.a = 1f; // 즉시 하얗게 만듦
            whiteImage.color = flashColor;

            // 4. 데미지 적용 (가장 밝을 때)
            DealDamageToEnemies();

            // 하얀 화면을 1초간 유지합니다.
            yield return new WaitForSeconds(1f);

            // Fade Out
            float fadeOutDuration = flashDuration; // 전체 번쩍임 시간 사용
            float timer = 0f;
            while (timer < fadeOutDuration)
            {
                timer += Time.deltaTime;
                flashColor.a = Mathf.Lerp(1f, 0f, timer / fadeOutDuration);
                whiteImage.color = flashColor;
                yield return null;
            }
            flashColor.a = 0f;
            whiteImage.color = flashColor;
            whiteImage.gameObject.SetActive(false);
        }
        else
        {
            // 번쩍이는 이미지가 없으면 그냥 데미지 적용
            DealDamageToEnemies();
        }
    }

    void DealDamageToEnemies()
    {
        // 1. 플레이어 주변에 있는 적들을 찾기 위한 범위를 설정합니다.
        // 여기서는 플레이어 위치를 중심으로 반경 5f(미터) 안에 있는 모든 Collider들을 찾습니다.
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 50f);

        // 2. 찾아낸 Collider들을 하나씩 검사합니다.
        foreach (var hitCollider in hitColliders)
        {
            // 3. 충돌한 오브젝트의 태그가 "Enemy"인지 확인합니다.
            if (hitCollider.CompareTag("Enemy"))
            {
                // 4. "Enemy" 태그를 가진 오브젝트의 Health 스크립트를 가져옵니다.
                // (적 캐릭터에 Health 스크립트가 있다고 가정합니다.)
                UnitController enemyHealth = hitCollider.GetComponent<UnitController>();

                // 5. 만약 Health 스크립트가 존재하면 데미지를 적용합니다.
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(skillDamage);
                    Debug.Log(hitCollider.name + "가 " + skillDamage + "만큼의 데미지를 입었습니다!");
                }
            }
        }
    }
}