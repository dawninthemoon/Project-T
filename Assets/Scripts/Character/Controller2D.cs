using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RaycastController
{
    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();

        _collisions.Reset();

        if (velocity.x != 0f)
            HorizontalCollisions(ref velocity);
        if (velocity.y != 0f)
            VerticalCollisions(ref velocity);

        transform.Translate(velocity);
    }
}
