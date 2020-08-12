#pragma warning disable 0649
using UnityEngine;
using System.Collections.Generic;
using Aroma;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private int _defaultAttackDamage = 10;
    [SerializeField] private LayerMask _attackableLayers;
    public int DefaultAttackDamage { get { return _defaultAttackDamage; } }
    public LayerMask AttackableLayers { get { return _attackableLayers; } }
    private const float InitalInputDelay = 0.15f;
    private float _inputDelay;
    private static readonly int MaxAttackCount = 2;
    public bool CanInputNextCombo { get; set; } = true;
    public int CurrentAttackState { get; private set; }
    public bool IsInAttackProgress { get; private set; }
    private PlayerRenderer _characterRenderer;
    public List<Collider2D> AlreadyHitColliders { get; private set; }

    public void Initialize() {
        AlreadyHitColliders = new List<Collider2D>();
        _characterRenderer = GetComponent<PlayerRenderer>();
    }

    public void Progress(bool attackRequested) {
        _inputDelay -= Time.deltaTime;
        
        if (attackRequested && _inputDelay < 0f && CanInputNextCombo) {
            _inputDelay = InitalInputDelay;

            CanInputNextCombo = false;
            CurrentAttackState = (CurrentAttackState % MaxAttackCount) + 1;
            _characterRenderer.SetAttackState(CurrentAttackState);
        }
    }

    public void EnableHitbox(Vector2 position, Vector2 size, int damage, string hitEffectName) {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(position, size, 0f, _attackableLayers);
        bool enemyHit = false;
        for (int i = 0; i < colliders.Length; i++) {
            if (!IsAlreadyExists(colliders[i])) {
                AlreadyHitColliders.Add(colliders[i]);
                EnemyBase enemy = colliders[i].gameObject.GetComponentNoAlloc<EnemyBase>();
                enemyHit = OnEnemyHit(enemy, damage, hitEffectName);
            }
        }
        
        Time.timeScale = enemyHit ? 0f : 1f;
    }

    public void AttackEnd() {
        _inputDelay = InitalInputDelay;
        IsInAttackProgress = false;
        CurrentAttackState = 0;
        _characterRenderer.SetAttackState(CurrentAttackState);
    }

    public void EnterAttackProgress() {
        IsInAttackProgress = true;
    }

    public void SpawnAttackEffect(string effectName, bool effectTracks = false) {
        Vector3 pos = transform.position;
        float dir = transform.localScale.x;

        if (effectTracks) {
            EffectManager.GetInstance().SpawnTrackEffectAndRemove(pos, effectName, transform, dir);
        }
        else {
            EffectManager.GetInstance().SpawnAndRemove(pos, effectName, dir);
        }
    }

    private bool OnEnemyHit(EnemyBase enemy, int damage, string hitEffectName) {
        Vector3 enemyPosition = enemy.transform.position;
        float dirX = transform.localScale.x;
        
        var effectManager = EffectManager.GetInstance();
        if (enemy.ReceiveDamage(damage, dirX)) {
            effectManager.SpawnAndRemove(enemyPosition, hitEffectName, dirX);
            effectManager.ShakeCamera(0.2f);
        }

        return true;
    }

    private bool IsAlreadyExists(Collider2D collider) {
        for (int i = 0; i < AlreadyHitColliders.Count; i++) {
            if (GameObject.ReferenceEquals(collider, AlreadyHitColliders[i]))
                return true;
        }
        return false;
    }
}
