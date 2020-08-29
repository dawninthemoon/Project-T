#pragma warning disable 0649
using UnityEngine;
using System.Collections.Generic;
using Aroma;

public class PlayerAttack : MonoBehaviour
{
    #region non reference fields
    [SerializeField] private int _defaultAttackDamage = 10;
    [SerializeField] private LayerMask _attackableLayers;
    public int DefaultAttackDamage { get { return _defaultAttackDamage; } }
    public LayerMask AttackableLayers { get { return _attackableLayers; } }
    private static readonly float InitalInputDelay = 0.15f;
    private static readonly float TalismanInputDelay = 0.4f;
    private static readonly float InputDelayAfterCombo = 0.05f;
    private float _inputDelay;
    private static readonly int MaxRequestCount = 1;
    public int RequestedAttackCount { get; set; }
    public bool RequestThrow { get; set; }
    public int TalismanCount { get; set; } = 5;
    public bool IsInAttackProgress { get; private set; }
    private Vector3 _throwPosition;

    private Vector2 _meleeAttackOffset;
    private Vector2 _meleeAttackSize;
    private int _meleeAttackDamage;
    private static readonly string HitEffectName = "meleeAttack_hit";

    #endregion
    
    private List<Talisman> _activeTalismans;
    public List<Collider2D> AlreadyHitColliders { get; private set; }

    public void Initialize() {
        _activeTalismans = new List<Talisman>(5);
        AlreadyHitColliders = new List<Collider2D>();

        var status = GetComponent<TBLPlayerStatus>();
        _throwPosition = status.throwOffset;
        _meleeAttackOffset = status.meleeAttackOffset;
        _meleeAttackSize = status.meleeAttackSize;
        _meleeAttackDamage = status.meleeAttackDamage;
    }

    public void Progress(bool attackRequested, bool throwRequested) {
        _inputDelay -= Time.deltaTime;
        
        if (attackRequested && _inputDelay < 0f) {
            _inputDelay = InputDelayAfterCombo;
            RequestedAttackCount = Mathf.Min(RequestedAttackCount + 1, MaxRequestCount);
        }

        if (throwRequested && _inputDelay < 0f && TalismanCount > 0) {
            _inputDelay = TalismanInputDelay;
            RequestThrow = true;
        }
    }

    public void FixedProgress() {
        foreach (var talisman in _activeTalismans) {
            talisman.FixedProgress();
        }
    }

    public void EnableMeleeAttack() {
        Vector2 position = transform.position;
        Vector2 offset = _meleeAttackOffset * transform.localScale.x;
        EnableHitbox(position + offset, _meleeAttackSize, _meleeAttackDamage, HitEffectName);
    }

    private void EnableHitbox(Vector2 position, Vector2 size, int damage, string hitEffectName) {
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

    public void ThrowTalisman() {
        var talisman = ObjectManager.GetInstance().GetTalisman();
        float dirX = transform.localScale.x;

        talisman.transform.position = transform.position + _throwPosition;
        //var table = TBLTalisman.GetEntity(new BansheeGz.BGDatabase.BGId("44Zgd9sxt0eyEdxw8zsBJQ"));

        //talisman.Initalize(dirX, table.moveSpeed);
        //_activeTalismans.Add(talisman);
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
