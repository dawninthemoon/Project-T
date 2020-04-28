using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class CharacterAttack : MonoBehaviour
{
    private static readonly string DefaultAttackName = "PlayerEffect/defaultAttack";

    private EffectManager _effectManager;
    private StringBuilder _sb;
    private const float InitalInputDelay = 0.15f;
    private const float InputDelayAfterCombo = 0.15f;
    private float _inputDelay;

    private int _maxAttackCount = 3;

    private int _currentAttackState = 0;

    public bool IsInAttackProgress { get; private set; }

    private CharacterRenderer _characterRenderer;

    public void Initalize() {
        _characterRenderer = GetComponent<CharacterRenderer>();
        _sb = new StringBuilder();
        _effectManager = EffectManager.GetInstance();
    }

    public void Attack(bool attackRequested) {
        _inputDelay -= Time.fixedDeltaTime;

        if (attackRequested && _inputDelay < 0f) {
            _inputDelay = InitalInputDelay;

            _currentAttackState = Mathf.Min(_currentAttackState + 1, _maxAttackCount);
            if(_currentAttackState <= _maxAttackCount) {
                _characterRenderer.SetAttackState(_currentAttackState);
            }
            
            if(_currentAttackState == 1){
                EnableHitbox(0);
                SpawnAttackEffect(1);
            }
        }
    }

    public void AttackEnd(int state) {
        if (state < _currentAttackState){
            int attackType = state + 1;
            EnableHitbox(attackType);
            SpawnAttackEffect(attackType);
        }
        else {
            _inputDelay = InputDelayAfterCombo;
            IsInAttackProgress = false;
            _currentAttackState = 0;
            _characterRenderer.SetAttackState(_currentAttackState);
        }
    }

    private void EnableHitbox(int attackType) {
        IsInAttackProgress = true;
    }

    private void SpawnAttackEffect(int attackType) {
        Vector3 pos = transform.position;
        float dir = transform.localScale.x;

        _sb.Clear();
        _sb.Append(DefaultAttackName);
        _sb.Append(attackType.ToString());

        _effectManager.SpawnAndRemove(pos, _sb.ToString(), dir);
    }
}
