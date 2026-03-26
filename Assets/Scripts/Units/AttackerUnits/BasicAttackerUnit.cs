using System;

using UnityEngine;

/// <summary>
/// 地面を歩く近接攻撃ユニット(=足軽)
/// </summary>
public class BasicAttackerUnit : MonoBehaviour, IDamageable, ITarget, IMovable
{
    [SerializeField] protected AttackerUnitData unitData;
    protected float currentHealth;

    private IMoveStrategy _moveStrategy;
    private IAttackStrategy _attackStrategy;

    public GameManager.TeamType Team => GameManager.TeamType.Attack;

    public event Action<ITarget> OnDied;

    public virtual void Initialize(AttackerUnitData data, IMoveStrategy moveStrategy = null, IAttackStrategy attackStrategy = null)
    {
        unitData = data;

        currentHealth = unitData.MaxHealth;

        if (moveStrategy != null)
            _moveStrategy = moveStrategy;

        if (attackStrategy != null)
            _attackStrategy = attackStrategy;

        GameManager.Instance.OnTickEvent += Move;
    }

    public void SetMoveStrategy(IMoveStrategy moveStrategy)
    {
        _moveStrategy = moveStrategy;
    }

    public void Move()
    {
#if UNITY_EDITOR
        if (_moveStrategy == null)
        {
            Debug.LogError("Move strategy is not assigned.");
            return;
        }
#endif
        _moveStrategy?.Move(this);
        TryAttack();
    }

    public Vector3 GetTargetPosition()
    {
        return transform.position;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public bool TryAttack()
    {
#if UNITY_EDITOR
        if (_attackStrategy == null)
        {
            Debug.LogError("Attack strategy is not assigned.");
            return false;
        }
#endif
        if (_attackStrategy?.IsAttackAble ?? false)
        {
            _attackStrategy.Attack(this);
            return true;
        }
        return false;
    }

    private ITarget GetNearestTarget()
    {
#if UNITY_EDITOR
        Debug.LogWarning("GetNearestTarget is not implemented yet.");
#endif
        return null;
    }

    public void Die()
    {
        OnDied?.Invoke(this);
        GameManager.Instance.OnTickEvent -= Move;
    }
}