using UnityEngine;

// 設置物の種類
public enum ItemType 
{ 
    Unit, 
    Tile 
}

// 共通のデータ保持クラス
[System.Serializable]
public class DefenderUIObject
{
    public string name;
    public int summonCost;
    public ItemType type;
    
    // ユニットの場合に使用
    public DefencerUnitData unitData; 

    // タイルの場合に使用（SOではなくEnumを使用！）
    public DirectionTileSpawner.DirectionType tileDirection; 
}
