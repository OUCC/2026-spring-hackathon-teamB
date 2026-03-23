using System.Collections.Generic;
using UnityEngine;

public class DefencerUnitSpawner : MonoBehaviour
{
    [SerializeField]
    private List<DefencerUnitData> _defencerUnitData;

    public IReadOnlyList<DefencerUnitData> DefencerUnitData => _defencerUnitData;

    private Dictionary<string, DefencerUnitData> _defencerUnitDataDict = new();

    private void Awake()
    {
        foreach (var data in _defencerUnitData)
        {
            if (data == null)
            {
                Debug.LogWarning("DefencerUnitData に null が含まれています");
                continue;
            }

            if (_defencerUnitDataDict.ContainsKey(data.UnitName))
            {
                Debug.LogWarning($"DefencerUnitData の UnitName が重複しています: {data.UnitName}");
                continue;
            }

            _defencerUnitDataDict.Add(data.UnitName, data);
        }
    }

    public BasicDefencerUnit Spawn(Vector3 position, string name)
    {
        if (_defencerUnitDataDict.TryGetValue(name, out var data))
        {
            return Spawn(position, data);
        }
        else
        {
            Debug.LogError($"no such {nameof(DefencerUnitData)}: name == {name}");
            return null;
        }
    }

    public BasicDefencerUnit Spawn(Vector3 position, DefencerUnitData defencerUnitData)
    {
        if (defencerUnitData == null)
        {
            Debug.LogError("DefencerUnitData is null");
            return null;
        }

        if (defencerUnitData.Prefab == null)
        {
            Debug.LogError($"Prefab is not set in DefencerUnitData: {defencerUnitData.UnitName}");
            return null;
        }

        var unitInstance = Instantiate(defencerUnitData.Prefab, position, Quaternion.identity);

        IAttackStrategy attackStrategy = new AttackArrow(
            defencerUnitData.AttackCoolTime,
            defencerUnitData.AttackRange,
            defencerUnitData.AttackDamage
        );

        unitInstance.Initialize(defencerUnitData, attackStrategy);

        return unitInstance;
    }
}