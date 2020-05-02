using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    private InputControl _inputControl;

    private Player _character;
    
    private EffectManager _effectManager;
    private ObjectManager _objectManager;
    private RoomManager _roomManager;

    private void Awake() {
        _effectManager = EffectManager.GetInstance();
        _objectManager = ObjectManager.GetInstance();
        _roomManager = RoomManager.GetInstance();

        _inputControl = GetComponent<InputControl>();
    }

    private void Start() {
        ResourceManager.GetInstance().Initialize();
        _objectManager.Initialize();
        _effectManager.Initialize();
        _roomManager.Initalize();

        Vector3 initalPos = new Vector3(-2f, -1f);
        _character = _objectManager.CreatePlayer(initalPos);
        _character.Initialize();

        _inputControl.Initalize(_character);
    }

    private void Update() {
        _inputControl.Progress();
        _effectManager.Progress();
        _character.Progress();
    }

    private void FixedUpdate() {
        _character.FixedProgress();
        _objectManager.FixedProgress();
    }
}
