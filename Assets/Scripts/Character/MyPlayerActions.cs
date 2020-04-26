using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class MyPlayerActions : PlayerActionSet
{
    public PlayerAction Left { get; set; }
    public PlayerAction Right { get; set; }
    public PlayerAction Jump { get; set; }
    public PlayerAction Attack { get; set; }
    public PlayerOneAxisAction Move { get; set; }

    public MyPlayerActions()
    {
        Left = CreatePlayerAction("Move Left");
        Right = CreatePlayerAction("Move Right");
        Jump = CreatePlayerAction("Jump");
        Attack = CreatePlayerAction("Attack");
        Move = CreateOneAxisPlayerAction(Left, Right);
    }
}
