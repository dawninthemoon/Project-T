using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

[RequireComponent(typeof(Controller2D))]
public class Character : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 10f;

    private static readonly float GravityPerFrame = -300f;

    private float _inputX;
    private Vector3 _velocity;

    private Controller2D _controller;

    private void Start()
    {
        _controller = GetComponent<Controller2D>();
    }

    private void FixedUpdate()
    {
        _velocity.x = _inputX * _moveSpeed;
        _velocity.y = GravityPerFrame * Time.fixedDeltaTime;
        _controller.Move(_velocity * Time.fixedDeltaTime);
    }

    public void SetInputX(float horizontal)
    {
        _inputX = horizontal;
    }
}
