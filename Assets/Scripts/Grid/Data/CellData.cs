using UnityEngine;

public class CellData
{
    public Vector2Int Coordinates { get; private set; }
    public bool IsOccupied => PlacedObject != null;
    public GameObject PlacedObject { get; set; }
    public PlaceableItemSO ItemType { get; set; }

    public CellData(int x, int z)
    {
        Coordinates = new Vector2Int(x, z);
    }
}
