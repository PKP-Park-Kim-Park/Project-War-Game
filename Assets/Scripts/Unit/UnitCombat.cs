using UnityEngine;

public class UnitCombat
{
    private Transform rayOrigin;
    private Vector3 moveDirection;
    private string attackTargetTag;
    private int attackDamage;
    private float attackRange;

    public void Setup(Transform rayOrigin, Vector3 moveDirection, string attackTargetTag, int attackDamage, float attackRange)
    {
        this.rayOrigin = rayOrigin;
        this.moveDirection = moveDirection;
        this.attackTargetTag = attackTargetTag;
        this.attackDamage = attackDamage;
        this.attackRange = attackRange;
    }

    public void Attack()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin.position, moveDirection, attackRange);
        Debug.DrawRay(rayOrigin.position, moveDirection * attackRange, Color.green);
        GameObject closestTarget = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D collider = hits[i].collider;
            if (collider != null && collider.CompareTag(attackTargetTag))
            {
                float distance = Vector2.Distance(rayOrigin.position, hits[i].point);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = collider.gameObject;
                }
            }
        }

        if (closestTarget != null)
        {
            IDamageable target = closestTarget.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(attackDamage);
            }
        }
    }
}
