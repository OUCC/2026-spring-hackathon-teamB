using UnityEngine;

[CreateAssetMenu(fileName = "AttackerUnitData", menuName = "ScriptableObjects/AttackerUnitData")]
public class AttackerUnitData : ScriptableObject
{
    /// <summary>
    /// ユニットの種類名
    /// </summary>
    public string UnitName;

    /// <summary>
    /// ユニットの最大体力
    /// </summary>
    public int MaxHealth;

    /// <summary>
    /// 1回の移動で進む距離
    /// </summary>
    public float MoveSpeed;

    /// <summary>
    /// 1回の移動にかかる時間
    /// </summary>
    public int MoveRequiredTime;

    /// <summary>
    /// 攻撃範囲
    /// </summary>
    public float AttackRange;

    /// <summary>
    /// 攻撃を行った後、次の攻撃ができるようになるまでの時間
    /// </summary>
    public float AttackCoolTime;
    /// <summary>
    /// 攻撃のダメージ量
    /// </summary>
    public float AttackDamage;

    /// <summary>
    /// 召喚コスト
    /// </summary>
    public int SummonCost;
    /// <summary>
    /// 召喚の後、次の召喚ができるようになるまでの時間
    /// </summary>
    public int SummonCoolTime;
    /// <summary>
    /// 盤面に召喚できるユニットの最大数
    /// </summary>
    public int SummonLimit;
}
