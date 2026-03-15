using UnityEngine;
/// <summary>
/// <see cref="IShootable"/>によって発射される物体を表すインターフェース
/// </summary>
public interface IProjectile
{
    IShootable GetSource();

    Vector3 GetTarget();

    float GetSpeed();

    int GetDamage();

    /// <summary>
    /// 後何回攻撃できるかを返す。0になったら消滅する。
    /// </summary>
    /// <returns></returns>
    int GetRemainAttackCount();

    void ReduceAttackCount();

    void Move();
}
