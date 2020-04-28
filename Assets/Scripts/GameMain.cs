using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    private InputControl _inputControl;

    private Character _character;
    
    private EffectManager _effectManager;
    private ObjectManager _objectManager;

    private void Awake() {
        _effectManager = EffectManager.GetInstance();
        _objectManager = ObjectManager.GetInstance();

        _inputControl = GetComponent<InputControl>();
    }

    private void Start() {
        _objectManager.Initalize();
        _effectManager.Initalize();

        Vector3 initalPos = new Vector3(0f, -1f);
        _character = _objectManager.CreateCharacter(initalPos);
        _character.Initalize();

        _inputControl.Initalize(_character);
    }

    private void Update() {
        _inputControl.Progress();
        _effectManager.Progress();
    }

    private void FixedUpdate() {
        _character.FixedProgress();
    }
}
