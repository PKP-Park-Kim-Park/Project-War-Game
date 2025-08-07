using UnityEngine;

/// <summary>
/// 총알의 이동과 충돌 처리
/// </summary>
public class Bullet : MonoBehaviour
{
    [Header("Bullet Stats")]
    [SerializeField] private float speed = 20f;
    [Tooltip("총알이 사라지기까지의 시간 (초)")]
    [SerializeField] private float lifeTime = 3f;
    [Tooltip("총알이 공격할 대상의 태그")]
    [SerializeField] private string _attackTargetTag = "Enemy";

    // TurretFire.cs에서 설정하는 값들
    public int damage;
    public string AttackTargetTag;

    private Rigidbody2D rb;
    // private bool hasHit = false; // 중복 충돌 방지 플래그

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 총알을 생성된 방향(로컬 X축)으로 발사
        rb.linearVelocity = transform.right * speed;

        // 일정 시간 후 총알 자동 파괴
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // 충돌한 객체가 공격 대상 태그(AttackTargetTag)를 가지고 있는지 확인
        if (hitInfo.CompareTag(AttackTargetTag))
        {
            IDamageable damageableObject = hitInfo.GetComponent<IDamageable>();

            // IDamageable 컴포넌트가 있다면 TakeDamage를 호출
            if (damageableObject != null)
            {
                damageableObject.TakeDamage(damage);
            }

            // 공격 대상과 충돌했으므로 총알을 즉시 파괴
            Destroy(gameObject);
        }
    }
}
