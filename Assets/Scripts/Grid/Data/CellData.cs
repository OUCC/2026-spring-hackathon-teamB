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

    public int X { get; private set; }
    public int Z { get; private set; }
    public CellData NextCellToCastle { get; set; }

    public event Action OnCellDataChanged;

    public CellData(int x, int z, GridCell gridCell)
    {
        X = x;
        Z = z;
        GridCell = gridCell;
    }

    public Vector3 DirectionToNextCell => NextCellToCastle != null ? (NextCellToCastle.GridCell.transform.position - GridCell.transform.position).normalized : Vector3.zero;
}
