using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private Character _model = null;

    private MyPlayerActions _myActions;

    private bool _jumpReleased = true;
    private bool _attackReleased = true;

    private void Start()
    {
        Initalize();
    }

    private void Initalize()
    {
        _myActions = new MyPlayerActions();

        _myActions.Left.AddDefaultBinding(Key.LeftArrow);
        _myActions.Left.AddDefaultBinding(InputControlType.DPadLeft);

        _myActions.Right.AddDefaultBinding(Key.RightArrow);
        _myActions.Right.AddDefaultBinding(InputControlType.DPadRight);

        _myActions.Jump.AddDefaultBinding(Key.Z);
        _myActions.Jump.AddDefaultBinding(InputControlType.Action1);

        _myActions.Attack.AddDefaultBinding(Key.X);
        _myActions.Attack.AddDefaultBinding(InputControlType.Action3);
    }

    private void Update()
    {
        InputKeys();
    }


    private void CheckKeyReleases(ref bool jumpKeyDown, ref bool attackKeyDown) {
        if (_jumpReleased) {
            if (_myActions.Jump.IsPressed)
                _jumpReleased = false;
            jumpKeyDown = _myActions.Jump.IsPressed;
        }
        else {
            _jumpReleased = _myActions.Jump.WasReleased;
        }

        if (_attackReleased) {
            if (_myActions.Attack.IsPressed)
                _attackReleased = false;
            attackKeyDown = _myActions.Attack.IsPressed;
        }
        else {
            _attackReleased = _myActions.Attack.WasReleased;
        }
    }

    private void InputKeys() {

        bool jumpKeyDown = false;
        bool attackKeyDown = false;

        CheckKeyReleases(ref jumpKeyDown, ref attackKeyDown);
        
        _model.SetInputX(_myActions.Move.Value);
        _model.SetJump(jumpKeyDown);
        _model.SetAttack(attackKeyDown);
    }
}
