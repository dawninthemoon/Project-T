using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPosition : SpawnPosition
{
    [SerializeField] public EnemyTypes _requestEnemy = EnemyTypes.Enemy_Dummy;
    public EnemyTypes RequestEnemy { get { return _requestEnemy; } }
}
