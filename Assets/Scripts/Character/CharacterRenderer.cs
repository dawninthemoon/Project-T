using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRenderer : MonoBehaviour
{
    private const string DirXName = "dirX";
    private const string DirYName = "dirY";
    private const string RequestJumpName = "requestJump";
    private const string AttackStateName = "attackState";

    private Animator _animator;

    public void Initalize() {
        _animator = GetComponent<Animator>();
    }

    public void ApplyAnimation(float dirX, float velocityY, bool requestJump) {
        int dirY = 0;
        if (velocityY > 0f)
            dirY = 1;
        else if (velocityY < 0f)
            dirY = -1;

        _animator.SetInteger(DirXName, Mathf.RoundToInt(dirX));
        _animator.SetInteger(DirYName, dirY);
        _animator.SetBool(RequestJumpName, requestJump);

        if(dirX != 0f) {
            SetDirection(dirX);
        }
    }

    public void SetAttackState(int state){
        _animator.SetInteger(AttackStateName, state);
    }

    public void SetDirection(float dirX) {
        Vector3 scaleVector = new Vector3(Mathf.Sign(dirX), 1f, 1f);
        transform.localScale = scaleVector;
    }
}
