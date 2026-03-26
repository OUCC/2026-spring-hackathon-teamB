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
            Debug.LogWarning("召喚しようとしましたが、ユニットデータが空です。");
            return null;
        }

        // 63行目：ここで data.UnitName などにアクセスしても安全になる
        if (!GameManager.Instance.CanSpendMoney(GameManager.TeamType.Defense, defencerUnitData.SummonCost))
        {
            // ... お金足りない処理
            return null;
        }

        float yOffset = 0.5f; 
        Vector3 spawnPosition = new Vector3(position.x, position.y + yOffset, position.z);

        var unitInstance = Instantiate(defencerUnitData.Prefab, spawnPosition, Quaternion.identity);

        IAttackStrategy attackStrategy = defencerUnitData.AttackStrategyData.CreateInstance();
        attackStrategy.AddFilter(IAttackStrategy.TargetAttackers(unitInstance.transform));

        var defenderUnitInstance = unitInstance.GetComponentInChildren<BasicDefencerUnit>();

        defenderUnitInstance.Initialize(defencerUnitData, attackStrategy);

        if (!GameManager.Instance.GridManager.TryGetGridCellFromWorld(position, out var cell))
        {
            Debug.LogError("防御側オブジェクトをマップ外に配置しようとしています");
        }

        cell.CellData.PlacedObject = unitInstance;

        var defenceAnimation = unitInstance.GetComponentInChildren<IDefenceAnimation>();
        if (defenceAnimation != null)
        {
            if (cell != null)
            {
                defenceAnimation.SetPotion(cell.CellData);
            }

        }

        return defenderUnitInstance;
    }
}