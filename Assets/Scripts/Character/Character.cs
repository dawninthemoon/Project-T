using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Character : MonoBehaviour
{
    private static readonly float GravityPerFrame = -300f;
    private Vector3 _velocity;

    private Controller2D _controller;

    private void Start()
    {
        _controller = GetComponent<Controller2D>();
    }

    private void FixedUpdate()
    {
        //Vector2 input = new Vector2(Input)

        _velocity.y = GravityPerFrame * Time.fixedDeltaTime;
        _controller.Move(_velocity * Time.fixedDeltaTime);
    }
}
