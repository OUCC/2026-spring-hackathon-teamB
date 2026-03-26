using System.Collections;
using UnityEngine;

public class TestDirectionTilePlacer : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Vector2Int tilePos = new Vector2Int(3, 2);
    [SerializeField] private Vector2Int direction = new Vector2Int(1, 0);

    private IEnumerator Start()
    {
        yield return null;

        if (gridManager == null)
        {
            gridManager = FindFirstObjectByType<GridManager>();
        }

        if (gridManager == null)
        {
            Debug.LogError("GridManager not found.");
            yield break;
        }

        CellData cell = gridManager.GetCellData(tilePos.x, tilePos.y);
        if (cell == null)
        {
            Debug.LogError($"Cell not found at {tilePos}");
            yield break;
        }

        cell.HasDirectionTile = true;
        cell.Direction = direction;
        cell.DirectionTileRemainingUses = 3;

        Debug.Log($"Test direction tile placed at {tilePos}, dir={direction}");
    }
}