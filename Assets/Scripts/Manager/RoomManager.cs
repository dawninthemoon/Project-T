using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cinemachine;

public class RoomManager : SingletonWithMonoBehaviour<RoomManager>
{
    private int _currentRoomNumber = 0;
    private RoomInfo[] _rooms;
    private Tilemap _currentTilemap;
    public PolygonCollider2D CameraClampCollider { get; private set;}
    private CinemachineConfiner _confiner;
    
    public void Initalize(CinemachineConfiner confiner) {
        CameraClampCollider = new GameObject().AddComponent<PolygonCollider2D>();
        _confiner = confiner;
        _confiner.m_BoundingShape2D = CameraClampCollider;

        _currentTilemap = GameObject.Find("Grid").GetComponentInChildren<Tilemap>();
        
        MakeAllRooms();

        System.Array.Sort(
            _rooms,
             (RoomInfo val1, RoomInfo val2) => val1.RoomNumber.CompareTo(val2.RoomNumber));
    }

    public void FixedProgress() {
        _rooms[_currentRoomNumber].FixedProgress();
    }

    public void MoveRoom(Vector3 offset, int targetRoomNumber, int targetIndex) {
        if (targetRoomNumber < _rooms.Length) {
            _rooms[_currentRoomNumber].ResetRoom();
            _currentRoomNumber = targetRoomNumber;

            _rooms[targetRoomNumber].StartRoom(_currentTilemap);

            Vector3 playerPos = _rooms[targetRoomNumber].GetDoorPosition(targetIndex) + offset;
            bool vertical = _rooms[targetRoomNumber].IsDoorVertical(targetIndex);
            ObjectManager.GetInstance().SetPlayerPos(playerPos, vertical);

            CameraClampCollider.SetPath(0, _rooms[targetRoomNumber].GetColliderPath(targetIndex));
            _confiner.InvalidatePathCache();
        }
        else {
            Debug.LogError("Room does not exists");
        }
    }

    private void MakeAllRooms() {
        SORoomBase[] roomBases = AssetLoader.GetInstance().GetAllRoomBases();
        int numOfRooms = roomBases.Length;
        _rooms = new RoomInfo[numOfRooms];

        for (int i = 0; i < numOfRooms; i++) {
            _rooms[i] = new RoomInfo(roomBases[i]);
        }
    }
}
