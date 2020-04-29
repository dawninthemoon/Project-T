using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

public class AttackStateBase : StateMachineBehaviour<CharacterAttack>
{
    protected float _timeAgo = 0f;
    protected float _stateLength = 0f;

    private const string HitEffectName = "PlayerEffect/meleeAttack_hit";
    private static readonly Vector2 _hitboxOffset = new Vector2(0.4f, 0.75f);
    private static readonly Vector2 _hitboxSize = new Vector2(3f, 1.5f);

    private int _attackDamage;

    private float DirX { get { return Transform.localScale.x; } }

    private List<Collider2D> _alreadyHitColliders = new List<Collider2D>();

    override protected void OnStateEntered(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   
        _attackDamage = Context.DefaultAttackDamage;
        _timeAgo = 0f;
        _stateLength = stateInfo.length;
        _alreadyHitColliders.Clear();
    }

    protected void RequestEnableHitbox(int attackType, int clipCount) {
        _timeAgo += Time.deltaTime;
        if (_timeAgo > _stateLength / clipCount * 2f) {
            EnableHitbox();
        }
    }

    private void EnableHitbox() {
        Vector2 offset = _hitboxOffset * DirX;
        Vector2 hitboxPoint = (Vector2)Transform.position + offset;
        LayerMask attackableLayers = Context.AttackableLayers;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(hitboxPoint, _hitboxSize, 0f, attackableLayers);

        bool enemyHit = false;
        for (int i = 0; i < colliders.Length; i++) {
            if (!IsAlreadyExists(colliders[i])) {
                _alreadyHitColliders.Add(colliders[i]);
                enemyHit = OnEnemyHit(colliders[i].gameObject.GetComponentNoAlloc<EnemyBase>());
            }
        }
        
        Time.timeScale = enemyHit ? 0f : 1f;
    }

    private bool OnEnemyHit(EnemyBase enemy) {
        Vector3 enemyPosition = enemy.transform.position;

        EffectManager.GetInstance().SpawnAndRemove(enemyPosition, HitEffectName, DirX);
        enemy.GetDamage(_attackDamage);

        return true;
    }

    private bool IsAlreadyExists(Collider2D collider) {
        for (int i = 0; i < _alreadyHitColliders.Count; i++) {
            if (GameObject.ReferenceEquals(collider, _alreadyHitColliders[i]))
                return true;
        }
        return false;
    }
}
