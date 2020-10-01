#pragma warning disable 0649
using UnityEngine;
using System.Collections.Generic;
using Aroma;

public class PlayerAttack : MonoBehaviour
{
    #region non reference fields
    public static readonly float DefaultChargeTime = 1f;
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
    public bool CanThrow { get; set; }
    public float ChargeTime { get; set; }
    private Vector3 _throwPosition;
    private Vector2 _meleeAttackOffset;
    private Vector2 _meleeAttackSize;
    private int _meleeAttackDamage;
    public bool Charged { get; set; }
    private static readonly string HitEffectName = "EFFECT_Hit";
    private static readonly string[] FireEffectName = { "EFFECT_Fire", "EFFECT_Explode_Flame" };
    private static readonly string[] ElectricEffectName = { "EFFECT_Electric", "EFFECT_Field_Electric" };
    private static readonly string[] IceEffectName = { "EFFECT_Snowflake", "EFFECT_Snowflake" };
    private string[] CurrentEffectName;


    #endregion
    [SerializeField] private Talisman _talismanPrefab = null;
    private List<Talisman> _activeTalismans;

    public List<Collider2D> AlreadyHitColliders { get; private set; }

    public void Initialize() {
        _activeTalismans = new List<Talisman>(5);
        AlreadyHitColliders = new List<Collider2D>();
        ChargeTime = DefaultChargeTime;

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
        for (int i = 0; i < _activeTalismans.Count; ++i) {
            var talisman = _activeTalismans[i];
            if (!talisman.MoveSelf()) {
                float dir = talisman.transform.localScale.x;
                var enemy = talisman.GetHitEnemy();
                if (enemy != null) {
                    var et = enemy.transform;

                    if (talisman.Charged && (talisman.Type == Talisman.TalismanType.Fire || talisman.Type == Talisman.TalismanType.Electric)) {
                        if (talisman.Type == Talisman.TalismanType.Fire) {
                            if (talisman.GetHitCollider(1.5f)) {
                                enemy.ReceiveDamage(2, dir, true);
                            }
                        }
                    }
                    else {
                        enemy.ReceiveDamage(1, dir, talisman.Charged);
                    }

                    if (talisman.Type != Talisman.TalismanType.Normal) {
                        int effectIndex = (talisman.Charged) ? 1 : 0;
                        if (talisman.Type == Talisman.TalismanType.Ice) {
                            enemy.StartFreeze(2f);
                        }
                        else if ((talisman.Type != Talisman.TalismanType.Electric) || !talisman.Charged) {
                            EffectManager.GetInstance().SpawnTrackEffectAndRemove(et.position, CurrentEffectName[effectIndex], et, dir);
                            Invoke("IncreaseTalisman", 0.5f);
                        }
                        else {
                            SpriteAtlasAnimator.OnAnimationEnd onEnd = IncreaseTalisman;
                            onEnd += () => { Destroy(talisman); };
                            System.Action action = () => {
                                if (talisman.GetHitCollider(2.5f)) {
                                    enemy.RequestReceiveDamage(1f / 60f, dir, true);
                                }
                            };
                            EffectManager.GetInstance().SpawnEffectWithDuration(et.position, CurrentEffectName[effectIndex], 3f, action, onEnd);
                        }
                    }
                }

                _activeTalismans.RemoveAt(i--);
                talisman.gameObject.SetActive(false);

                if (talisman.Type != Talisman.TalismanType.Electric && !talisman.Charged)
                    Destroy(talisman);
            }
        }
    }

    private void IncreaseTalisman() => ++TalismanCount;

    public void EnableMeleeAttack() {
        Vector2 position = transform.position;
        Vector2 offset = _meleeAttackOffset.ChangeXPos(_meleeAttackOffset.x * transform.localScale.x);
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
        --TalismanCount;
        Charged = ChargeTime < 0f;
        ChargeTime = DefaultChargeTime;

        var talisman = Instantiate(_talismanPrefab);
        float dirX = transform.localScale.x;

        talisman.transform.position = transform.position + _throwPosition;

        Talisman.TalismanType type = (CurrentEffectName == FireEffectName) ? Talisman.TalismanType.Fire : ((CurrentEffectName == IceEffectName) ? Talisman.TalismanType.Ice : Talisman.TalismanType.Electric);
        type = (CurrentEffectName == null) ? Talisman.TalismanType.Normal : type;
        talisman.Initalize(dirX, 10f, Charged, type);
        _activeTalismans.Add(talisman);
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

    private void OnGUI() {
        if (GUI.Button(new Rect(20, 20, 200, 60), "Fire")) {
            CurrentEffectName = FireEffectName;
        }
        if (GUI.Button(new Rect(230, 20, 200, 60), "Electric")) {
            CurrentEffectName = ElectricEffectName;
        }
        if (GUI.Button(new Rect(440, 20, 200, 60), "Ice")) {
            CurrentEffectName = IceEffectName;
        }

        GUI.Label(new Rect(10, 100, 200, 40), ("Remain: " + TalismanCount.ToString()));
    }
}
