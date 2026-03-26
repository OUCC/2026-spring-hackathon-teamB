public enum ItemType
{
    Unit,
    Tile
}

public class DefenderUIObject
{
    public string name;
    public int summonCost;
    public ItemType type;
    public DefencerUnitData unitData;
    public DirectionTileSpawner.DirectionType tileDirection;
}