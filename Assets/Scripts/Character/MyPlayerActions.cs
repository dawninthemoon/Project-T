using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class MyPlayerActions : PlayerActionSet
{
    public PlayerAction Left { get; set; }
    public PlayerAction Right { get; set; }
    public PlayerAction Jump { get; set; }
    public PlayerOneAxisAction Move { get; set; }

    public MyPlayerActions()
    {
        Left = CreatePlayerAction("Move Left");
        Right = CreatePlayerAction("Move Right");
        Jump = CreatePlayerAction("Jump");
        Move = CreateOneAxisPlayerAction(Left, Right);
    }
}
