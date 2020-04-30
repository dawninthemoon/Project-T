#pragma warning disable 0649
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Aroma;

public class CharacterAttack : MonoBehaviour
{
    [SerializeField] private int _defaultAttackDamage = 10;
    [SerializeField] private LayerMask _attackableLayers;
    public int DefaultAttackDamage { get { return _defaultAttackDamage; } }
    public LayerMask AttackableLayers { get { return _attackableLayers; } }

    private static readonly string DefaultAttackName = "PlayerEffect/defaultAttack";

    private EffectManager _effectManager;
    private StringBuilder _sb;
    private const float InitalInputDelay = 0.15f;
    private const float InputDelayAfterCombo = 0.15f;
    private float _inputDelay;
    private int _maxAttackCount = 3;
    public int CurrentAttackState { get; private set; }
    public bool IsInAttackProgress { get; private set; }
    private CharacterRenderer _characterRenderer;
    public List<Collider2D> AlreadyHitColliders { get; private set; }

    public void Initialize() {
        AlreadyHitColliders = new List<Collider2D>();
        _characterRenderer = GetComponent<CharacterRenderer>();
        _sb = new StringBuilder();
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

        _sb.Clear();
        _sb.Append(DefaultAttackName);
        _sb.Append(attackType.ToString());

        _effectManager.SpawnAndRemove(pos, _sb.ToString(), dir);
    }

    private bool OnEnemyHit(EnemyBase enemy, int damage, string hitEffectName) {
        Vector3 enemyPosition = enemy.transform.position;
        float dirX = transform.localScale.x;
        
        if (enemy.RecieveDamage(damage, dirX)) {
            EffectManager.GetInstance().SpawnAndRemove(enemyPosition, hitEffectName, dirX);
            EffectManager.GetInstance().ShakeCamera(0.2f);
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
