﻿#pragma warning disable 0649
using UnityEngine;
using System.Collections.Generic;
using Aroma;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private int _defaultAttackDamage = 10;
    [SerializeField] private LayerMask _attackableLayers;
    public int DefaultAttackDamage { get { return _defaultAttackDamage; } }
    public LayerMask AttackableLayers { get { return _attackableLayers; } }

    private static readonly string EffectDirectory = "PlayerEffect/";
    private static readonly string DefaultAttackName = "defaultAttack";

    private EffectManager _effectManager;
    private const float InitalInputDelay = 0.15f;
    private const float InputDelayAfterCombo = 0.15f;
    private float _inputDelay;
    private int _maxAttackCount = 3;
    public int CurrentAttackState { get; private set; }
    public bool IsInAttackProgress { get; private set; }
    private PlayerRenderer _characterRenderer;
    private Character _model;
    public List<Collider2D> AlreadyHitColliders { get; private set; }

    public void Initialize() {
        AlreadyHitColliders = new List<Collider2D>();
        _model = GetComponent<Character>();
        _characterRenderer = GetComponent<PlayerRenderer>();
        _effectManager = EffectManager.GetInstance();
    }

    public void Progress(bool attackRequested) {
        _inputDelay -= Time.deltaTime;

        if (attackRequested && _inputDelay < 0f) {
            _inputDelay = InitalInputDelay;

            CurrentAttackState = Mathf.Min(CurrentAttackState + 1, _maxAttackCount);
            if(CurrentAttackState <= _maxAttackCount) {
                _characterRenderer.SetAttackState(CurrentAttackState);
            }
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
        _inputDelay = InputDelayAfterCombo;
        IsInAttackProgress = false;
        CurrentAttackState = 0;
        _characterRenderer.SetAttackState(CurrentAttackState);
    }

    public void EnterAttackProgress() {
        IsInAttackProgress = true;
    }

    public void SpawnAttackEffect(int attackType) {
        Vector3 pos = transform.position;
        float dir = transform.localScale.x;

        string path = EffectDirectory + DefaultAttackName + attackType.ToString();
        _effectManager.SpawnAndRemove(pos, path, dir);
    }

    public void SpawnAttackEffect(string effectName, bool effectTracks = false) {
        Vector3 pos = transform.position;
        float dir = transform.localScale.x;

        string path = EffectDirectory + effectName;
        if (effectTracks) {
            _effectManager.SpawnTrackEffectAndRemove(pos, path, transform, dir);
        }
        else {
            _effectManager.SpawnAndRemove(pos, path, dir);
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