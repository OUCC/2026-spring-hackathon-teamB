using System.Linq;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] private DefencerUnitSpawner defencerUnitSpawner;

    void Start()
    {
        var gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogError("GameManager.Instance is null");
            return;
        }

        var gridManager = gameManager.GridManager;
        if (gridManager == null)
        {
            Debug.LogError("GridManager is null");
            return;
        }

        var attackerSpawner = gameManager.AttackerUnitSpawner;
        if (attackerSpawner == null)
        {
            Debug.LogError("AttackerUnitSpawner is null");
            return;
        }

        if (defencerUnitSpawner == null)
        {
            Debug.LogError("DefencerUnitSpawner is not assigned");
            return;
        }

        var attackerSpawnCell = gridManager.GetGridCell(2, 0);
        var defenderSpawnCell = gridManager.GetGridCell(2, 5);

        var attackerData = attackerSpawner.AttackerUnitData.FirstOrDefault();
        if (attackerData == null)
        {
            Debug.LogError("No attacker unit data found!");
            return;
        }

        var defenderData = defencerUnitSpawner.DefencerUnitData.FirstOrDefault();
        if (defenderData == null)
        {
            Debug.LogError("No defender unit data found!");
            return;
        }

        gameManager.StartGame();

        var attacker = attackerSpawner.Spawn(
            attackerSpawnCell.transform.position + new Vector3(0, 1, 0),
            attackerData
        );

        var defender = defencerUnitSpawner.Spawn(
            defenderSpawnCell.transform.position + new Vector3(0, 1, 0),
            defenderData
        );

        Debug.Log($"attacker spawned: {attacker?.name}");
        Debug.Log($"defender spawned: {defender?.name}");
    }

    void Update()
    {
    }
}