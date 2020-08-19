using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

public class PlayerAnimator : SpriteAtlasAnimator
{

    public override void Initalize(string prefix, string idleStateName, bool loop = false) {
        base.Initalize(prefix, idleStateName, loop);
    }
}
