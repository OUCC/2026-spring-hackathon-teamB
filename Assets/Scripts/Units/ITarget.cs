using System;

using UnityEngine;

/// <summary>
/// 攻撃・防御側関係なく、攻撃の対象となるユニットのインターフェース
/// </summary>
public interface ITarget
{
    Vector3 GetTargetPosition();

#pragma warning disable IDE1006 // 命名スタイル
    GameObject gameObject { get; }
#pragma warning restore IDE1006 // 命名スタイル

    event Action<ITarget> OnDied;
}
