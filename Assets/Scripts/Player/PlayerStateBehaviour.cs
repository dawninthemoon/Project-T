using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerAnimator : MonoBehaviour
{
    #region IDLE
    private void Idle_Enter() {
        _animator.ChangeAnimation("idle", true);
    }

    private void Idle_Update() {
        if (Mathf.Abs(_direction.x) > Mathf.Epsilon) {
            _fsm.ChangeState(States.Run);
        }
    }
    #endregion

    #region RUN

    private void Run_Enter() {
        _animator.ChangeAnimation("run", true);
    }

    private void Run_Update() {
        if (Mathf.Abs(_direction.x) < Mathf.Epsilon) {
            _fsm.ChangeState(States.Idle);
        }
    }

    #endregion

    #region Attack
    private void Attack_A_Enter() {

    }
    private void Attack_B_Enter() {

    }
    #endregion
}