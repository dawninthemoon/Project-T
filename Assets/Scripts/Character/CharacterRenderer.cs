using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRenderer : MonoBehaviour
{
    private const string DirXStateName = "dirX";
    private const string DirYStateName = "dirY";
    private const string RequestJumpStateName = "requestJump";

    private Animator _animator;

    private void Start() {
        _animator = GetComponent<Animator>();
    }

    public void ApplyAnimation(float dirX, float velocityY, bool requestJump) {
        int dirY = 0;
        if (velocityY > 0f)
            dirY = 1;
        else if (velocityY < 0f)
            dirY = -1;

        _animator.SetInteger(DirXStateName, Mathf.RoundToInt(dirX));
        _animator.SetInteger(DirYStateName, dirY);
        _animator.SetBool(RequestJumpStateName, requestJump);

        if(dirX != 0f) {
            SetDirection(dirX);
        }
    }

    public void SetDirection(float dirX) {
        Vector3 scaleVector = new Vector3(dirX, 1f, 1f);
        transform.localScale = scaleVector;
    }
}
