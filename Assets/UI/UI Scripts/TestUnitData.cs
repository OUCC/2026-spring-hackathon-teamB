using UnityEngine;

// 右クリックメニューから簡単に作成できるようにする属性
[CreateAssetMenu(fileName = "NewUnitData", menuName = "ScriptableObjects/UnitData")]
public class UnitData : ScriptableObject
{
    public Texture2D unitImage;       // ユニットのアイコン画像
    public int cost = 10;          // 使用コスト
}
