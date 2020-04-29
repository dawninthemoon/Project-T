#pragma warning disable 0649
using UnityEngine;
using System.Text;

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

    public void Initialize() {
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
}
