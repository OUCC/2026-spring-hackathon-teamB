using UnityEngine;

public class DirectionTileSpawner : MonoBehaviour
{
    public enum SpawnMode
    {
        Place,
        Remove
    }

    public enum DirectionType
    {
        Right,
        Left,
        Up,
        Down
    }

    [Header("Reference")]
    [SerializeField] private GridManager gridManager;

    [Header("Target Cell")]
    [SerializeField] private int x = 2;
    [SerializeField] private int z = 1;

    [Header("Tile Setting")]
    [SerializeField] private SpawnMode spawnMode = SpawnMode.Place;
    [SerializeField] private DirectionType directionType = DirectionType.Down;
    [SerializeField] private int uses = 3;

    [Header("Execution")]
    [SerializeField] private bool executeOnStart = true;

    private void Start()
    {
        if (executeOnStart)
        {
            Execute();
        }
    }

    [ContextMenu("Execute")]
    public void Execute()
    {
        if (!TryGetGridManager(out GridManager gm))
            return;

        if (!TryGetCell(gm, x, z, out CellData cell))
            return;

        if (spawnMode == SpawnMode.Remove)
        {
            RemoveDirectionTile(cell);
            Debug.Log($"[DirectionTileSpawner] Removed direction tile at ({x}, {z})");
            return;
        }

        Vector2Int dir = ToVector(directionType);
        if (dir == Vector2Int.zero)
        {
            Debug.LogError("[DirectionTileSpawner] Invalid direction.");
            return;
        }

        int safeUses = Mathf.Max(1, uses);

        PlaceDirectionTile(cell, dir, safeUses);

        Debug.Log(
            $"[DirectionTileSpawner] Placed direction tile at ({x}, {z}), " +
            $"dir={dir}, uses={safeUses}"
        );
    }

    private bool TryGetGridManager(out GridManager gm)
    {
        gm = gridManager;

        if (gm == null)
        {
            gm = FindFirstObjectByType<GridManager>();
        }

        if (gm == null)
        {
            Debug.LogError("[DirectionTileSpawner] GridManager not found.");
            return false;
        }

        return true;
    }

    private bool TryGetCell(GridManager gm, int cellX, int cellZ, out CellData cell)
    {
        cell = null;

        if (gm == null)
        {
            Debug.LogError("[DirectionTileSpawner] GridManager is null.");
            return false;
        }

        cell = gm.GetCellData(cellX, cellZ);

        if (cell == null)
        {
            Debug.LogError($"[DirectionTileSpawner] CellData not found at ({cellX}, {cellZ}).");
            return false;
        }

        return true;
    }

    private void PlaceDirectionTile(CellData cell, Vector2Int dir, int tileUses)
    {
        if (cell == null)
        {
            Debug.LogError("[DirectionTileSpawner] CellData is null in PlaceDirectionTile.");
            return;
        }

        cell.Direction = dir;
        cell.DirectionTileRemainingUses = tileUses;
        cell.HasDirectionTile = true;
    }

    private void RemoveDirectionTile(CellData cell)
    {
        if (cell == null)
        {
            Debug.LogError("[DirectionTileSpawner] CellData is null in RemoveDirectionTile.");
            return;
        }

        cell.HasDirectionTile = false;
        cell.Direction = Vector2Int.zero;
        cell.DirectionTileRemainingUses = 0;
    }

    private Vector2Int ToVector(DirectionType type)
    {
        switch (type)
        {
            case DirectionType.Right:
                return new Vector2Int(1, 0);
            case DirectionType.Left:
                return new Vector2Int(-1, 0);
            case DirectionType.Up:
                return new Vector2Int(0, 1);
            case DirectionType.Down:
                return new Vector2Int(0, -1);
            default:
                return Vector2Int.zero;
        }
    }
    // 外部のスクリプトから呼び出してタイルを配置するための専用メソッド
    public void PlaceTileFromExternal(int targetX, int targetZ, DirectionType dir)
    {
        // 自分の変数を上書きする
        this.x = targetX;
        this.z = targetZ;
        this.directionType = dir;
        this.spawnMode = SpawnMode.Place;

        // 既存の配置ロジックを実行する
        Execute();
    }
}