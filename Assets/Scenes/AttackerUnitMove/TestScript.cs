using System.Linq;

using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var gameManager = GameManager.Instance;
        var gridManager = gameManager.GridManager;
        var spawnPoint = gridManager.GetGridCell(2, 0);
        var spawner = gameManager.AttackerUnitSpawner;
        var unitData = spawner.AttackerUnitData.First();
        if (unitData == null)
        {
            Debug.LogError("No attacker unit data found!");
            return; 
        }
        var unit = spawner.Spawn(spawnPoint.transform.position + new Vector3(0, 1, 0), unitData);

        gameManager.StartGame();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
