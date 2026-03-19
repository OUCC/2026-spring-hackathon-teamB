using UnityEngine;

/// <summary>
/// 盤面上で移動するユニットのインターフェース
/// </summary>
public interface IMoveStrategy
{
    void Move(IMovable movable);
    void OnMapUpdated();
}

