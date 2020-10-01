using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerAnimator : MonoBehaviour
{
    public void Progress() {
        _animator.Progress(_renderer, _spriteAtlas);
    }

    #region IDLE
    private void IdleIn_Enter() {
        _animator.ChangeAnimation(
            "Idle_in",
            false,
            () => {
                _fsm.ChangeState(States.Idle);
            }
        );
    }
    private void Idle_Enter() {
        _animator.ChangeAnimation("Idle_loop", true);
    }

    private void Idle_Update() {
        if (_jumpRequested) {
            _fsm.ChangeState(States.Jump);
        }
        else if (_playerAttack.RequestThrow) {
            _fsm.ChangeState(States.Throw);
        }
        else if (_playerAttack.RequestedAttackCount > 0) {
            _fsm.ChangeState(States.AttackA);
        }
        else if (Mathf.Abs(_direction.x) > Mathf.Epsilon) {
            _fsm.ChangeState(States.RunIn);
        }
    }
    private void IdleIn_Update() {
        Idle_Update();
    }
    #endregion

    #region RUN

    private void RunIn_Enter() {
        _animator.ChangeAnimation(
            "Run_in",
            false,
            () => {
                States nextState = (Mathf.Abs(_direction.x) > Mathf.Epsilon) ? States.Run : States.RunOut;
                _fsm.ChangeState(nextState);
            }
        );
    }
    private void RunOut_Enter() {
        _animator.ChangeAnimation(
            "Run_out",
            false,
            () => {
                States nextState = (Mathf.Abs(_direction.x) > Mathf.Epsilon) ? States.RunIn : States.Idle;
                _fsm.ChangeState(nextState);
            }
        );
    }
    private void Run_Enter() {
        _animator.ChangeAnimation("Run_loop",true);
    }

    private void Run_Update() {
        if (_jumpRequested) {
            _fsm.ChangeState(States.Jump);
        }
        else if (_playerAttack.RequestThrow) {
            _fsm.ChangeState(States.Throw);
        }
        else if (_playerAttack.RequestedAttackCount > 0) {
            _fsm.ChangeState(States.AttackA);
        }
        else if (Mathf.Abs(_direction.x) < Mathf.Epsilon) {
            _fsm.ChangeState(States.RunOut);
        }
    }
    private void RunOut_Update() {
        if (_jumpRequested) {
            _fsm.ChangeState(States.Jump);
        }
        else if (_playerAttack.RequestThrow) {
            _fsm.ChangeState(States.Throw);
        }
        else if (_playerAttack.RequestedAttackCount > 0) {
            _fsm.ChangeState(States.AttackA);
        }
        else if (Mathf.Abs(_direction.x) > Mathf.Epsilon) {
            _fsm.ChangeState(States.Run);
        }
    }
    private void RunIn_Update() {
        Run_Update();
    }

    #endregion

    #region Attack
    private void AttackA_Enter() {
        _direction.x = 0f;
        --_playerAttack.RequestedAttackCount;
        _playerAttack.AlreadyHitColliders.Clear();
        _animator.ChangeAnimation(
            "AttackA", 
            false, 
            () => {
                States nextState = (_playerAttack.RequestedAttackCount > 0) ? States.AttackB : States.AttackAOut;
                _fsm.ChangeState(nextState);
            }
        );
    }

    private void AttackA_Update() {
        _player.CanDrawHitbox = true;
        if (_animator.SpriteIndex == 0) {
            _playerAttack.EnableMeleeAttack();
        }
    }

    private void AttackB_Enter() {
        _playerAttack.AlreadyHitColliders.Clear();
        --_playerAttack.RequestedAttackCount;
        _animator.ChangeAnimation(
            "AttackB", 
            false,
            () => {
                States nextState = (_playerAttack.RequestedAttackCount > 0) ? States.AttackA : States.IdleIn;
                _fsm.ChangeState(nextState);
            }
        );
    }

    private void AttackB_Update() {
        _player.CanDrawHitbox = true;
        if (_animator.SpriteIndex == 0) {
            _playerAttack.EnableMeleeAttack();
        }
    }

    private void AttackAOut_Enter() {
        _animator.ChangeAnimation(
            "AttackA_out", 
            false,
            () => {
                States nextState = (Mathf.Abs(_direction.x) < Mathf.Epsilon) ? States.Idle : States.Run;
                _fsm.ChangeState(nextState);
            }
        );
    }
    private void AttackAir_Enter() {
        --_playerAttack.RequestedAttackCount;
        _playerAttack.AlreadyHitColliders.Clear();
        _animator.ChangeAnimation(
            "Attack_air",
            false,
            () => {
                _fsm.ChangeState(States.Jump);
            }
        );
    }
    private void AttackAir_Update() {
        _player.CanDrawHitbox = true;
        if (_animator.SpriteIndex == 0) {
            _playerAttack.EnableMeleeAttack();
        }
    }
    #endregion

    #region THROW

    private void Throw_Enter() {
        _playerAttack.RequestThrow = false;

        _animator.ChangeAnimation(
            "Throw", 
            false,
            () => {
                States nextState = (Mathf.Abs(_direction.x) < Mathf.Epsilon) ? States.IdleIn : States.Run;
                _fsm.ChangeState(nextState);
            }
        );
        _playerAttack.CanThrow = true;
    }

    private void Throw_Update() {
        if (_playerAttack.CanThrow && _animator.SpriteIndex == 1) {
            _playerAttack.CanThrow = false;
            _playerAttack.ThrowTalisman();
        }
    }

    private void ThrowAir_Enter() {
        _playerAttack.RequestThrow = false;
        _animator.ChangeAnimation(
            "Throw_air",
            false,
            ()=> 
            {
                _fsm.ChangeState(States.Jump);
            }
        );
    }
    private void ThrowAir_Update() {
        Throw_Update();
    }

    #endregion

    #region JUMP
    private void Jump_Enter() {
        _jumpRequested = false;
        _animator.ChangeAnimation("Jump");
    }

    private void Jump_Update() {
        if (_playerAttack.RequestThrow) {
            _fsm.ChangeState(States.ThrowAir);
        }
        else if (_playerAttack.RequestedAttackCount > 0) {
            _fsm.ChangeState(States.AttackAir);
        }
        else if (Mathf.Abs(_direction.y) < Mathf.Epsilon) {
            States nextState = (Mathf.Abs(_direction.x) < Mathf.Epsilon) ? States.IdleIn : States.Run;
            _fsm.ChangeState(nextState);
        }

        if (Mathf.Abs(_player.Velocity.y) < 1) {
            _animator.SpriteIndex = 1;
        }
        else if (Mathf.Sign(_direction.y) == -1) {
            _animator.SpriteIndex = 0;
        }
        else if (Mathf.Sign(_direction.y) == 1) {
            _animator.SpriteIndex = 2;
        }
    }
    #endregion

    #region HIT

    public void SetPlayerHit() => _fsm.ChangeState(States.Hit);

    private void Hit_Enter() {
        _direction.x = -Mathf.Sign(_direction.x)*3f;
        _direction.y = 2f;
        _animator.ChangeAnimation(
            "Hit",
            false,
            () => {
                States nextState = (Mathf.Abs(_direction.y) < Mathf.Epsilon) ? States.Idle : States.Jump;
                _fsm.ChangeState(nextState);
            }
        );
    }
    
    #endregion
}