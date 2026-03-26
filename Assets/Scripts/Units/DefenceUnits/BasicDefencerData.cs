using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "DefencerUnitData", menuName = "ScriptableObjects/DefencerUnitData")]
public class DefencerUnitData : ScriptableObject
{
    /// <summary>
    /// ユニットの種類名
    /// </summary>
    public string UnitName;
    /// <summary>
    /// 生成するPrefab
    /// </summary>
    public GameObject Prefab;

    public Image UnitImage
    {
        get
        {
            var animation = Prefab.GetComponent<IAttackAnimation>();
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
    /// 壁を含むオブジェクトであるかどうか。
    /// 壁の接続のために必要な情報。壁であれば、隣接する同じ種類の壁と接続して見えるようにする。
    /// trueの場合、壁の接続を試みる。
    /// </summary>
    public bool IsWall = false;

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
    /// 自動体力回復量
    /// </summary>
    public int AutoHealAmount;

    public AttackStrategyData AttackStrategyData;
}
