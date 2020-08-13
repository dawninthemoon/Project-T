using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRenderer : MonoBehaviour
{
    private static readonly string DirXName = "dirX";
    private static readonly string DirYName = "dirY";
    private static readonly string RequestJumpName = "requestJump";
    private static readonly string RequestedAttackCountName = "requestedAttackCount";
    private static readonly string RequestedThrowCountName = "requestedThrowCount";

    private Animator _animator;

    public void Initialize() {
        _animator = GetComponent<Animator>();
    }

    public void ApplyAnimation(float dirX, float velocityY, bool requestJump) {
        int dirY = (velocityY > 0f) ? 1 : ((velocityY < 0f) ? -1 : 0);

        _animator.SetInteger(DirXName, Mathf.RoundToInt(dirX));
        _animator.SetInteger(DirYName, dirY);

        if (requestJump)
            _animator.SetTrigger(RequestJumpName);

        if(dirX != 0f) {
            SetDirection(dirX);
        }
    }

    public void RequestAttack(int count){
        _animator.SetInteger(RequestedAttackCountName, count);
    }

    public void RequestThrow(int count) {
        _animator.SetInteger(RequestedThrowCountName, count);
    }

    public void SetDirection(float dirX) {
        Vector3 scaleVector = new Vector3(Mathf.Sign(dirX), 1f, 1f);
        transform.localScale = scaleVector;
    }
}
