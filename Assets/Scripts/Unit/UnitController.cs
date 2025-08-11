using System.Collections;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum UnitType
{
    Normal,
    Archer,
    Tank,
    Long
}

public class UnitController : MonoBehaviour, IDamageable
{
    [Header("Unit Settings")]
    [SerializeField] private UnitData unitData;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Transform rayShootTransform;
    [SerializeField] private float stopDistance = 1f;

    private UnitStat stat = new UnitStat();
    private UnitAnimation unitAnimation;
    private UnitCombat combat = new UnitCombat();

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector3 moveDirection;
    private int currentHealth;
    private bool isMoving = false;
    private bool isAttacking = false;
    private bool isDie = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        unitAnimation = new UnitAnimation(animator);
        stat.Initialize(unitData, gameObject.tag);

        currentHealth = stat.MaxHealth;

        SetMoveDirection();
        combat.Setup(rayShootTransform, moveDirection, stat.AttackTargetTags, stat.AttackDamage, stat.AttackRange, stat.Gold, stat.Exp);
    }

    private void Start()
    {
        healthBar.maxValue = stat.MaxHealth;
        healthBar.value = stat.MaxHealth;
    }

    private void Update()
    {
        if (isDie)
        {
            return;
        }

        if (isMoving && !isAttacking)
        {
            transform.position += moveDirection * stat.MoveSpeed * Time.deltaTime;
        }

        CheckState();
    }

    private void SetMoveDirection()
    {
        if (spriteRenderer.flipX)
        {
            moveDirection = Vector3.left;
            rayShootTransform.localPosition = new Vector3(-rayShootTransform.localPosition.x, rayShootTransform.localPosition.y, rayShootTransform.localPosition.z);
        }
        else
        {
            moveDirection = Vector3.right;
        }
    }

    private void CheckState()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayShootTransform.position, moveDirection, stat.AttackRange);
        GameObject closestAttack = null;
        GameObject closestStop = null;

        float minAttackDist = float.MaxValue;
        float minStopDist = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D collider = hits[i].collider;
            if (collider == null) continue;

            float distance = Vector2.Distance(rayShootTransform.position, hits[i].point);
            GameObject target = collider.gameObject;

            // 공격 가능한 태그 목록에 포함되어 있는지 확인합니다.
            if (stat.AttackTargetTags.Contains(target.tag) && distance < minAttackDist)
            {
                minAttackDist = distance;
                closestAttack = target;
            }

            if (target.CompareTag(stat.StopTargetTag) && distance < minStopDist)
            {
                minStopDist = distance;
                closestStop = target;
            }
        }

        if (stat.UnitType == UnitType.Archer || stat.UnitType == UnitType.Long)
        {
            if (closestStop != null && minStopDist <= stopDistance)
            {
                // 아군이 가로막고 있으면 멈춤, 공격은 적이 보일 때만
                SetMove(false);
                SetAttack(closestAttack != null);
            }
            else if (closestAttack != null)
            {
                // 적이 있는데 stopDistance 밖이면 이동하면서 공격
                SetMove(minAttackDist > stopDistance);
                SetAttack(true);
            }
            else
            {
                // 아무것도 없으면 그냥 이동
                SetMove(true);
                SetAttack(false);
            }
        }
        else if (stat.UnitType == UnitType.Normal || stat.UnitType == UnitType.Tank)
        {
            if (closestStop != null && minStopDist <= stopDistance)
            {
                SetMove(false);
                SetAttack(false);
            }
            else if (closestAttack != null)
            {
                SetMove(false);
                SetAttack(true);
            }
            else
            {
                SetMove(true);
                SetAttack(false);
            }
        }
    }

    private void SetMove(bool move)
    {
        isMoving = move;
        unitAnimation.PlayMove(move);
    }

    private void SetAttack(bool attack)
    {
        isAttacking = attack;
        unitAnimation.PlayAttack(attack);
    }

    public void StartEventAttack()
    {
        combat.Attack();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.value -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        healthBar.gameObject.SetActive(false);
        unitAnimation.PlayDie();
        GetComponent<Collider2D>().enabled = false;
        isDie = true;

        if (gameObject.CompareTag("Enemy"))
        {
            GoldManager.instance.AddGold(stat.Gold);
            ExpManager.instance.AddExp(stat.Exp);
        }

        yield return unitAnimation.WaitForDeathAnimation();

        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}