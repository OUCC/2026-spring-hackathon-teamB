using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "AttackerUnitData", menuName = "ScriptableObjects/AttackerUnitData/AttackerUnitData")]
public class AttackerUnitData : ScriptableObject
{
    /// <summary>
    /// ユニットの種類名
    /// </summary>
    public string UnitName;

    /// <summary>
    /// ユニットのプレハブ
    /// </summary>
    public GameObject Prefab;

    public float Scale = 1f;

    public Image UnitImage
    {
        get
        {
            var animation = Prefab.GetComponent<IAttackableAnimation>();
            if (animation != null)
            {
                return animation.UnitImage;
            }
            else
            {
                Debug.LogWarning($"Prefab {Prefab.name} does not have IAttackableAnimation component. UnitImage will be null.");
                return null;
            }
        }
    }

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
