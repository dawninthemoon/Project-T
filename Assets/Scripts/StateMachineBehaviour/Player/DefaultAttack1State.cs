using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultAttack1State : AttackStateBase
{
    private static readonly int ClipCount = 6;

    override protected void OnStateEntered(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   
        base.OnStateEntered(animator, stateInfo, layerIndex);
        Context.EnterAttackProgress();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RequestEnableHitbox(ClipCount, 0);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Context.RequestedAttackCount == 0) {
            Context.AttackEnd();
        }
    }
}
