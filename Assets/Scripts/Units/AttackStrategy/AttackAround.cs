using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class AttackAround : IAttackStrategy
{
    private int _cooldownTime;
    private float _attackRange;
    private int _attackDamage;
    private int _remainingCooldown;
    private List<ITarget> _currentTargets = new List<ITarget>();

    public AttackAround(int cooldownTime, float attackRange, int attackDamage)
    {
        _cooldownTime = cooldownTime;
        _attackRange = attackRange;
        _attackDamage = attackDamage;
        _remainingCooldown = 0;
    }

    public int CooldownTime => _cooldownTime;
    public int RemainingCooldown => _remainingCooldown;

    public bool IsCooling => _remainingCooldown > 0;

    public bool IsAttackAble => !IsCooling;

    public void TickCooldown()
    {
        if (_remainingCooldown > 0)
        {
            _remainingCooldown -= 1;
        }
    }

    public void Attack(MonoBehaviour source)
    {
        if (source == null)
        {
            Debug.LogWarning("Attack source is null.");
            return;
        }
        if (!IsAttackAble)
        {
            return;
        }

        source.GetComponentInChildren<IAttackAnimation>()?.AttackAnimation(_cooldownTime);

        Vector3 sourcePos = source.transform.position;

        foreach (var target in _currentTargets)
        {
            if (target == null) continue;

            float distance = Vector3.Distance(sourcePos, target.GetTargetPosition());
            if (distance > _attackRange) continue;

            if (target is IDamageable damageable)
            {
                damageable.TakeDamage(_attackDamage);
                Debug.Log($"[AttackAround] Attack success! source={source.name}, target={target}, damage={_attackDamage}, distance={distance}");
                _remainingCooldown = _cooldownTime;
                return;
            }
        }
    }

    public IOrderedEnumerable<ITarget> TargetFiler(IEnumerable<ITarget> targets, int countHint)
    {
        if (targets == null)
        {
            _currentTargets = new List<ITarget>();
            return _currentTargets.OrderBy(_ => 0);
        }

        IEnumerable<ITarget> filtered = targets.Where(t => t != null);

        if (countHint > 0)
        {
            filtered = filtered.Take(countHint);
        }

        _currentTargets = filtered.ToList();

        return _currentTargets.OrderBy(_ => 0);
    }
}