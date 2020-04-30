using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPosition : SpawnPosition
{
    [SerializeField] private int _index = 0;
    [SerializeField] private int _targetRoomNumber = 0;
    [SerializeField] private int _targetIndex = 0;
    [SerializeField] private Vector2 _spawnPos = Vector2.zero;

    public Vector3 SpawnPos { get { return transform.position + (Vector3)_spawnPos; }}

    public int Index { get { return _index; } }

    private void OnTriggerEnter2D(Collider2D collider) {
        RoomManager.GetInstance().MoveRoom(_targetRoomNumber, _targetIndex);
    }
}
