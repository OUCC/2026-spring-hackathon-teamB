using System;
using UnityEngine;

/// <summary>
/// 地面を歩く近接防御ユニット(=足軽)
/// </summary>
public class BasicDefencerUnit : MonoBehaviour, IDamageable, ITarget,IShootable
{
    [SerializeField] protected DefencerUnitData unitData;
    protected float currentHealth;

    private IAttackStrategy _attackStrategy;

    private Action<ITarget> _onDied;
    public Action<ITarget> OnDied { get => _onDied; }

    public virtual void Initialize(IAttackStrategy attackStrategy = null)
    {
        if (unitData == null)
        {
            Debug.LogError("DefencerUnitData is not assigned.");
            return;
        }

        currentHealth = unitData.MaxHealth;


        if (attackStrategy != null)
            _attackStrategy = attackStrategy;
        GameManager.Instance.OnTickEvent += Heal;
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
    public void Heal()
    {
        TryAttack();
        currentHealth = Mathf.Min(currentHealth + unitData.AutoHealAmount, unitData.MaxHealth);
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
        _onDied?.Invoke(this);
    }
}