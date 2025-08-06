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
    [SerializeField] private string _attackTargetTag;

    // TurretFire.cs에서 설정하는 데미지값
    public int damage;

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
        // 이제 유닛이 총알 맞을 때 처리(데미지 등)
        //

        Debug.Log("총알 맞음!!!!!");
    }
}
