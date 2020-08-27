using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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
        var virtualCamera = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();

        AssetLoader.GetInstance().Initalize();
        _objectManager.Initialize();
        _effectManager.Initialize();
        _roomManager.Initalize();
        virtualCamera.GetComponent<CinemachineConfiner>().m_BoundingShape2D = _roomManager.CameraClampCollider;

        _character = _objectManager.CreatePlayer(Vector2.zero);
        _character.Initialize();
        virtualCamera.LookAt = _character.transform;
        virtualCamera.Follow = _character.transform;

        _roomManager.MoveRoom(Vector2.zero, _startRoomNumber, _targetDoorIndex);

        _inputControl.Initalize(_character);
    }

    private void Update() {
        _inputControl.Progress();
        _effectManager.Progress();
        _character.Progress();
        _objectManager.Progress();
    }

    private void FixedUpdate() {
        _character.FixedProgress();
        _objectManager.FixedProgress();
        _roomManager.FixedProgress();
    }
}
