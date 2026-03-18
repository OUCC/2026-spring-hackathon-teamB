
using System;

using UnityEngine;

public class GroundToCastle : IMoveStrategy
{
    private static readonly int GROUND_LAYER = LayerMask.GetMask("Ground");

    private GridCell _lastCell;
    private bool _isPassedCurrentCell = false;
    private GridCell _currentCell;

    public void Move(IMovable movable)
    {
        var currentCell = GetBelowGridCell(movable.transform);
        if (currentCell == null)
            return;
        if (!ReferenceEquals(_currentCell, currentCell))
        {
            _lastCell = _currentCell;
            _currentCell = currentCell;
            _isPassedCurrentCell = false;
        }

        if (!_isPassedCurrentCell)
        {
            var diffBetween = _currentCell.transform.position - movable.transform.position;
            diffBetween.y = 0f;
            if (diffBetween.sqrMagnitude < movable.MoveSpeed * movable.MoveSpeed)
            {
                _isPassedCurrentCell = true;
                var leftDistance = movable.MoveSpeed - diffBetween.magnitude;
                movable.transform.position = _currentCell.transform.position + currentCell.CellData.DirectionToNextCell * leftDistance;
                return;
            }
            else
            {
                movable.transform.position += _lastCell.CellData.DirectionToNextCell * movable.MoveSpeed;
            }
        }
        else
        {
            movable.transform.position += _currentCell.CellData.DirectionToNextCell * movable.MoveSpeed;
        }
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
