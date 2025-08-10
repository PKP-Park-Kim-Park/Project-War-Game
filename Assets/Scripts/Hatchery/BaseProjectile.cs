using UnityEngine;

/// <summary>
/// 모든 투사체의 공통 로직을 관리하는 기본 클래스.
/// 히트 이펙트 생성, 생명 주기, 기본 충돌 처리를 담당합니다.
/// </summary>
public abstract class BaseProjectile : MonoBehaviour, IProjectile
{
    [Header("Base Projectile Stats")]
    [Tooltip("투사체가 사라지기까지의 시간 (초)")]
    [SerializeField] protected float lifeTime = 3f;
    [Tooltip("파괴될 때 생성될 이펙트 프리팹 (폭발, 타격 효과 등)")]
    [SerializeField] private GameObject hitEffectPrefab;

    // IProjectile 인터페이스 구현
    public int Damage { get; set; }
    public string AttackTargetTag { get; set; }

    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        // 일정 시간 후 투사체 자동 파괴
        Destroy(gameObject, lifeTime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // 충돌한 객체가 공격 대상 태그(AttackTargetTag)를 가지고 있는지 확인
        if (hitInfo.CompareTag(AttackTargetTag))
        {
            IDamageable damageableObject = hitInfo.GetComponent<IDamageable>();

            // IDamageable 컴포넌트가 있다면 TakeDamage를 호출
            if (damageableObject != null)
            {
                damageableObject.TakeDamage(Damage);
            }

            // 공격 대상과 충돌했으므로 투사체를 즉시 파괴 (이때 OnDestroy가 호출됩니다)
            Destroy(gameObject);
        }
    }

    // 오브젝트가 파괴될 때 Unity에 의해 자동으로 호출됩니다.
    protected virtual void OnDestroy()
    {
        // hitEffectPrefab이 할당되어 있다면, 현재 위치에 이펙트를 생성합니다.
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
    }
}