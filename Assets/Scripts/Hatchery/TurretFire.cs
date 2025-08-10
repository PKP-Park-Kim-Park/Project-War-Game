using UnityEngine;

/// <summary>
/// 터렛의 기본 방향 좌우 결정
/// </summary>
public enum FacingDirection
{
    Right,
    Left
}

public class TurretFire : MonoBehaviour
{
    [Header("Turret Attributes")]
    [Tooltip("터렛의 기본 방향")]
    public FacingDirection defaultFacingDirection = FacingDirection.Right;
    [Tooltip("터렛의 공격 사거리")]
    public float attackRange = 15f;
    [Tooltip("터렛의 회전 속도")]
    public float turnSpeed = 10f;
    [Tooltip("터렛의 발사 속도 (초당 발사)")]
    public float fireRate = 1f;
    [Tooltip("첫 타겟 감지 후 발사까지의 딜레이")]
    public float initialFireDelay = 0.5f;
    [Tooltip("발사체가 가하는 데미지")]
    public int projectileDamage = 10;
    private float fireCountdown = 0f;

    [Header("Required Setup")]
    [Tooltip("추적할 적의 태그")]
    public string enemyTag = "Enemy";
    [Tooltip("회전할 터렛 파츠 (총구 부분)")]
    public Transform partToRotate;
    [Tooltip("총알이 발사될 위치 목록. 여러 개를 설정하면 번갈아가며 발사됩니다.")]
    public System.Collections.Generic.List<Transform> firePoints;
    [Tooltip("발사할 발사체(총알, 미사일 등) 프리팹")]
    public GameObject projectilePrefab;

    private Transform target;
    private Quaternion initialRotation;
    private int nextFirePointIndex = 0; // 다음 발사할 총구의 인덱스

    void Start()
    {
        SetInitialRotation();

        // 0.5초마다 가장 가까운 적을 찾도록 설정
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
    }

    void SetInitialRotation()
    {
        if (partToRotate != null)
        {
            // 터렛의 기본 방향 설정
            if (defaultFacingDirection == FacingDirection.Left)
            {
                partToRotate.rotation = Quaternion.Euler(0f, 0f, 180f);
            }
            // 설정된 회전값을 초기 회전값으로 저장합니다.
            initialRotation = partToRotate.rotation;
        }
        else
        {
            Debug.LogError("'Part To Rotate'를 설정하시오..", this);
            initialRotation = transform.rotation; // Fallback
        }
    }

    // 가장 가까운 적을 찾아 타겟으로 설정
    void UpdateTarget()
    {
        Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(transform.position, attackRange);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (Collider2D collider in collidersInRange)
        {
            if (collider.CompareTag(enemyTag))
            {
                float distanceToEnemy = Vector2.Distance(transform.position, collider.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = collider.gameObject;
                }
            }
        }

        if (nearestEnemy != null)
        {
            // 새로운 타겟을 감지했는지 확인 (이전 타겟이 없었거나 다른 타겟일 경우)
            if (target != nearestEnemy.transform)
            {
                // 새로운 타겟을 조준하기 시작할 때 발사 카운트다운을 초기 딜레이로 설정
                fireCountdown = initialFireDelay;
            }
            target = nearestEnemy.transform;
        }
        else
        {
            // 사거리 내에 적이 없으면 타겟을 초기화
            target = null;
        }
    }

    void Update()
    {
        // 타겟이 없으면 기본 각도로 복귀
        if (target == null)
        {
            if (partToRotate != null && partToRotate.rotation != initialRotation)
            {
                partToRotate.rotation = Quaternion.Slerp(partToRotate.rotation, initialRotation, Time.deltaTime * turnSpeed);
            }
            return;
        }

        // 개선: 사거리 벗어난 것 확인해서 사격 중지
        if (Vector2.Distance(transform.position, target.position) > attackRange)
        {
            // 타겟 없음으로 초기화하고 함수를 종료
            target = null;
            return;
        }

        // 타겟을 향해 회전
        LockOnTarget();
        // 발사 로직
        if (fireCountdown <= 0f)
        {
            FireProjectile();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }
    // 타겟을 향해 총구를 회전시키는 함수
    void LockOnTarget()
    {
        // 타겟 방향 벡터 계산
        Vector2 dir = target.position - partToRotate.position;
        // Z축을 중심 회전
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion lookRotation = Quaternion.Euler(0f, 0f, angle);
        // 부드럽게 회전
        partToRotate.rotation = Quaternion.Slerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    // 투사체 발사 함수
    void FireProjectile()
    {
        if (projectilePrefab == null || firePoints == null || firePoints.Count == 0)
        {
            Debug.LogError("'Projectile Prefab' 또는 'Fire Points'가 설정되지 않았습니다.", this);
            return;
        }

        // 현재 발사할 총구를 가져옵니다.
        Transform firePoint = firePoints[nextFirePointIndex];

        if (firePoint == null)
        {
            Debug.LogWarning($"Fire Points 목록의 인덱스 {nextFirePointIndex}가 비어있습니다. 건너뜁니다.", this);
            // 다음 인덱스로 넘어가서 다음 프레임에 시도하도록 함
            nextFirePointIndex = (nextFirePointIndex + 1) % firePoints.Count;
            return;
        }

        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        IProjectile projectile = projectileGO.GetComponent<IProjectile>();

        if (projectile != null)
        {
            // 생성된 발사체에 데미지와 타겟 태그를 설정합니다.
            projectile.Damage = projectileDamage;
            projectile.AttackTargetTag = enemyTag;
        }
        else
        {
            Debug.LogWarning($"'{projectilePrefab.name}' 프리팹에 IProjectile을 구현하는 컴포넌트 부재", projectileGO);
        }

        // 다음 발사를 위해 인덱스를 순환시킵니다.
        nextFirePointIndex = (nextFirePointIndex + 1) % firePoints.Count;
    }

    // 씬 뷰에서 터렛의 사거리를 시각적으로 표시
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
