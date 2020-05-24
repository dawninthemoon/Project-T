using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManager : SingletonWithMonoBehaviour<RoomManager>
{
    private int _currentRoomNumber = 0;
    private RoomInfo[] _rooms;
    private Tilemap _currentTilemap;
    
    public void Initalize() {
        _currentTilemap = GameObject.Find("Grid").GetComponentInChildren<Tilemap>();
        
        MakeAllRooms();

        System.Array.Sort(
            _rooms,
             (RoomInfo val1, RoomInfo val2) => val1.RoomNumber.CompareTo(val2.RoomNumber));

        _rooms[_currentRoomNumber].StartRoom(_currentTilemap);
    }

    public void FixedProgress() {
        _rooms[_currentRoomNumber].FixedProgress();
    }

    public void MoveRoom(int targetRoomNumber, int targetIndex) {
        if (targetRoomNumber < _rooms.Length) {
            _rooms[_currentRoomNumber].ResetRoom();
            _currentRoomNumber = targetRoomNumber;

            _rooms[targetRoomNumber].StartRoom(_currentTilemap);

            Vector3 playerPos = _rooms[targetRoomNumber].GetDoorPosition(targetIndex);
            ObjectManager.GetInstance().SetPlayerPos(playerPos);
        }
        else {
            Debug.LogError("Room does not exists");
        }
    }

    private void MakeAllRooms() {
        RoomBase[] roomBases = ResourceManager.GetInstance().GetAllRoomBases();
        int numOfRooms = roomBases.Length;
        _rooms = new RoomInfo[numOfRooms];

        for (int i = 0; i < numOfRooms; i++) {
            _rooms[i] = new RoomInfo(roomBases[i]);
        }
    }
}
