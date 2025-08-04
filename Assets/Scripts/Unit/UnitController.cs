using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 1.0f;
    private Animator _animator;
    private bool _isMoveCommanded = false;
    private bool _isAttacking = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Move();
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
}
