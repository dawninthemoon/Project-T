using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Only For Editor </summary> ///
public class PlayerSpawnPosition : SpawnPosition
{
    public int _index = 0;
    public int _targetRoomNumber = 0;
    public int _targetIndex = 0;
    public Vector2 _spawnPos = Vector2.zero;
    [HideInInspector] public BoxCollider2D _collider;
}
