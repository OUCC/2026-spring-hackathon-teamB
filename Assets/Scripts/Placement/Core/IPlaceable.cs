using UnityEngine;

public interface IPlaceable
{
    void OnPlaced(Vector2Int gridPosition);
    void OnRemoved();
}
