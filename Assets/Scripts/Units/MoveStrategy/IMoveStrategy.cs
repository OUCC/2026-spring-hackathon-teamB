using UnityEngine;

/// <summary>
/// 盤面上で移動するユニットのインターフェース
/// </summary>
public interface IMoveStrategy
{
    void Move(Transform transform, float MoveSpeed);
    void OnMapUpdated();
}

