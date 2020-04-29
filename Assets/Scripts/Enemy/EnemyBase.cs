using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] protected float _moveSpeed = 4f;
    [SerializeField] protected string _enemyName = null;
    [SerializeField] protected int _maxHp = 20;
    protected Animator _animator;
    protected int _hp;

    public virtual void Initalize() {
        _animator = GetComponent<Animator>();
        Setup();
    }

    public virtual void Setup() {
        _hp = _maxHp;
    }

    public abstract void GetDamage(int amount);
}
