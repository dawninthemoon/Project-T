using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class InputControl : MonoBehaviour
{
    private Character _model;

    private MyPlayerActions _myActions;

    public void Initalize(Character character)
    {
        _model = character;

        _myActions = new MyPlayerActions();

        _myActions.Left.AddDefaultBinding(Key.LeftArrow);
        _myActions.Left.AddDefaultBinding(InputControlType.DPadLeft);
        _myActions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);

        _myActions.Right.AddDefaultBinding(Key.RightArrow);
        _myActions.Right.AddDefaultBinding(InputControlType.DPadRight);
        _myActions.Right.AddDefaultBinding(InputControlType.LeftStickRight);

        _myActions.Jump.AddDefaultBinding(Key.Z);
        _myActions.Jump.AddDefaultBinding(InputControlType.Action1);

        _myActions.Attack.AddDefaultBinding(Key.X);
        _myActions.Attack.AddDefaultBinding(InputControlType.Action3);
    }

    public void Progress() {
        InputKeys();
    }

    private void InputKeys() {
        _model.SetInputX(_myActions.Move.Value);
        _model.SetJump(_myActions.Jump.WasPressed);
        _model.SetAttack(_myActions.Attack.WasPressed);
    }
}
