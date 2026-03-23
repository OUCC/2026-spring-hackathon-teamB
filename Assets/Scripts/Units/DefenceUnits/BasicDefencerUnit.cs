using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 地面を歩く近接防御ユニット(=足軽)
/// </summary>
public class BasicDefencerUnit : MonoBehaviour, IDamageable, ITarget,IShootable
{
    [SerializeField] protected DefencerUnitData unitData;
    protected float currentHealth;

    private IAttackStrategy _attackStrategy;

    public event Action<ITarget> OnDied;
    private void Start()
    {
        Initialize(new AttackAround(
            unitData.AttackCoolTime,
            unitData.AttackRange,
            unitData.AttackDamage
        ));
    }
    public virtual void Initialize(IAttackStrategy attackStrategy = null)
    {
        if (unitData == null)
        {
            Debug.LogError("DefencerUnitData is not assigned.");
            return;
        }

        currentHealth = unitData.MaxHealth;
        Debug.Log("a");

        if (attackStrategy != null)
            _attackStrategy = attackStrategy;
        Debug.Log("b");
        GameManager.Instance.OnTickEvent += OnTick;
        Debug.Log("c");
    }
    private void OnTick()
    {

        _attackStrategy?.TickCooldown();
        var targets = GetAttackTargets();
        _attackStrategy?.TargetFiler(targets, 1);

        bool attacked=TryAttack();

        currentHealth = Mathf.Min(currentHealth + unitData.AutoHealAmount, unitData.MaxHealth);
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
        if (_attackStrategy.IsAttackAble)
        {
            _attackStrategy.Attack(this);
            return true;
        }

#endif
        if (_attackStrategy?.IsAttackAble ?? false)
        {
            _attackStrategy.Attack(this);
            return true;
        }
        return false;
    }
    private IEnumerable<ITarget> GetAttackTargets()
    {
        //敵味方の判別はついてなさそう
        var targets = FindObjectsOfType<MonoBehaviour>()
            .OfType<ITarget>()
            .Where(t => t != this)
            .ToList();

        Debug.Log($"[BasicDefencerUnit] target count = {targets.Count}");

        return targets;
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
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTickEvent -= OnTick;
        }
    }
}