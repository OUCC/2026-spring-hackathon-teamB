using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class AttackAround : IAttackStrategy
{
    private int _cooldownTime;
    private float _attackRange;
    private int _attackDamage;

    public AttackAround(int cooldownTime, float attackRange, int attackDamage)
    {
        _cooldownTime = cooldownTime;
        _attackRange = attackRange;
        _attackDamage = attackDamage;
    }

    public int CooldownTime => _cooldownTime;
    public int RemainingCooldown => throw new System.NotImplementedException();

    public bool IsCooling => throw new System.NotImplementedException();

    public bool IsAttackAble => false;

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