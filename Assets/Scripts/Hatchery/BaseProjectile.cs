using UnityEngine;

/// <summary>
/// 모든 투사체의 공통 로직을 관리하는 기본 클래스
/// 히트 이펙트 생성, 생명 주기, 기본 충돌 처리를 담당
/// </summary>
public abstract class BaseProjectile : MonoBehaviour, IProjectile
{
    [Header("Base Projectile Stats")]
    [Tooltip("투사체가 사라지기까지의 시간 (초)")]
    [SerializeField] protected float lifeTime = 3f;
    [Tooltip("투사체의 속도")]
    [SerializeField] protected float speed = 20f;
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
        rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, lifeTime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // 충돌한 객체가 공격 대상 태그(AttackTargetTag)를 가지고 있는지 확인
        if (hitInfo.CompareTag(AttackTargetTag))
        {
            IDamageable damageableObject = hitInfo.GetComponent<IDamageable>();

            if (damageableObject != null)
            {
                damageableObject.TakeDamage(Damage);
            }

            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
    }
}