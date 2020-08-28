using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerAnimator _playerAnimator;
    private PlayerAttack _playerAttack;
    private bool _attackRequested;
    private bool _throwRequested;
    private GroundMove _controller;
    public Vector2 Velocity { get { return _controller.Velocity;} }
    public bool CanDrawHitbox { get; set; }

    public void Initialize() {
        _controller = GetComponent<GroundMove>();
        _playerAttack = GetComponent<PlayerAttack>();
        _playerAnimator = GetComponent<PlayerAnimator>();

        var status = GetComponent<TBLPlayerStatus>();
        _controller.Initialize(status.moveSpeed, status.minJumpHeight, status.maxJumpHeight);

        _playerAttack.Initialize();
        _playerAnimator.Initalize();
    }

    public void Progress() {
        bool throwRequested = _throwRequested && (Mathf.Abs(Velocity.y) < Mathf.Epsilon); 
        _playerAttack.Progress(_attackRequested, throwRequested);
        _playerAnimator.Progress();
    }

    public void FixedProgress() {
        bool jumpRequested = _controller.JumpRequested;
        _controller.FixedProgress();
        _playerAttack.FixedProgress();
        _playerAnimator.ApplyAnimation(_controller.InputX, Velocity.y, jumpRequested || _controller.JumpRequested);
    }

    public void SetInputX(float horizontal)
    {
        if (_playerAttack.IsInAttackProgress) {
            horizontal = 0f;
        }
        _controller.InputX = horizontal;
    }

    public void SetInputY(float vertical) {
        _controller.InputY = vertical;
    }

    public void SetJump(bool jumpPressed) {
        if (!_playerAttack.IsInAttackProgress)
            _controller.SetJump(jumpPressed);
        else
            _controller.SetJumpEnd(true, true);   
    }

    public void SetJumpEnd(bool isNotPressed, bool pressedAtLastFrame) {
        if (!_playerAttack.IsInAttackProgress)
            _controller.SetJumpEnd(isNotPressed, pressedAtLastFrame);
    }

    public void SetThrow(bool throwPressed) => _throwRequested = throwPressed; 

    public void SetAttack(bool attackPressed) => _attackRequested = attackPressed;

    public void ReceiveDamage(int damage) {
        // _hp -= damage;
        EffectManager.GetInstance().ShakeCamera(0.2f);
        _playerAnimator.SetPlayerHit();
    }   

    private void OnDrawGizmos() {
        if (Application.isPlaying && !CanDrawHitbox) return;
        CanDrawHitbox = false;

        var status = GetComponent<TBLPlayerStatus>();
        float dirX = transform.localScale.x;
        Vector2 position = transform.position;
        Vector2 offset = new Vector2(status.meleeAttackOffset.x * dirX, status.meleeAttackOffset.y);
        position += offset;

        Vector2[] attackBoxPoints = new Vector2[4] {
            new Vector2(-1f, 1f),
            new Vector2(1f, 1f),
            new Vector2(-1f, -1f),
            new Vector2(1f, -1f)
        };
        
        for (int i = 0; i < 4; ++i) {
            attackBoxPoints[i] = position + attackBoxPoints[i] * status.meleeAttackSize * 0.5f;
        }

        Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
        Gizmos.DrawCube(position, status.meleeAttackSize);
    }
}
