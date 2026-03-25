using System;

using UnityEngine;

public class CellData
{
    [System.Obsolete("非推奨プロパティです。 CellData.X及びCellData.Zを使用してください。")]
    public Vector2Int Coordinates { get { return new(X, Z); } }
    public bool IsOccupied => PlacedObject != null;
    public GameObject PlacedObject { get; set; }

    public PlaceableItemSO _itemType;
    public PlaceableItemSO ItemType
    {
        get => _itemType;
        set
        {
            if (_itemType != value)
            {
                _itemType = value;
                OnCellDataChanged?.Invoke();
            }
        }
    }

    private GridCell _gridCell;

    public GridCell GridCell
    {
        get
        {
            if (_gridCell == null)
            {
                _gridCell = GameManager.Instance.GridManager.GetGridCell(X, Z);
            }
            if (_gridCell == null)
            {
                Debug.LogError($"GridCell at ({X}, {Z}) could not be found.");
            }
            return _gridCell;
        }
        set { _gridCell = value; }
    }

    public bool CanEnter => GridCell != null && !IsOccupied;

    public int X { get; private set; }
    public int Z { get; private set; }

    private CellData _nextCellToCastle;
    public CellData NextCellToCastle
    {
        get => _nextCellToCastle;
        set
        {
            if (_nextCellToCastle != value)
            {
                _nextCellToCastle = value;
                OnCellDataChanged?.Invoke();
            }
        }
    }

    public event Action OnCellDataChanged;

    private bool _hasDirectionTile;
    public bool HasDirectionTile
    {
        get => _hasDirectionTile;
        set
        {
            if (_hasDirectionTile != value)
            {
                _hasDirectionTile = value;
                OnCellDataChanged?.Invoke();
            }
        }
    }

    private Vector2Int _direction;
    public Vector2Int Direction
    {
        get => _direction;
        set
        {
            if (_direction != value)
            {
                _direction = value;
                OnCellDataChanged?.Invoke();
            }
        }
    }

    private int _directionTileRemainingUses;
    public int DirectionTileRemainingUses
    {
        get => _directionTileRemainingUses;
        set
        {
            if (_directionTileRemainingUses != value)
            {
                _directionTileRemainingUses = value;
                OnCellDataChanged?.Invoke();
            }
        }
    }

    public CellData(int x, int z, GridCell gridCell)
    {
        X = x;
        Z = z;
        GridCell = gridCell;
    }

    public Vector3 DirectionToNextCell => NextCellToCastle != null ? (NextCellToCastle.GridCell.transform.position - GridCell.transform.position).normalized : Vector3.zero;
}