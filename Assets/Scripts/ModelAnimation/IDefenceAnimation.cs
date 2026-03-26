using UnityEngine.UIElements;

public interface IDefenceAnimation
{
    /// <summary>
    /// 初期化する
    /// </summary>
    /// <param name="position">設置する場所</param>
    void SetPotion(CellData position);
    Image UnitImage { get; }
}
