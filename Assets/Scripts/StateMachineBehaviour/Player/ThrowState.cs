using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowState : StateMachineBehaviour<PlayerAttack>
{
    override protected void OnStateEntered(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        --Context.TalismanCount;

        Context.EnterAttackProgress();
        Context.RequestThrow = false;
        
        Context.ThrowTalisman();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Context.AttackEnd();
    }
}
