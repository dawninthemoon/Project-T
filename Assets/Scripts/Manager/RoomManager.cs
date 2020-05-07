using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : SingletonWithMonoBehaviour<RoomManager>
{
    private const string RoomPrefabsPath = "Room";
    private int _currentRoomNumber = 0;
    private RoomInfo[] _rooms;
    
    public void Initalize() {
        MakeAllRooms();
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

            _rooms[targetRoomNumber].StartRoom();

            Vector3 playerPos = _rooms[targetRoomNumber].GetDoorPosition(targetIndex);
            ObjectManager.GetInstance().SetPlayerPos(playerPos);
        }
        else {
            Debug.LogError("Room does not exists");
        }
    }

    private void MakeAllRooms() {
        GameObject[] prefabs = ResourceManager.GetInstance().GetAllPrefabs(RoomPrefabsPath);
        int numOfRooms = prefabs.Length;
        _rooms = new RoomInfo[numOfRooms];

        for (int i = 0; i < numOfRooms; i++) {
            _rooms[i] = Instantiate(prefabs[i]).GetComponent<RoomInfo>();
        }
    }
}
