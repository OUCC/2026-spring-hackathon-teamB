using UnityEngine;
/// <summary>
/// <see cref="IShootable"/>によって発射される物体を表すインターフェース
/// </summary>
public interface IProjectile
{
    IShootable Source { get; }

    Vector3 Target { get; }

    float Speed { get; }

    int Damage { get; }

    /// <summary>
    /// 後何回攻撃できるかを返す。0になったら消滅する。
    /// </summary>
    /// <returns></returns>
    int RemainAttackCount { get; }

    void ReduceAttackCount();

    void Move();
}
