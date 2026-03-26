using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

/// <summary>
/// 地面を歩く近接防御ユニット(=足軽)
/// </summary>
public class BasicDefencerUnit : MonoBehaviour, IDamageable, ITarget, IShootable
{
    [SerializeField] protected DefencerUnitData unitData;
    [Header("Shoot Settings")]
    [SerializeField] private Arrow arrowPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 1.0f;
    protected float currentHealth;

    private IAttackStrategy _attackStrategy;

    public event Action<ITarget> OnDied;
    public Arrow ArrowPrefab => arrowPrefab;
    public Transform FirePoint => firePoint != null ? firePoint : transform;
    public float ProjectileSpeed => projectileSpeed;

    public bool IsWall => unitData.IsWall;

    public GameManager.TeamType Team => GameManager.TeamType.Defense;

    public virtual void Initialize(DefencerUnitData data, IAttackStrategy attackStrategy = null)
    {
        unitData = data;
        if (unitData == null)
        {
            Debug.LogError("DefencerUnitData is not assigned.");
            return;
        }

        currentHealth = unitData.MaxHealth;

        if (attackStrategy != null)
            _attackStrategy = attackStrategy;
        GameManager.Instance.OnTickEvent += OnTick;
    }
    private void OnTick()
    {
        TryAttack();
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
    public Arrow Shoot(Vector3 target, int damageAmount, int attackCount = 1)
    {
        if (arrowPrefab == null)
        {
            Debug.LogWarning("Arrow prefab is not assigned.");
            return null;
        }

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;

        Arrow arrow = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
        arrow.Initialize(this, target, projectileSpeed, damageAmount, attackCount);
        return arrow;
    }
}