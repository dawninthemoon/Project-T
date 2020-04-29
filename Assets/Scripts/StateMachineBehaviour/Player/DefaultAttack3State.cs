using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultAttack3State : AttackStateBase
{
    private static readonly int ClipCount = 5;

    override protected void OnStateEntered(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEntered(animator, stateInfo, layerIndex);
        Context.SpawnAttackEffect(3);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       RequestEnableHitbox(3, ClipCount);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Context.AttackEnd();
    }
}
