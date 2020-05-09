using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMoveAttack1State : AttackStateBase
{
    private static readonly int ClipCount = 7;
    private const string AttackEffectName = "defaultAttack1_move";

    override protected void OnStateEntered(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   
        base.OnStateEntered(animator, stateInfo, layerIndex);
        Context.EnterAttackProgress();
        Context.SpawnAttackEffect(AttackEffectName, true);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RequestEnableHitbox(1, ClipCount, 4);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Context.CurrentAttackState <= 1) {
            Context.AttackEnd();
        }
    }
}
