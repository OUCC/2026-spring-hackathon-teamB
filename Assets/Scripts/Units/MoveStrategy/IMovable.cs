using UnityEngine;

public interface IMovable
{
#pragma warning disable IDE1006 // 命名スタイル
    Transform transform { get; }

    GameObject gameObject { get; }
#pragma warning restore IDE1006 // 命名スタイル
}
