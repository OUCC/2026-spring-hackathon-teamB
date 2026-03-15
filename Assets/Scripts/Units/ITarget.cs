using System;
using UnityEngine;

/// <summary>
/// 攻撃・防御側関係なく、攻撃の対象となるユニットのインターフェース
/// </summary>
public interface ITarget
{
    Vector3 GetTargetPosition();
    Action<ITarget> OnDied { get; }
}
