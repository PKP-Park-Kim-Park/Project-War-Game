using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public enum UnitType
{
    Normal,
    Long,
    Tank,
    Archer
}

[System.Serializable]
public class UnitTypeAudioClips
{
    public UnitType unitType;
    public AudioClip[] attackSounds;
}

[System.Serializable]
public class UnitDeathAudioClips
{
    public UnitType unitType;
    public AudioClip[] dieSounds;
}

public class UnitController : MonoBehaviour, IDamageable
{
    [Header("Unit Settings")]
    [SerializeField] private UnitData unitData;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Transform rayShootTransform;
    [SerializeField] private int _upGradeMagnification = 1;

    [Header("Audio")]
    [SerializeField] private UnitTypeAudioClips[] unitAttackSounds;
    [SerializeField] private UnitDeathAudioClips[] unitDieSounds;
    [SerializeField] private float stopDistance = 1f;

    [Tooltip("공격 효과음의 볼륨 (0.0 ~ 1.0)")]
    [Range(0f, 1f)]
    public float hitSoundVolume = 1.0f;

    [Tooltip("죽음 효과음의 볼륨 (0.0 ~ 1.0)")]
    [Range(0f, 1f)]
    public float dieSoundVolume = 1.0f;

    private UnitStat stat = new UnitStat();
    private UnitAnimation unitAnimation;
    private UnitCombat combat = new UnitCombat();

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private Dictionary<UnitType, AudioClip[]> attackSoundMap;
    private Dictionary<UnitType, AudioClip[]> dieSoundMap;

    private Vector3 moveDirection;
    private int currentHealth;
    private bool isMoving = false;
    private bool isAttacking = false;
    private bool isDie = false;
    private bool isSetting = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        unitAnimation = new UnitAnimation(animator);
        stat.Initialize(unitData, gameObject.tag);

