using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class AttackArrow : IAttackStrategy, IDisposable
{
    private int _cooldownTime;
    private float _attackRange;
    private int _attackDamage;
    private int _remainingCooldown;
    private List<ITarget> _currentTargets = new List<ITarget>();


    private Collider[] _lookAroundResults;

    public AttackArrow(int cooldownTime, float attackRange, int attackDamage)
    {
        _cooldownTime = cooldownTime;
        _attackRange = attackRange;
        _attackDamage = attackDamage;
        _remainingCooldown = 0;

        // 攻撃時に周囲の攻撃対象を探すための配列。
        // フィルタによって実際に攻撃をする数は減るため、同時攻撃対象数より十分多くしておく。
        _lookAroundResults = new Collider[10];

        GameManager.Instance.OnTickEvent += TickCooldown;
    }

    public void Dispose()
    {
        GameManager.Instance.OnTickEvent -= TickCooldown;
    }

    public int CooldownTime => _cooldownTime;
    public int RemainingCooldown => _remainingCooldown;
    public bool IsCooling => _remainingCooldown > 0;
    public bool IsAttackAble => !IsCooling;

    private void TickCooldown()
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

        if (source is not IShootable)
        {
            Debug.LogWarning("Attack source is not IShootable.");
            return;
        }

        BasicDefencerUnit shooter = source as BasicDefencerUnit;
        if (shooter == null)
        {
            Debug.LogWarning("AttackArrow currently supports BasicDefencerUnit only.");
            return;
        }

        var foundTargetCount = Physics.OverlapSphereNonAlloc(source.gameObject.transform.position, _attackRange, _lookAroundResults);
        var filterdTargets = TargetFiler(_lookAroundResults.Take(foundTargetCount).Select(c => c.GetComponent<ITarget>()).Where(t => t != null), 1);
        var actualTargets = filterdTargets.Take(1); // 同時攻撃数は1に制限

        var attackAnimation = source.GetComponentInChildren<IAttackAnimation>();
        Vector3 sourcePos = source.transform.position;

        foreach (var target in actualTargets)
        {
            if (target == null) continue;

            float distance = Vector3.Distance(sourcePos, target.GetTargetPosition());
            if (distance > _attackRange) continue;

            shooter.Shoot(target.GetTargetPosition(), _attackDamage, 1);
            if (attackAnimation != null)
            {
                attackAnimation.AttackAnimation(_cooldownTime, Quaternion.LookRotation(target.gameObject.transform.position - source.transform.position).normalized);
            }

            Debug.Log($"[AttackArrow] Shoot success! source={source.name}, target={target}, damage={_attackDamage}, distance={distance}");
        }
        _remainingCooldown = _cooldownTime;
        return;
    }

    public IOrderedEnumerable<ITarget> TargetFiler(IEnumerable<ITarget> targets, int countHint)
    {
        if (targets == null)
        {
            _currentTargets = new List<ITarget>();
            return _currentTargets.OrderBy(_ => 0);
        }

        var filtered = targets.Where(t => t != null);

        if (_targetFilters.Count == 0)
        {
            filtered = filtered.OrderBy(_ => 0);
        }
        foreach (var filter in _targetFilters)
        {
            filtered = filter(filtered);
        }

        var orderd = filtered as IOrderedEnumerable<ITarget>;
        _currentTargets = orderd.ToList();
        return orderd;
    }

    private List<Func<IEnumerable<ITarget>, IOrderedEnumerable<ITarget>>> _targetFilters = new();

    public void AddFilter(Func<IEnumerable<ITarget>, IOrderedEnumerable<ITarget>> filter)
    {
        if (!_targetFilters.Contains(filter))
            _targetFilters.Add(filter);
    }

    public void RemoveFilter(Func<IEnumerable<ITarget>, IOrderedEnumerable<ITarget>> filter)
    {
        _targetFilters.Remove(filter);
    }
}