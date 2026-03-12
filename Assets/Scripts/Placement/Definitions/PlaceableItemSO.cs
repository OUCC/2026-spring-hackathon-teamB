using UnityEngine;

[CreateAssetMenu(fileName = "NewPlaceableItem", menuName = "TeamB/Grid/Placeable Item")]
public class PlaceableItemSO : ScriptableObject
{
    [Header("Basic Info")]
    public string ItemName;
    public string ItemID;
    public Sprite Icon;

    [Header("Visuals")]
    public GameObject Prefab;
    public GameObject PreviewPrefab; // Optional semi-transparent version

    [Header("Placement Rules")]
    public bool CanOccupiedCells = false;
    // Add more rules here (e.g., categories, costs)

    public virtual bool CanPlace(CellData data)
    {
        if (data == null) return false;
        if (!CanOccupiedCells && data.IsOccupied) return false;
        return true;
    }
}
