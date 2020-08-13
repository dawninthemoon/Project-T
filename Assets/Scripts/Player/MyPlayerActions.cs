using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class MyPlayerActions : PlayerActionSet
{
    public PlayerAction Left { get; set; }
    public PlayerAction Right { get; set; }
    public PlayerAction Up { get; set; }
    public PlayerAction Down { get; set; }
    public PlayerAction Jump { get; set; }
    public PlayerAction Attack { get; set; }
    public PlayerAction Throw { get; set; }
    public PlayerOneAxisAction Horizontal { get; set; }
    public PlayerOneAxisAction Vertical { get; set; }

    public MyPlayerActions()
    {
        Left = CreatePlayerAction("Move Left");
        Right = CreatePlayerAction("Move Right");
        Up = CreatePlayerAction("Input Up");
        Down = CreatePlayerAction("Input Down");
        Jump = CreatePlayerAction("Jump");
        Attack = CreatePlayerAction("Attack");
        Throw = CreatePlayerAction("Throw");
        Horizontal = CreateOneAxisPlayerAction(Left, Right);
        Vertical = CreateOneAxisPlayerAction(Down, Up);
    }
}
