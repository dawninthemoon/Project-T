using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using System.Linq;

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

    private Player _model;
    private PlayerAnimator _animator;
    private MyPlayerActions _myActions;
    private Dictionary<string, ActionBoolPair> _wasPressedAtLastFrame;
    private bool _isJumpPressed;
    private static readonly string JumpActionName = "Jump";
    private static readonly string AttackActionName = "Attack";
    private static readonly string ThrowActionName = "Throw";
    private PlayerAnimator.States[] _inputIgnoreStates, _moveIgnoreStates;

    public void Initalize(Player character)
    {
        _model = character;
        _animator = _model.GetComponent<PlayerAnimator>();

        _myActions = new MyPlayerActions();

        _myActions.Left.AddDefaultBinding(Key.LeftArrow);
        _myActions.Left.AddDefaultBinding(InputControlType.DPadLeft);
        _myActions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);

        _myActions.Right.AddDefaultBinding(Key.RightArrow);
        _myActions.Right.AddDefaultBinding(InputControlType.DPadRight);
        _myActions.Right.AddDefaultBinding(InputControlType.LeftStickRight);

        _myActions.Up.AddDefaultBinding(Key.UpArrow);
        _myActions.Up.AddDefaultBinding(InputControlType.DPadUp);
        _myActions.Up.AddDefaultBinding(InputControlType.LeftStickUp);

        _myActions.Down.AddDefaultBinding(Key.DownArrow);
        _myActions.Down.AddDefaultBinding(InputControlType.DPadDown);
        _myActions.Down.AddDefaultBinding(InputControlType.LeftStickDown);

        _myActions.Jump.AddDefaultBinding(Key.Z);
        _myActions.Jump.AddDefaultBinding(InputControlType.Action1);

        _myActions.Attack.AddDefaultBinding(Key.X);
        _myActions.Attack.AddDefaultBinding(InputControlType.Action3);

        _myActions.Throw.AddDefaultBinding(Key.A);
        _myActions.Throw.AddDefaultBinding(InputControlType.Action2);

        _wasPressedAtLastFrame = new Dictionary<string, ActionBoolPair>();
        _wasPressedAtLastFrame.Add(JumpActionName, new ActionBoolPair(_myActions.Jump));
        _wasPressedAtLastFrame.Add(AttackActionName, new ActionBoolPair(_myActions.Attack));
        _wasPressedAtLastFrame.Add(ThrowActionName, new ActionBoolPair(_myActions.Throw));
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
        if (CheckCannotInput()) return;


        float horizontal = 0f, vertical = 0f;
        if (!CheckCannotMove()) {
            horizontal = IgnoreSmallValue(_myActions.Horizontal.Value);
            vertical = IgnoreSmallValue(_myActions.Vertical.Value);
        }
            
        _model.SetInputX(horizontal);
        _model.SetInputY(vertical);

        _model.SetJump(GetKeyDown(JumpActionName));
        _model.SetJumpEnd(!_myActions.Jump.IsPressed, _isJumpPressed);
        _model.SetAttack(GetKeyDown(AttackActionName));
        _model.SetThrow(GetKeyDown(ThrowActionName));

        _isJumpPressed = _myActions.Jump.IsPressed;
    }

    private float IgnoreSmallValue(float value) {
        value = (Mathf.Abs(value) > 0.5f) ? value : 0f;
        value = (value == 0f) ? value : Mathf.Sign(value);
        return value;
    }

    private bool CheckCannotInput() {
        var state = _animator.State;

        if (_inputIgnoreStates == null) {
            _inputIgnoreStates = new PlayerAnimator.States[] {
                PlayerAnimator.States.LandHard
            };
        }

        return _inputIgnoreStates.Contains(state);
    }

    private bool CheckCannotMove() {
        var state = _animator.State;

        if (_moveIgnoreStates == null) {
            _moveIgnoreStates = new PlayerAnimator.States[] {
                PlayerAnimator.States.AttackIn,
                PlayerAnimator.States.AttackA,
                PlayerAnimator.States.AttackB,
                PlayerAnimator.States.AttackOut,
                PlayerAnimator.States.AttackAir,
                PlayerAnimator.States.Throw,
                PlayerAnimator.States.ThrowAir,
            };
        }

        return _moveIgnoreStates.Contains(state);
    }
}
