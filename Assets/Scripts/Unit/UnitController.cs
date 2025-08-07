using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UnitType
{
    Normal,
    Archer,
    Tank
}

[System.Serializable]
public struct UnitStat
{
    public UnitType unitType;
    public float moveSpeed;
    public int maxHealth;
    public int attackDamage;
    public float attackRange;
}

public class UnitController : MonoBehaviour, IDamageable
{
    [Header("Unit Stats")]
    [SerializeField] private List<UnitStat> _unitStatsList;
    [SerializeField] private UnitType _unitType;
    private float _moveSpeed;
    private int _maxHealth;
    private int _attackDamage;
    private int _currentHealth;
    private float _attackRange;
    private string _attackTargetTag;

    private string _stopTargetTag;

    [Header("Unit Component")]
    [SerializeField] private Slider _healthBar;
    [SerializeField] private Transform _rayShootTransform;
    [SerializeField] private float _moveStopRayDistance = 1f;
    private Animator _animator;
    private bool _isMoveCommanded = false;
    private bool _isAttacking = false;
    private Vector3 _moveDirection;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        SettingStat();

    }

    private void Start()
    {
        SettingDiraction();
    }

    private void Update()
    {
        Move();

        CheckState();
    }

    private void TryAttack()
    {
        Debug.DrawRay(_rayShootTransform.position, _moveDirection * _attackRange, Color.green);

        RaycastHit2D[] hits = Physics2D.RaycastAll(_rayShootTransform.position, _moveDirection, _attackRange);

        GameObject closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null)
            {
                continue;
            }

            if (hit.collider.CompareTag(_attackTargetTag))
            {
                float dist = Vector2.Distance(_rayShootTransform.position, hit.point);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestEnemy = hit.collider.gameObject;
                }
            }
        }

        if (closestEnemy != null)
        {
            IDamageable attackTarget = closestEnemy.GetComponent<IDamageable>();
            if (attackTarget != null)
            {
                attackTarget.TakeDamage(_attackDamage);
                Debug.Log($"{_stopTargetTag}가 {_attackTargetTag}를 공격함!");
            }
        }
    }

    private void SettingStat()
    {
        UnitStat stat = _unitStatsList.Find(IsMatchingUnitType);

        _moveSpeed = stat.moveSpeed;
        _maxHealth = stat.maxHealth;
        _attackDamage = stat.attackDamage;
        _attackRange = stat.attackRange;
        _currentHealth = _maxHealth;

        if (gameObject.CompareTag("Player"))
        {
            _attackTargetTag = "Enemy";

            _stopTargetTag = "Player";
        }
        else if (gameObject.CompareTag("Enemy"))
        {
            _attackTargetTag = "Player";

            _stopTargetTag = "Enemy";
        }
        else
        {
            Debug.LogWarning("유닛에 태그가 안적용 되어있는듯?");
        }
    }

    private void SettingDiraction()
    {
        if (_spriteRenderer.flipX)
        {
            _moveDirection = Vector3.left;
            _rayShootTransform.localPosition = new Vector3(-_rayShootTransform.localPosition.x, _rayShootTransform.localPosition.y, _rayShootTransform.localPosition.z);
        }
        else
        {
            _moveDirection = Vector3.right;
        }
    }

    private bool IsMatchingUnitType(UnitStat stat)
    {
        return stat.unitType == _unitType;
    }

    private void Move()
    {
        if (_isMoveCommanded && !_isAttacking)
        {
            transform.position += _moveDirection * _moveSpeed * Time.deltaTime;
        }
    }

    private void CheckState()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(_rayShootTransform.position, _moveDirection, _attackRange);
        Debug.DrawLine(_rayShootTransform.position, _rayShootTransform.position + _moveDirection * _attackRange, Color.red);

        GameObject closestAttackTarget = null;
        GameObject closestStopTarget = null;
        float closestAttackDistance = float.MaxValue;
        float closestStopDistance = float.MaxValue;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null) continue;

            float hitDistance = Vector2.Distance(_rayShootTransform.position, hit.point);
            GameObject hitObject = hit.collider.gameObject;

            // 공격 대상 찾기
            if (hitObject.CompareTag(_attackTargetTag) && hitDistance < closestAttackDistance)
            {
                closestAttackDistance = hitDistance;
                closestAttackTarget = hitObject;
            }

            // 아군 또는 멈춰야 할 대상 찾기
            if (hitObject.CompareTag(_stopTargetTag) && hitDistance < closestStopDistance)
            {
                closestStopDistance = hitDistance;
                closestStopTarget = hitObject;
            }
        }

        switch (_unitType)
        {
            case UnitType.Normal:
                NormalState(closestAttackTarget, closestStopTarget, closestStopDistance);
                break;
            case UnitType.Archer:
                ArcherState(closestAttackTarget, closestStopTarget, closestStopDistance);
                break;
            case UnitType.Tank:
                break;
            default:
                break;
        }
    }

    private void ArcherState(GameObject attackTarget, GameObject stopTarget, float stopDist)
    {
        if (attackTarget != null)
        {
            SetAttack(true);
            SetMove(false);
        }
        else if (stopTarget != null)
        {
            SetMove(stopDist > _moveStopRayDistance);
            SetAttack(false);
        }
        else
        {
            SetMove(true);
            SetAttack(false);
        }
    }

    private void NormalState(GameObject attackTarget, GameObject stopTarget, float stopDist)
    {
        if (stopTarget != null && stopDist <= _moveStopRayDistance)
        {
            SetMove(false);
            SetAttack(false);
        }
        else if (attackTarget != null)
        {
            SetAttack(true);
            SetMove(false);
        }
        else
        {
            SetMove(true);
            SetAttack(false);
        }
    }

    private void SetMove(bool isMoving)
    {
        _animator.SetBool("IsMove", isMoving);
        _isMoveCommanded = isMoving;
    }

    private void SetAttack(bool isAttacking)
    {
        _animator.SetBool("IsAttack", isAttacking);
        _isAttacking = isAttacking;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        _healthBar.value -= damage;

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        _healthBar.gameObject.SetActive(false);
        _animator.SetTrigger("Die");
        gameObject.GetComponent<Collider2D>().enabled = false;

        StartCoroutine(WaitForDeathAnimation());
    }

    private IEnumerator WaitForDeathAnimation()
    {
        yield return null;

        // 애니메이션이 끝날 때까지 대기
        while (_animator.GetCurrentAnimatorStateInfo(0).IsName("UnitDie") == false
            && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        Destroy(gameObject);
    }

    public void StartEventAttack()
    {
        TryAttack();
    }
}
