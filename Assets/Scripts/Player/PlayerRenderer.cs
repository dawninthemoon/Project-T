using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRenderer : MonoBehaviour
{
    private const string DirXName = "dirX";
    private const string DirYName = "dirY";
    private const string RequestJumpName = "requestJump";
    private const string RequestedAttackCount = "requestedAttackCount";

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
        _animator.SetInteger(RequestedAttackCount, count);
    }

    public void SetDirection(float dirX) {
        Vector3 scaleVector = new Vector3(Mathf.Sign(dirX), 1f, 1f);
        transform.localScale = scaleVector;
    }
}
