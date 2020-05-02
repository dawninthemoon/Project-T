using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultJumpAttackState : StateMachineBehaviour<PlayerAttack>
{
    private static readonly int ClipCount = 7;

    private const string HitEffectName = "PlayerEffect/meleeAttack_hit";
    private const string AttackEffectname = "jumpAttack";
    private static readonly Vector2 _hitboxOffset = new Vector2(0.65f, 0.75f);
    private static readonly Vector2 _hitboxSize = new Vector2(3f, 2.1f);
    private int _attackDamage;
    private float _timeAgo = 0f;
    private float _stateLength;
    private float DirX { get { return Transform.localScale.x; } }

    override protected void OnStateEntered(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   
        _attackDamage = Context.DefaultAttackDamage;
        _timeAgo = 0f;
        _stateLength = stateInfo.length;
        Context.AlreadyHitColliders.Clear();
        Context.EnterAttackProgress();
        Context.SpawnAttackEffect(AttackEffectname, true);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RequestEnableHitbox(2);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Context.AttackEnd();
    }

    private void RequestEnableHitbox(int delayCount) {
        _timeAgo += Time.deltaTime;
        if (_timeAgo >= _stateLength / ClipCount * delayCount
            && _timeAgo < _stateLength / ClipCount * (delayCount + 1)) {
            Vector2 offset = _hitboxOffset;
            offset.x *= DirX;
            Vector2 hitboxPoint = (Vector2)Transform.position + offset;

            Context.EnableHitbox(hitboxPoint, _hitboxSize, _attackDamage, HitEffectName);
        }
    }
}
