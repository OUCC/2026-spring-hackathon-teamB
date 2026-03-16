
using System;
using UnityEngine;

public class GroundToCastle : IMoveStrategy
{
    public void Move(Transform transform, float moveSpeed)
    {
        Vector3 direction = GetFlowDirection(transform);
        transform.position += direction.normalized * moveSpeed;
    }

    /// <summary>
    /// マップ情報から、次に進むべき方向を求める。
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    private Vector3 GetFlowDirection(Transform transform)
    {
        Debug.Log("not implemented yet");
        return new Vector3(1f, 1f, 0f);
    }

    public void OnMapUpdated()
    {
        // do nothing
        throw new InvalidOperationException();
    }
}
