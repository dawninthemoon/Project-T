using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class InputControl : MonoBehaviour
{
    private class ActionBoolPair {
        public bool WasReleased;
        public PlayerAction Actions;
        public ActionBoolPair(PlayerAction action) {
            WasReleased = true;
            Actions = action;
        }
    }

    private Character _model;

    private MyPlayerActions _myActions;

    private Dictionary<string, ActionBoolPair> _wasPressedAtLastFrame;

    private static readonly string JumpActionName = "Jump";
    private static readonly string AttackActionName = "Attack";

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

        _wasPressedAtLastFrame = new Dictionary<string, ActionBoolPair>();
        _wasPressedAtLastFrame.Add(JumpActionName, new ActionBoolPair(_myActions.Jump));
        _wasPressedAtLastFrame.Add(AttackActionName, new ActionBoolPair(_myActions.Attack));
    }

    public bool GetKeyDown(string actionName) {
        ActionBoolPair pair;
        if (_wasPressedAtLastFrame.TryGetValue(actionName, out pair)) {

            if (!pair.WasReleased) {
                pair.WasReleased = pair.Actions.WasReleased;
            }

            if (pair.Actions.IsPressed) {
                if (pair.WasReleased) {
                    pair.WasReleased = false;
                    return true;
                }
            }
        }

        return false;
    }

    public void Progress() {
        InputKeys();
    }

    private void InputKeys() {
        float horizontal = _myActions.Move.Value;
        horizontal = (horizontal == 0f) ? horizontal : Mathf.Sign(horizontal);
        _model.SetInputX(horizontal);

        _model.SetJump(GetKeyDown(JumpActionName));
        _model.SetAttack(GetKeyDown(AttackActionName));

        //_model.SetJump(_myActions.Jump.WasPressed);
        //_model.SetAttack(_myActions.Attack.WasPressed);
    }
}
