using UnityEngine;

/// <summary>
/// 미사일의 이동과 충돌 처리. 초기 가속
/// </summary>
public class Missile : BaseProjectile
{
    [Header("Missile Stats")]
    [Tooltip("미사일이 최대 속도에 도달하는 시간 (초)")]
    [SerializeField] private float accelerationDuration = 0.5f;
    [Tooltip("속도 증가(지수 함수)")]
    [SerializeField] private AnimationCurve accelerationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float timeElapsed = 0f;

    protected override void Start()
    {
        // Missile은 FixedUpdate에서 직접 속도를 제어하므로,
        // BaseProjectile의 Start()에 있는 기본 속도 설정을 호출하지 않습니다.
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
            currentSpeed = speed * speedMultiplier;
        }
        else
        {
            // 등속 구간: 최대 속도 유지
            currentSpeed = speed;
        }

        // 계산된 속도를 미사일의 전방(로컬 X축)으로 적용
        rb.linearVelocity = transform.right * currentSpeed;
    }
}
