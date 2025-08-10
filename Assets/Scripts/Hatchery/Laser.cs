using UnityEngine;

public class Laser : BaseProjectile
{
    [Header("Laser Stats")]
    [SerializeField] private float speed = 20f;

    protected override void Start()
    {
        base.Start(); // lifeTime 후 자동 파괴 및 이펙트 생성을 위해 부모의 Start() 호출

        // 총알을 생성된 방향(로컬 X축)으로 발사
        rb.linearVelocity = transform.right * speed;
    }
}
