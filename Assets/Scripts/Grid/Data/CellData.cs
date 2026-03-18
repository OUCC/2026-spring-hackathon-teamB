using UnityEngine;

public class CellData
{
    [System.Obsolete("非推奨プロパティです。 CellData.X及びCellData.Zを使用してください。")]
    public Vector2Int Coordinates { get { return new(X, Z); } }
    public bool IsOccupied => PlacedObject != null;
    public GameObject PlacedObject { get; set; }
    public PlaceableItemSO ItemType { get; set; }

    public GridCell GridCell { get; set; }

    public int X { get; private set; }
    public int Z { get; private set; }
    public CellData NextCellToCastle { get; set; }

    public CellData(int x, int z, GridCell gridCell)
    {
        X = x;
        Z = z;
        GridCell = gridCell;
    }

    public Vector3 DirectionToNextCell => NextCellToCastle != null ? (NextCellToCastle.GridCell.transform.position - GridCell.transform.position).normalized : Vector3.zero;
}
