using System.Collections.Generic;

using UnityEngine;

public class AttackerUnitSpawner : MonoBehaviour
{
    [SerializeField]
    /// <summary>
    /// 作成可能な攻撃ユニットのデータのリスト
    /// </summary>
    /// <remarks>
    /// 実行時に変更してはならない。
    /// </remarks>
    private List<AttackerUnitData> _attackerUnitData;

    public IReadOnlyList<AttackerUnitData> AttackerUnitData => _attackerUnitData;

    private Dictionary<string, AttackerUnitData> _attackerUnitDataDict = new();

    private void Awake()
    {
        foreach (var data in _attackerUnitData)
        {
            _attackerUnitDataDict.Add(data.UnitName, data);
        }
    }

    public BasicAttackerUnit Spawn(Vector3 position, string name)
    {
        if (_attackerUnitDataDict.TryGetValue(name, out var data))
        {
            return Spawn(position, data);

        }
        else
        {
            Debug.LogError($"no such {nameof(AttackerUnitData)}: name == {name}");
            return null;
        }
    }

    public BasicAttackerUnit Spawn(Vector3 position, AttackerUnitData attackerUnitData)
    {
        if (attackerUnitData.Prefab == null)
        {
            Debug.LogError($"Prefab is not set in AttackerUnitData: {attackerUnitData.UnitName}");
            return null;
        }

        var unitInstance = Instantiate(attackerUnitData.Prefab, position, Quaternion.identity);

        if (attackerUnitData.MoveStrategy == null)
        {
            Debug.LogWarning($"MoveStrategy is not set in AttackerUnitData: {attackerUnitData.UnitName}");
        }

        IMoveStrategy moveStrategy = attackerUnitData.MoveStrategy != null
            ? attackerUnitData.MoveStrategy.CreateInstance()
            : null;
        
        if (attackerUnitData.AttackStrategy == null)
        {
            Debug.LogWarning($"AttackStrategy is not set in AttackerUnitData: {attackerUnitData.UnitName}");
        }

        IAttackStrategy attackStrategy = attackerUnitData.AttackStrategy != null
           ? attackerUnitData.AttackStrategy.CreateInstance()
           : null;

        unitInstance.Initialize(attackerUnitData, moveStrategy, attackStrategy);

        return unitInstance;
    }
}
