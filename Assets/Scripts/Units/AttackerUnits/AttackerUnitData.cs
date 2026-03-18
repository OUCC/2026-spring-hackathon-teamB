using UnityEngine;

[CreateAssetMenu(fileName = "AttackerUnitData", menuName = "ScriptableObjects/AttackerUnitData/AttackerUnitData")]
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

    /// <summary>
    /// 動きのデータ(ScriptableObject)
    /// </summary>
    public MoveStrategyData MoveStrategy;

    /// <summary>
    /// 攻撃のデータ(ScriptableObject)
    /// </summary>
    public AttackStrategyData AttackStrategy;
}
