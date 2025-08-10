using UnityEngine;

/// <summary>
/// 미사일의 이동과 충돌 처리. 초기 가속
/// </summary>
public class Missile : BaseProjectile
{
    [Header("Missile Stats")]
    [Tooltip("최대 속도")]
    [SerializeField] private float maxSpeed = 20f;
    [Tooltip("미사일이 최대 속도에 도달하는 시간 (초)")]
    [SerializeField] private float accelerationDuration = 0.5f;
    [Tooltip("속도 증가(지수 함수)")]
    [SerializeField] private AnimationCurve accelerationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float timeElapsed = 0f;

    protected override void Start()
    {
        base.Start(); // lifeTime 후 자동 파괴 및 이펙트 생성을 위해 부모의 Start() 호출
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
}
