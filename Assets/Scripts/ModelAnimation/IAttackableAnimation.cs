using UnityEngine.UIElements;

public interface IAttackableAnimation
{
    /// <summary>
    /// 攻撃アニメーションを再生する
    /// </summary>
    /// <param name="flames">再生する時間</param>
    void AttackAnimation(int flames);

    Image UnitImage { get; }
}
