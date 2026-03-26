using UnityEngine;

public class DirectionTileSpawner : MonoBehaviour
{
    public enum DirectionType { Right, Left, Up, Down }

    [Header("Reference")]
    [SerializeField] private GridManager gridManager;

    private void Awake()
    {
        if (gridManager == null)
            gridManager = FindFirstObjectByType<GridManager>();
    }

    public enum PlaceCheckResult { Ok, NotEnoughMoney, CellNotFound, CellAlreadyOccupied }

    public PlaceCheckResult CheckPlace(int x, int z, int cost)
    {
        if (GameManager.Instance.DefenseMoney < cost) 
            return PlaceCheckResult.NotEnoughMoney;

        CellData cell = gridManager?.GetCellData(x, z);
        if (cell == null) 
            return PlaceCheckResult.CellNotFound;

        if (cell.HasDirectionTile) 
            return PlaceCheckResult.CellAlreadyOccupied;

        return PlaceCheckResult.Ok;
    }

    // 外部（PlayerCursorController）から呼ばれるメインの配置メソッド
    public bool Spawn(Vector3 position, DirectionType dirType, int cost, int uses = 3)
    {
        int x = Mathf.RoundToInt(position.x);
        int z = Mathf.RoundToInt(position.z);

        // 1. 配置可能かチェック
        switch (CheckPlace(x, z, cost))
        {
            case PlaceCheckResult.NotEnoughMoney:
                Debug.LogWarning($"タイルを配置する資金({cost})が不足しています。");
                return false;
            case PlaceCheckResult.CellNotFound:
                Debug.LogWarning($"座標 ({x}, {z}) にセルが見つかりません。");
                return false;
            case PlaceCheckResult.CellAlreadyOccupied:
                Debug.LogWarning($"座標 ({x}, {z}) には既にタイルが配置されています。");
                return false;
        }

        // 2. 資金消費
        if (!GameManager.Instance.SpendMoney(GameManager.TeamType.Defense, cost))
        {
            return false;
        }

        // 3. データの書き込み（GridManagerが検知して自動で矢印モデルを出してくれます！）
        CellData cell = gridManager.GetCellData(x, z);
        cell.Direction = ToVector(dirType);
        cell.DirectionTileRemainingUses = uses;
        
        // HasDirectionTile を true にすると、OnCellDataChanged が発火し、
        // GridManager.UpdateDirectionIndicator が呼ばれて矢印が生成される
        cell.HasDirectionTile = true; 

        Debug.Log($"[DirectionTileSpawner] Placed '{dirType}' tile at ({x}, {z})");
        return true;
    }

    private Vector2Int ToVector(DirectionType type)
    {
        return type switch
        {
            DirectionType.Right => new Vector2Int(1, 0),
            DirectionType.Left => new Vector2Int(-1, 0),
            DirectionType.Up => new Vector2Int(0, 1),
            DirectionType.Down => new Vector2Int(0, -1),
            _ => Vector2Int.zero,
        };
    }
}