using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    private const float InitalInputDelay = 0.15f;
    private const float InputDelayAfterCombo = 0.2f;
    private float _inputDelay;

    private int _maxAttackCount = 3;

    private int _currentAttackState = 0;

    public bool IsInAttackProgress { get; private set; }

    private CharacterRenderer _characterRenderer;

    private void Start() {
        _characterRenderer = GetComponent<CharacterRenderer>();
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
            }
        }
    }

    public void AttackEnd(int state) {
        if (state < _currentAttackState){
            EnableHitbox(state + 1);
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
}
