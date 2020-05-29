using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Only For Editor </summary> ///
public class PlayerSpawnPosition : MonoBehaviour
{
    public int index = 0;
    public int targetRoomNumber = 0;
    public int targetIndex = 0;
    public bool vertical = false;

    public Vector2 GetColliderOffset() {
        Vector2 size = transform.localScale;
        Vector2 offset = GetComponent<BoxCollider2D>().offset;

        if (size.x > 1f) {
            offset.x += (size.x - 1f) / 2f;
        }
        if (size.y > 1f) {
            offset.y += (size.y - 1f) / 2f;
        }

        return offset;
    }

    public Vector2 GetColliderSize() {
        Vector2 size = transform.localScale;
        Vector2 originSize = GetComponent<BoxCollider2D>().size;

        originSize.x *= size.x;
        originSize.y *= size.y;

        return originSize;
    }
}
