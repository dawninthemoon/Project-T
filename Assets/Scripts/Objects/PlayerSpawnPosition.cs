using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPosition : SpawnPosition
{
    public static readonly float Impossible = -987654321f;
    [SerializeField] private int _index = 0;
    [SerializeField] private int _targetRoomNumber = 0;
    [SerializeField] private int _targetIndex = 0;
    [SerializeField] private Vector2 _spawnPos = Vector2.zero;
    private BoxCollider2D _collider;
    private int _playerMask;

    public Vector3 SpawnPos { 
        get {
            Vector3 pos = transform.position + (Vector3)_spawnPos; 
            if (_spawnPos.y == 0f) pos.y = Impossible;
            if (_spawnPos.x == 0f) pos.x = Impossible;
            return pos;
        }
    }

    public int Index { get { return _index; } }

    public void Initalize() {
        _playerMask = 1 << LayerMask.NameToLayer("Player");
        _collider = GetComponent<BoxCollider2D>();
    }

    public void FixedProgress() {
        Vector2 position = (Vector2)transform.position + _collider.offset;
        if (Physics2D.OverlapBox(position, _collider.size, 0f, _playerMask)) {
            OnCollisionWithPlayer();
        }
    }

    private void OnCollisionWithPlayer() {
        RoomManager.GetInstance().MoveRoom(_targetRoomNumber, _targetIndex);
    }
}
