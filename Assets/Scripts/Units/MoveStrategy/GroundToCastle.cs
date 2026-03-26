
using System;

using UnityEngine;

public class GroundToCastle : IMoveStrategy
{
    public static readonly int GROUND_LAYER = LayerMask.GetMask("Ground");

    private float _moveSpeed;
    private float _moveRequiredTime;

    public GroundToCastle(float moveSpeed, float moveRequiredTime)
    {
        _moveSpeed = moveSpeed;
        _moveRequiredTime = moveRequiredTime;
    }

    private GridCell _lastSteppedCell;
    private Vector3 _destination;
    private bool _isInitialized = false;

    private bool _isReachedCastle = false;

    public void Move(IMovable movable)
    {
        // Castleに到達していたら、移動しない
        if (_isReachedCastle)
        {
            return;
        }

        // 真下のセルの情報から次に進む座標を取得 => 到達したら、次のセルの情報から次に進む座標を取得 => 以下ループ
        // CellData.OnCellDataChangedでセルの状態変化を取得できるから、進行先がなくなったら、進行先を再取得する

        if (!_isInitialized)
        {
            _isInitialized = true;
            _lastSteppedCell = GetBelowGridCell(movable.transform);

            var nextCell = _lastSteppedCell.CellData.NextCellToCastle;
            if (nextCell == null)
            {
                Debug.LogError("next cell is null");
                return;
            }
            UpdateDestination(nextCell, movable);
        }

        MoveTowardsDestination(movable, _moveSpeed);
    }

    private void MoveTowardsDestination(IMovable movable, float moveDistance)
    {
        var diff = (_destination - movable.transform.position);
        if (diff.sqrMagnitude > moveDistance * moveDistance)
        {
            diff.y = 0; // y座標は変えない
            movable.transform.position += diff.normalized * moveDistance;
            movable.transform.rotation = Quaternion.LookRotation(diff.normalized);
            return;
        }
        else
        {
            movable.transform.position = _destination;
            ReachedDestination(movable);
            if (_isReachedCastle)
            {
                return;
            }
            var remainingDistance = moveDistance - diff.magnitude;
            MoveTowardsDestination(movable, remainingDistance);
        }
    }


    private void ReachedDestination(IMovable movable)
    {
        // 到達したら、次のdestinationをセットする
        _lastSteppedCell = _lastSteppedCell.CellData.NextCellToCastle.GridCell;

        // Castleに到達していたら、移動終了
        if (ReferenceEquals(_lastSteppedCell.CellData, GameManager.Instance.GridManager.Castle))
        {
            _isReachedCastle = true;
            return;
        }

        var nextCell = _lastSteppedCell.CellData.NextCellToCastle;
        UpdateDestination(nextCell, movable);
    }

    private CellData _currentDestinationCell;
    private Action<CellData> _currentAction;
    private void UpdateDestination(CellData destinationCell, IMovable movable)
    {
        if (_currentDestinationCell != null && _currentAction != null)
        {
            _currentDestinationCell.OnCellDataChanged -= _currentAction;
        }
        _currentDestinationCell = destinationCell;

        var destinationCellPosition = destinationCell.GridCell.transform.position;
        destinationCellPosition.y = movable.transform.position.y; // y座標は変えない
        _destination = destinationCellPosition;
        _currentAction = (_) => HandleUnreachableDestination(movable);
        destinationCell.OnCellDataChanged += _currentAction;
    }

    private void HandleUnreachableDestination(IMovable movable)
    {
        // まだ同じセルにいる場合は、次のdestinationを再取得する
        // すでに次のセルに進んでいる場合は、戻って行き先を再取得する
        if (!ReferenceEquals(GetBelowGridCell(movable.transform), _lastSteppedCell))
        {
            var cellBoundary = (_lastSteppedCell.transform.position + _destination) / 2f;
            movable.transform.position = cellBoundary; // セルの境界に移動させる
        }
        UpdateDestination(_lastSteppedCell.CellData.NextCellToCastle, movable);
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
