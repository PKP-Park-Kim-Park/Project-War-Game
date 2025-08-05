using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitController : MonoBehaviour
{
    [Header("Unit Stats")]
    [SerializeField] private float _moveSpeed = 1.0f;
    [SerializeField] private int _maxHealth = 100;
    private int _currentHealth;

    [Header("Unit Component")]
    [SerializeField] private Slider _healthBar;
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

    /// <summary>
    /// 유닛의 움직임 상태 함수
    /// </summary>
    /// <param name="isMoving">유닛을 움직이겠나?</param>
    public void SetMoveState(bool isMoving)
    {
        _animator.SetBool("IsMove", isMoving);
        _isMoveCommanded = isMoving;
    }

    /// <summary>
    /// 유닛의 공격 상태 함수
    /// </summary>
    /// <param name="isAttacking">유닛이 공격을 하겠는가?</param>
    public void SetAttackState(bool isAttacking)
    {
        _animator.SetBool("IsAttack", isAttacking);
        _isAttacking = isAttacking;
    }
    
    /// <summary>
    /// 유닛이 데미지를 받음
    /// </summary>
    /// <param name="damage">유닛이 받을 데미지</param>
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
            Debug.Log("게임 오버");
        }

    }
}
