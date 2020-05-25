using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    [SerializeField] private int _startRoomNumber = 0;
    [SerializeField] private int _targetDoorIndex = 0;

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

        _character = _objectManager.CreatePlayer(Vector2.zero);
        _character.Initialize();

        _roomManager.MoveRoom(_startRoomNumber, _targetDoorIndex);

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
        _roomManager.FixedProgress();
    }
}
