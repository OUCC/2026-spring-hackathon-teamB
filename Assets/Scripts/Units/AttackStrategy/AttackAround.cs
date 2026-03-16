using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackAround : IAttackStrategy
{
    private int _cooldownTime;
    private float _attackRange;

    public AttackAround(int cooldownTime, float attackRange)
    {
        _cooldownTime = cooldownTime;
        _attackRange = attackRange;
    }

    public int CooldownTime => _cooldownTime;
    public int RemainingCooldown => throw new System.NotImplementedException();

    public bool IsCooling => throw new System.NotImplementedException();

    public bool IsAttackAble => throw new System.NotImplementedException();

    public void Attack()
    {
        throw new System.NotImplementedException();
    }

    public void Attack(MonoBehaviour source)
    {
        throw new System.NotImplementedException();
    }

    public IOrderedEnumerable<ITarget> TargetFiler(IEnumerable<ITarget> targets, int countHint)
    {
        throw new System.NotImplementedException();
    }
}