using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] protected float _moveSpeed;    
    protected int _hp;
}
