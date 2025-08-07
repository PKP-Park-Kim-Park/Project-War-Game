using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UnitCombat
{
    private Transform rayOrigin;
    private Vector3 moveDirection;
    private List<string> attackTargetTags;
    private int attackDamage;
    private float attackRange;

    public void Setup(Transform rayOrigin, Vector3 moveDirection, List<string> attackTargetTags, int attackDamage, float attackRange)
    {
        this.rayOrigin = rayOrigin;
        this.moveDirection = moveDirection;
        this.attackTargetTags = attackTargetTags;
        this.attackDamage = attackDamage;
        this.attackRange = attackRange;
    }

    public void Attack()
    {
        if (rayOrigin == null || !attackTargetTags.Any()) return;

        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin.position, moveDirection, attackRange);
        Debug.DrawRay(rayOrigin.position, moveDirection * attackRange, Color.green);
        GameObject closestTarget = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D collider = hits[i].collider;
            // 여러 태그 중 하나와 일치하는지 확인
            if (collider != null && attackTargetTags.Contains(collider.tag))
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
