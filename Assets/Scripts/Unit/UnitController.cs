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

public class UnitController : MonoBehaviour
{
    [Header("Unit Stats")]
    [SerializeField] private List<UnitStat> _unitStatsList;
    [SerializeField] private UnitType _unitType;
    private float _moveSpeed;
    private int _maxHealth;
    private int _attackDamage;
    private int _currentHealth;
    private float _attackRange;

    [Header("Unit Component")]
    [SerializeField] private Slider _healthBar;
    [SerializeField] private string _attackTargetTag;
    [SerializeField] private string _stopTargetTag;
    [SerializeField] private Transform _rayShootTransform;
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

    private void CheckAttack()
    {
        Debug.DrawRay(_rayShootTransform.position, _moveDirection * _attackRange, Color.green);
        GameObject closestEnemy = GetNearestEnemyInRay(_rayShootTransform.position, _moveDirection, _attackRange, _attackTargetTag);

        if (closestEnemy != null)
        {
            UnitController attackTarget = closestEnemy.GetComponent<UnitController>();
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
        RaycastHit2D hit = Physics2D.Raycast(_rayShootTransform.position, _moveDirection, _attackRange);
        Debug.DrawLine(_rayShootTransform.position, _rayShootTransform.position + _moveDirection * _attackRange, Color.red);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag(_stopTargetTag))
            {
                SetMoveState(false);
            }
            else if (hit.collider.CompareTag(_attackTargetTag))
            {
                SetAttackState(true);
            }
        }
        else
        {
            // 레이캐스트에 맞은 대상이 없을 때 기본 동작
            SetAttackState(false);
            SetMoveState(true);
        }
    }

    private void SetMoveState(bool isMoving)
    {
        _animator.SetBool("IsMove", isMoving);
        _isMoveCommanded = isMoving;
    }

    private void SetAttackState(bool isAttacking)
    {
        _animator.SetBool("IsAttack", isAttacking);
        _isAttacking = isAttacking;
    }

    public void TakeDamage(int damage)
    {

        _currentHealth -= damage;
        _healthBar.value -= damage;
        if (_currentHealth < 0)
        {
            _currentHealth = 0;
        }

        if (_currentHealth <= 0)
        {
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

        // 상태 전환될 때까지 기다림
        while (!_animator.GetCurrentAnimatorStateInfo(0).IsName("UnitDie"))
        {
            yield return null;
        }

        // 애니메이션이 끝날 때까지 대기
        while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        Destroy(gameObject);
    }

    public void StartEventAttack()
    {
        CheckAttack();
    }

    public GameObject GetNearestEnemyInRay(Vector2 origin, Vector2 direction, float distance, string targetTag)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, distance);

        GameObject nearestEnemy = null;
        float minDistance = float.MaxValue;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag(targetTag))
            {
                if (hit.distance < minDistance)
                {
                    minDistance = hit.distance;
                    nearestEnemy = hit.collider.gameObject;
                }
            }
        }

        return nearestEnemy;
    }
}
