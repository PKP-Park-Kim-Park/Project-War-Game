using System.Collections;
using UnityEngine;

public class UnitAnimation
{
    private Animator animator;

    public UnitAnimation(Animator animator)
    {
        this.animator = animator;
    }

    public void PlayMove(bool isMoving)
    {
        animator.SetBool("IsMove", isMoving);
    }

    public void PlayAttack(bool isAttacking)
    {
        animator.SetBool("IsAttack", isAttacking);
    }

    public void PlayDie()
    {
        animator.SetTrigger("Die");
    }

    public IEnumerator WaitForDeathAnimation()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("UnitDie"))
        {
            yield return null;
        }

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
    }
}
