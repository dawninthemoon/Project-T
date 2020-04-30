using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : SingletonWithMonoBehaviour<RoomManager>
{
    private int _currentRoomNumber = 0;
    private RoomInfo[] _rooms;
    
    public void Initalize() {
        _rooms = GetComponentsInChildren<RoomInfo>(true);

        for (int i = 0; i < _rooms.Length; i++) {
            _rooms[i].Initalize();
        }
        System.Array.Sort(
            _rooms,
             (RoomInfo val1, RoomInfo val2) => val1.RoomNumber.CompareTo(val2.RoomNumber));

        _rooms[_currentRoomNumber].StartRoom();
    }

    public void MoveRoom(int targetRoomNumber, int targetIndex) {
        if (targetRoomNumber < _rooms.Length) {
            _rooms[_currentRoomNumber].ResetRoom();
            _currentRoomNumber = targetRoomNumber;

            Vector3 playerPos = _rooms[targetRoomNumber].GetDoorPosition(targetIndex);
            ObjectManager.GetInstance().SetPlayerPos(playerPos);

            _rooms[targetRoomNumber].StartRoom();
        }
        else {
            Debug.LogError("Room does not exists");
        }
    }
}
