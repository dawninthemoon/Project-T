﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultAttack2State : AttackStateBase
{
    private static readonly int ClipCount = 5;

    override protected void OnStateEntered(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEntered(animator, stateInfo, layerIndex);
        Context.SpawnAttackEffect(2);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RequestEnableHitbox(2, ClipCount);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Context.CurrentAttackState <= 2) {
            Context.AttackEnd();
        }
    }
}