        InitializeSoundMap();
    }

    public void SetAgeUnit(int age)
    {
        int upgrade = 1 + _upGradeMagnification * age;

        SetMoveDirection();
        currentHealth = stat.MaxHealth * upgrade;
        combat.Setup(rayShootTransform,
            moveDirection,
            stat.AttackTargetTags,
            stat.AttackDamage * upgrade,
            stat.AttackRange,
            stat.Gold * upgrade,
            stat.Exp * upgrade);

        switch(age)
        {
            case 1:
                spriteRenderer.color = Color.yellow;
                break;
            case 2:
                spriteRenderer.color = Color.red;
                break;
        }

        isSetting = true;

        healthBar.maxValue = currentHealth;
        healthBar.value = currentHealth;
    }

    private void InitializeSoundMap()
    {
        attackSoundMap = new Dictionary<UnitType, AudioClip[]>();
        foreach (var soundData in unitAttackSounds)
        {
            if (soundData.attackSounds != null && soundData.attackSounds.Length > 0)
            {
                attackSoundMap[soundData.unitType] = soundData.attackSounds;
            }
        }

        dieSoundMap = new Dictionary<UnitType, AudioClip[]>();
        foreach (var soundData in unitDieSounds)
        {
            if (soundData.dieSounds != null && soundData.dieSounds.Length > 0)
            {
                dieSoundMap[soundData.unitType] = soundData.dieSounds;
            }
        }
    }

    private void Update()
    {
        if(isSetting == false)
        {
            return;
        }

        if (isDie)
        {
            return;
        }

        if (isMoving)
        {
            transform.position += moveDirection * stat.MoveSpeed * Time.deltaTime;
        }

        CheckState();
    }

    private void SetMoveDirection()
    {
        if (spriteRenderer.flipX)
        {
            moveDirection = Vector3.left;
            rayShootTransform.localPosition = new Vector3(-rayShootTransform.localPosition.x, rayShootTransform.localPosition.y, rayShootTransform.localPosition.z);
        }
        else
        {
            moveDirection = Vector3.right;
        }
    }

    private void CheckState()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayShootTransform.position, moveDirection, stat.AttackRange);
        GameObject closestAttack = null;
        GameObject closestStop = null;

        float minAttackDist = float.MaxValue;
        float minStopDist = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D collider = hits[i].collider;
            if (collider == null) continue;

            float distance = Vector2.Distance(rayShootTransform.position, hits[i].point);
            GameObject target = collider.gameObject;

            // 공격 가능한 태그 목록에 포함되어 있는지 확인합니다.
            if (stat.AttackTargetTags.Contains(target.tag) && distance < minAttackDist)
            {
                minAttackDist = distance;
                closestAttack = target;
            }

            if (target.CompareTag(stat.StopTargetTag) && distance < minStopDist)
            {
                minStopDist = distance;
                closestStop = target;
            }
        }

        if (stat.UnitType == UnitType.Archer || stat.UnitType == UnitType.Long)
        {
            if (closestStop != null && minStopDist <= stopDistance)
            {
                // 아군이 가로막고 있으면 멈춤, 공격은 적이 보일 때만
                SetMove(false);
                SetAttack(closestAttack != null);
            }
            else if (closestAttack != null)
            {
                // 적이 있는데 stopDistance 밖이면 이동하면서 공격
                SetMove(minAttackDist > stopDistance);
                SetAttack(true);
            }
            else
            {
                // 아무것도 없으면 그냥 이동
                SetMove(true);
                SetAttack(false);
            }
        }
        else if (stat.UnitType == UnitType.Normal || stat.UnitType == UnitType.Tank)
        {
            if (closestStop != null && minStopDist <= stopDistance)
            {
                SetMove(false);
                SetAttack(false);
            }
            else if (closestAttack != null)
            {
                SetMove(false);
                SetAttack(true);
            }
            else
            {
                SetMove(true);
                SetAttack(false);
            }
        }
    }

    private void SetMove(bool move)
    {
        isMoving = move;
        unitAnimation.PlayMove(move);
    }

    private void SetAttack(bool attack)
    {
        isAttacking = attack;
        unitAnimation.PlayAttack(attack);
    }

    public void StartEventAttack()
    {
        PlayAttackSound();
        combat.Attack();
    }

    private void PlayAttackSound()
    {
        // 디버깅을 위해 어떤 유닛이 사운드 재생을 시도하는지 로그를 남깁니다.
        Debug.Log($"[{gameObject.name}] (Type: {stat.UnitType}) is trying to play an attack sound.");

        if (attackSoundMap.TryGetValue(stat.UnitType, out AudioClip[] sounds))
        {
            if (sounds != null && sounds.Length > 0)
            {
                // 할당된 사운드 중 하나를 무작위로 선택
                int randomIndex = Random.Range(0, sounds.Length);
                AudioClip clipToPlay = sounds[randomIndex];

                if (clipToPlay != null)
                {
                    audioSource.PlayOneShot(clipToPlay, hitSoundVolume);
                }
            }
            else
            {
                Debug.LogWarning($"-> Sound array for [{stat.UnitType}] is empty or null.", gameObject);
            }
        }
        else
        {
            Debug.LogWarning($"-> No sound entry found in map for UnitType: [{stat.UnitType}]", gameObject);
        }
    }

    private void PlayDieSound()
    {
        if (dieSoundMap.TryGetValue(stat.UnitType, out AudioClip[] sounds))
        {
            if (sounds != null && sounds.Length > 0)
            {
                // 할당된 사운드 중 하나를 무작위로 선택
                int randomIndex = Random.Range(0, sounds.Length);
                AudioClip clipToPlay = sounds[randomIndex];

                if (clipToPlay != null)
                {
                    audioSource.PlayOneShot(clipToPlay, dieSoundVolume);
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.value -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        // 유닛이 죽을 때 효과음을 재생합니다.
        PlayDieSound();
        healthBar.gameObject.SetActive(false);
        unitAnimation.PlayDie();
        GetComponent<Collider2D>().enabled = false;
        isDie = true;

        if (gameObject.CompareTag("Enemy"))
        {
            GoldManager.instance.AddGold(stat.Gold);
            ExpManager.instance.AddExp(stat.Exp);
        }

        yield return unitAnimation.WaitForDeathAnimation();

        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}