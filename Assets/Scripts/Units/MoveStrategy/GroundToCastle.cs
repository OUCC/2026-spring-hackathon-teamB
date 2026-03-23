
using System;

using UnityEngine;

public class GroundToCastle : IMoveStrategy
{
    private static readonly int GROUND_LAYER = LayerMask.GetMask("Ground");

    private float _moveSpeed;
    private float _moveRequiredTime;

    public GroundToCastle(float moveSpeed, float moveRequiredTime)
    {
        _moveSpeed = moveSpeed;
        _moveRequiredTime = moveRequiredTime;
    }

    private GridCell _lastCell;
    private bool _isPassedCurrentCell = false;
    private GridCell _currentCell;

    public void Move(IMovable movable)
    {
        return; // do nothing. not implemented yet.
    }

    private GridCell GetBelowGridCell(Transform transform)
    {
        Ray ray = new(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, GROUND_LAYER))
        {
            if (hit.collider.TryGetComponent(out GridCell gridCell))
            {
                return gridCell;
            }
            else
            {
                Debug.LogError("Ground layer should have GridCell component.");
                return null;
            }
        }
        else
        {
            Debug.LogError($"Could not find ground(which can be found by layer:{GROUND_LAYER}) below the unit.");
            return null;
        }
    }

    public void OnMapUpdated()
    {
        // do nothing
        throw new InvalidOperationException();
    }
}
