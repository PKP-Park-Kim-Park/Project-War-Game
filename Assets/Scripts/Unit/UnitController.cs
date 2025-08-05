using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class UnitController : MonoBehaviour
{
    [Header("Unit Stats")]
    [SerializeField] private float _moveSpeed = 1.0f;
    [SerializeField] private int _maxHealth = 100;
    private int _currentHealth;

    [Header("Unit Component")]
    [SerializeField] private Slider _healthBar;
    [SerializeField] private string _attackTargetTag;
    [SerializeField] private string _stopTargetTag;
    [SerializeField] private Transform _rayShootTransform;
    [SerializeField] private float _rayDistance = 3f;
    [SerializeField] private Collider2D _AttackCollider;
    private Animator _animator;
    private bool _isMoveCommanded = false;
    private bool _isAttacking = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _currentHealth = _maxHealth;
    }

    private void Update()
    {
        Move();

        CheckState();

        if (Input.GetKeyDown(KeyCode.D))
        {
            TakeDamage(25);
        }
    }

    private void Move()
    {
        if (_isMoveCommanded && !_isAttacking)
        {
            transform.position += Vector3.right * _moveSpeed * Time.deltaTime;
        }
    }

    private void CheckState()
    {
        RaycastHit2D hit = Physics2D.Raycast(_rayShootTransform.position, Vector3.right, _rayDistance);
        Debug.DrawLine(_rayShootTransform.position, _rayShootTransform.position + Vector3.right * _rayDistance, Color.red);
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
            // 사망 로직 추가
            Debug.Log("유닛 사망");
        }

    }

    public void StartEventAttack()
    {
        _AttackCollider.enabled = true;
    }

    public void EndEventAttack()
    {
        _AttackCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_attackTargetTag))
        {
            UnitController enemy = collision.GetComponent<UnitController>();
            if (enemy != null)
            {
                enemy.TakeDamage(10);
                Debug.Log($"{_stopTargetTag}가 {_attackTargetTag}를 공격함!");
            }
        }   
    }
}
