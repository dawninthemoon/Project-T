using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

public class AttackStateBase : StateMachineBehaviour<PlayerAttack>
{
    protected float _timeAgo = 0f;
    protected float _stateLength = 0f;

    private const string HitEffectName = "meleeAttack_hit";
    private static readonly Vector2 _hitboxOffset = new Vector2(0.4f, 0.75f);
    private static readonly Vector2 _hitboxSize = new Vector2(3f, 1.5f);

    private int _attackDamage;

    private float DirX { get { return Transform.localScale.x; } }


    override protected void OnStateEntered(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _attackDamage = Context.DefaultAttackDamage;
        _timeAgo = 0f;
        _stateLength = stateInfo.length;
        --Context.RequestedAttackCount;
        Context.AlreadyHitColliders.Clear();
    }

    protected void RequestEnableHitbox(int clipCount, int delayCount) {
        _timeAgo += Time.deltaTime;
        if (_timeAgo >= _stateLength / clipCount * delayCount
            && _timeAgo < _stateLength / clipCount * (delayCount + 1)) {
            Vector2 offset = _hitboxOffset;
            offset.x *= DirX;
            Vector2 hitboxPoint = (Vector2)Transform.position + offset;

            Context.EnableHitbox(hitboxPoint, _hitboxSize, _attackDamage, HitEffectName);
        }
    }
}
