using UnityEngine;

/// <summary>
/// 미사일의 이동과 충돌 처리. 초기 가속
/// </summary>
public class Missile : MonoBehaviour, IProjectile
{
    [Header("Missile Stats")]
    [Tooltip("최대 속도")]
    [SerializeField] private float maxSpeed = 20f;
    [Tooltip("미사일이 최대 속도에 도달하는 시간 (초)")]
    [SerializeField] private float accelerationDuration = 0.5f;
    [Tooltip("미사일이 사라지기까지의 시간 (초)")]
    [SerializeField] private float lifeTime = 3f;
    [Tooltip("속도 증가(지수 함수)")]
    [SerializeField] private AnimationCurve accelerationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // TurretFire.cs에서 설정하는 값들
    public int Damage { get; set; }
    public string AttackTargetTag { get; set; }

    private Rigidbody2D rb;
    private float timeElapsed = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 일정 시간 후 미사일 자동 파괴
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        timeElapsed += Time.fixedDeltaTime;

        float currentSpeed;

        if (timeElapsed < accelerationDuration)
        {
            // 가속 구간: AnimationCurve를 사용하여 속도 계산
            float curveTime = timeElapsed / accelerationDuration;
            float speedMultiplier = accelerationCurve.Evaluate(curveTime);
            currentSpeed = maxSpeed * speedMultiplier;
        }
        else
        {
            // 등속 구간: 최대 속도 유지
            currentSpeed = maxSpeed;
        }

        // 계산된 속도를 미사일의 전방(로컬 X축)으로 적용
        rb.linearVelocity = transform.right * currentSpeed;
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
                damageableObject.TakeDamage(Damage);
            }

            // 공격 대상과 충돌했으므로 미사일을 즉시 파괴
            Destroy(gameObject);
        }
    }
}
