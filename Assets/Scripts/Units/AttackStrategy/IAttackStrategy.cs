using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IAttackStrategy
{
    bool IsCooling { get { return RemainingCooldown > 0; } }

    int CooldownTime { get; }

    /// <summary>
    /// 残りクールタイム(tick)
    /// クールタイムが無ければ、常に0
    /// </summary>
    int RemainingCooldown { get; }

    /// <summary>
    /// 攻撃可能かどうか。クールタイムがあっても、特定の条件下では攻撃可能な場合もあるかも
    /// </summary>
    bool IsAttackAble { get; }

    IOrderedEnumerable<ITarget> TargetFiler(IEnumerable<ITarget> targets, int countHint);

    /// <summary>
    /// 攻撃を行う。
    /// アニメーションとの整合性を取るために、複数に分割してもいいかも
    /// </summary>
    void Attack(MonoBehaviour source);
}