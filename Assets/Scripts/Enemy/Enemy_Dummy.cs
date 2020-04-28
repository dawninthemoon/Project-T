using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

public class Enemy_Dummy : EnemyBase
{
    public enum State { Idle, Move, Attack, Hurt, Die }

    private StateMachine<State> _fsm;

    private void Start() {
        Initalize();
    }

    private void Initalize() {

    }
}
