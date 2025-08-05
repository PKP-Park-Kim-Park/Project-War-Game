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
    private Vector3 _moveDirection;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _currentHealth = _maxHealth;
    }

    private void Start()
    {
        if (_spriteRenderer.flipX)
        {
            _moveDirection = Vector3.left;
            _rayShootTransform.localPosition = new Vector3(-_rayShootTransform.localPosition.x, _rayShootTransform.localPosition.y, _rayShootTransform.localPosition.z);
            _AttackCollider.transform.localPosition = new Vector3(-_AttackCollider.transform.localPosition.x, _AttackCollider.transform.localPosition.y, _AttackCollider.transform.localPosition.z);
        }
        else
        {
            _moveDirection = Vector3.right;
        }
    }

    private void Update()
    {
        Move();

        CheckState();
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
        RaycastHit2D hit = Physics2D.Raycast(_rayShootTransform.position, _moveDirection, _rayDistance);
        Debug.DrawLine(_rayShootTransform.position, _rayShootTransform.position + _moveDirection * _rayDistance, Color.red);
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
            UnitController attackTarget = collision.GetComponent<UnitController>();
            if (attackTarget != null)
            {
                attackTarget.TakeDamage(10);
                Debug.Log($"{_stopTargetTag}가 {_attackTargetTag}를 공격함!");
            }
        }   
    }
}
