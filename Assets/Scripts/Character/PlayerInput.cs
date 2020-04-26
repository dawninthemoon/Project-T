using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private Character _model = null;

    private MyPlayerActions _myActions;

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
        _myActions.Jump.AddDefaultBinding(InputControlType.DPadX);
    }

    private void Update()
    {
        _model.SetInputX(_myActions.Move.Value);
        _model.SetJump(_myActions.Jump.WasPressed);
    }
}
