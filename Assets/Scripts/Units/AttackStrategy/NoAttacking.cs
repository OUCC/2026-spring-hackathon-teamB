using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class NoAttacking : IAttackStrategy
{
    public int CooldownTime => int.MaxValue;

    public int RemainingCooldown => int.MaxValue;

    public bool IsAttackAble => false;

    public void Attack(MonoBehaviour source)
    {
        return;
    }

    public void AddFilter(Func<IEnumerable<ITarget>, IOrderedEnumerable<ITarget>> filter)
    {
        return;
    }

    public void RemoveFilter(Func<IEnumerable<ITarget>, IOrderedEnumerable<ITarget>> filter)
    {
        return;
    }

    public IOrderedEnumerable<ITarget> TargetFiler(IEnumerable<ITarget> targets, int countHint)
    {
        return targets.OrderBy(t => 0); // 常に空の順序付き列挙を返す
    }
}
