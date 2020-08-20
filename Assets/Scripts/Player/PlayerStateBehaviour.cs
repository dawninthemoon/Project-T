using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerAnimator : MonoBehaviour
{
    public void Progress() {
        _animator.Progress(_renderer, _spriteAtlas);
    }

    #region IDLE
    private void Idle_Enter() {
        _animator.ChangeAnimation("idle", true);
    }

    private void Idle_Update() {
        if (_jumpRequested) {
            _fsm.ChangeState(States.Jump);
        }
        else if (_playerAttack.RequestThrow) {
            _fsm.ChangeState(States.Throw);
        }
        else if (_playerAttack.RequestedAttackCount > 0) {
            _fsm.ChangeState(States.AttackIn);
        }
        else if (Mathf.Abs(_direction.x) > Mathf.Epsilon) {
            _fsm.ChangeState(States.Run);
        }
    }
    #endregion

    #region RUN

    private void Run_Enter() {
        _animator.ChangeAnimation("run", true);
    }

    private void Run_Update() {
        if (_jumpRequested) {
            _fsm.ChangeState(States.Jump);
        }
        else if (_playerAttack.RequestThrow) {
            _fsm.ChangeState(States.Throw);
        }
        else if (_playerAttack.RequestedAttackCount > 0) {
            _fsm.ChangeState(States.AttackIn);
        }
        else if (Mathf.Abs(_direction.x) < Mathf.Epsilon) {
            _fsm.ChangeState(States.Idle);
        }
    }

    #endregion

    #region Attack
    private void AttackIn_Enter() {
        _direction.x = 0f;
        _animator.ChangeAnimation(
            "attack_in", 
            false,
            () => {
                _fsm.ChangeState(States.AttackA);
            });
    }
    private void AttackA_Enter() {
        --_playerAttack.RequestedAttackCount;
        _animator.ChangeAnimation(
            "Attack_A", 
            false, 
            () => {
                States nextState = (_playerAttack.RequestedAttackCount > 0) ? States.AttackB : States.AttackOut;
                _fsm.ChangeState(nextState);
            });
    }
    private void AttackB_Enter() {
        --_playerAttack.RequestedAttackCount;
        _animator.ChangeAnimation(
            "Attack_B", 
            false,
            () => {
                States nextState = (_playerAttack.RequestedAttackCount > 0) ? States.AttackA : States.AttackOut;
                _fsm.ChangeState(nextState);
            });
    }
    private void AttackOut_Enter() {
        _animator.ChangeAnimation(
            "attack_out", 
            false,
            ()=> {
                States nextState = (Mathf.Abs(_direction.x) < Mathf.Epsilon) ? States.Idle : States.Run;
                _fsm.ChangeState(nextState);
            });
    }
    #endregion

    #region THROW

    private void Throw_Enter() {
        _playerAttack.RequestThrow = false;
        _animator.ChangeAnimation(
            "Throw", 
            false,
            () => {
                States nextState = (Mathf.Abs(_direction.x) < Mathf.Epsilon) ? States.Idle : States.Run;
                _fsm.ChangeState(nextState);
            });
    }

    private void ThrowAir_Enter() {
        _playerAttack.RequestThrow = false;
        _animator.ChangeAnimation(
            "throw_air",
            false,
            ()=> 
            {
                States nextState = (Mathf.Abs(_direction.x) < Mathf.Epsilon) ? States.Idle : States.Run;
                _fsm.ChangeState(nextState);
            });
    }

    #endregion

    #region JUMP

    private void Jump_Enter() {
        _jumpRequested = false;
        _animator.ChangeAnimation("jump");
    }

    private void Jump_Update() {
        if (_direction.y < 0f) {
            _fsm.ChangeState(States.Fall);
        }
    }

    private void Fall_Enter() {
        _animator.ChangeAnimation("fall");
    }

    private void Fall_Update() {
        if (Mathf.Abs(_direction.y) < Mathf.Epsilon) {
            States nextState = (Mathf.Abs(_direction.x) < Mathf.Epsilon) ? States.LandIdle : States.LandRun;
            _fsm.ChangeState(nextState);
        }
    }
    #endregion

    #region LAND

    private void LandIdle_Enter() {
        _animator.ChangeAnimation(
            "land_idle", 
            false,
            () => { _fsm.ChangeState(States.Idle); });
    }

    private void LandRun_Enter() {
        _animator.ChangeAnimation(
            "land_run", 
            false,
            () => { _fsm.ChangeState(States.Run); });
    }

    private void LandRun_Update() {
        Run_Update();
    }
    #endregion
}