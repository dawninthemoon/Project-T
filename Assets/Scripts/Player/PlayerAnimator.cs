using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using UnityEngine.U2D;

[RequireComponent(typeof(StateMachineRunner))]
public partial class PlayerAnimator : MonoBehaviour
{
    private SpriteAtlas _spriteAtlas;

    public enum States { 
        Idle, AttackA, AttackB, AttackAir, AttackIn, AttackOut, Dead, Evade, Explode, 
        Fall, Hit, Jump, LandHard, LandIdle, LandRun, Run, Slide, Throw, ThrowAir 
    };

    public States State { get => _fsm.State; }
    private StateMachine<States> _fsm;
    private SpriteRenderer _renderer;
    private SpriteAtlasAnimator _animator;

    private Player _player;
    private PlayerAttack _playerAttack;

    #region Non-reference Fields
    private Vector2 _direction;
    private bool _jumpRequested;
    #endregion

    public void Initalize() {
        _animator = new SpriteAtlasAnimator();
        _fsm = GetComponent<StateMachineRunner>().Initialize<States>(this);
        _renderer = GetComponent<SpriteRenderer>();
        _player = GetComponent<Player>();
        _playerAttack = GetComponent<PlayerAttack>();
        _spriteAtlas = Resources.Load<SpriteAtlas>("Atlas/CharacterAtlas1");

        _animator.Initalize("PLAYER_", "idle", true);
        _fsm.ChangeState(States.Idle);
    }

    public void Progress() {
        _animator.Progress(_renderer, _spriteAtlas);
    }

    public void ApplyAnimation(float dirX, float velocityY, bool requestJump) {
        _direction.x = dirX;
        _direction.y = (velocityY > 0f) ? 1 : ((velocityY < 0f) ? -1 : 0);

        if (requestJump) {
            _jumpRequested = true;
        }

        if (Mathf.Abs(dirX) > Mathf.Epsilon) {
            SetDirection(dirX);
        }
    }

    public void SetDirection(float dirX) {
        Vector3 scaleVector = new Vector3(Mathf.Sign(dirX), 1f, 1f);
        transform.localScale = scaleVector;
    }
}
