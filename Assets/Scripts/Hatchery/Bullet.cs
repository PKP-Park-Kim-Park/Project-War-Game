using UnityEngine;

/// <summary>
/// 총알의 이동과 충돌 처리. BaseProjectile을 상속받아 공통 로직을 재사용합니다.
/// </summary>
public class Bullet : BaseProjectile
{
    [Header("Bullet Stats")]
    [SerializeField] private float speed = 20f;

    protected override void Start()
    {
        base.Start(); // lifeTime 후 자동 파괴 및 이펙트 생성을 위해 부모의 Start() 호출

        // 총알을 생성된 방향(로컬 X축)으로 발사
        rb.linearVelocity = transform.right * speed;
    }
}
