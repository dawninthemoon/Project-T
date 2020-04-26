using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private List<InputSettings> _inputSettingsList;

    public InputSettings LeftMoveInput { get; set; }

    public Vector2 DirectionalInput;

    private void Start()
    {
        Initalize();
    }

    private void Initalize()
    {
        _inputSettingsList = new List<InputSettings>();
        _inputSettingsList.Add(LeftMoveInput = new InputSettings(KeyCode.LeftArrow, MoveLeft, null, null));
    }

    private void Update()
    {
        InputKeys();
    }

    private void InputKeys()
    {
        foreach (var setting in _inputSettingsList)
        {
            if ((setting.OnKey != null) && Input.GetKey(setting.key))
            {
                setting.OnKey.Invoke();
            }

            if ((setting.OnKeyDown != null) && Input.GetKey(setting.key))
            {
                setting.OnKeyDown.Invoke();
            }

            if ((setting.OnKeyUp != null) && Input.GetKey(setting.key))
            {
                setting.OnKeyUp.Invoke();
            }
        }
    }

    private void MoveLeft() => DirectionalInput.x = -1f;
    private void MoveRight() => DirectionalInput.x = 1f;

    public class InputSettings 
    {
        public delegate void KeyInputCallback();

        public KeyCode key;
        public KeyInputCallback OnKey;
        public KeyInputCallback OnKeyDown;
        public KeyInputCallback OnKeyUp;

        public InputSettings(KeyCode code, KeyInputCallback onKey, KeyInputCallback onKeyDown, KeyInputCallback onKeyUp)
        {
            key = code;
            OnKey = onKey;
            OnKeyDown = onKeyDown;
            OnKeyUp = onKeyUp;
        }
    }
}
