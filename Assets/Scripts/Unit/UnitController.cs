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
    /// ������ ������ ���� �Լ�
    /// </summary>
    /// <param name="isMoving">������ �����̰ڳ�?</param>
    public void SetMoveState(bool isMoving)
    {
        _animator.SetBool("IsMove", isMoving);
        _isMoveCommanded = isMoving;
    }

    /// <summary>
    /// ������ ���� ���� �Լ�
    /// </summary>
    /// <param name="isAttacking">������ ������ �ϰڴ°�?</param>
    public void SetAttackState(bool isAttacking)
    {
        _animator.SetBool("IsAttack", isAttacking);
        _isAttacking = isAttacking;
    }
}
