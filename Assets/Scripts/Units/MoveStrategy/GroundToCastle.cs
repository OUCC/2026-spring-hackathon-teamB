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

    // 方向タイルでの直進中か
    private bool _isSlidingByDirectionTile = false;

    // 直進方向
    private Vector2Int _slideDirection;

    public void Move(IMovable movable)
    {
        // Castleに到達していたら、移動しない
        if (_isReachedCastle)
        {
            return;
        }

        if (!_isInitialized)
        {
            _isInitialized = true;
            _lastSteppedCell = GetBelowGridCell(movable.transform);

            if (_lastSteppedCell == null)
            {
                Debug.LogError("Could not find current cell.");
                return;
            }

            // 初期位置が方向タイルなら、そちらを優先
            if (TryStartDirectionTileMove(_lastSteppedCell.CellData, movable))
            {
                MoveTowardsDestination(movable, _moveSpeed);
                return;
            }

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
            if (remainingDistance > 0f)
            {
                MoveTowardsDestination(movable, remainingDistance);
            }
        }
    }

    private void ReachedDestination(IMovable movable)
    {
        // 方向タイルによる直進中
        if (_isSlidingByDirectionTile)
        {
            _lastSteppedCell = GetBelowGridCell(movable.transform);

            if (_lastSteppedCell == null)
            {
                Debug.LogError("Could not find current cell after sliding.");
                return;
            }

            // Castleに到達していたら、移動終了
            if (ReferenceEquals(_lastSteppedCell.CellData, GameManager.Instance.GridManager.Castle))
            {
                _isReachedCastle = true;
                return;
            }

            // 同じ方向へさらに進めるなら続行
            var straightNext = GetNextCellInDirection(_lastSteppedCell.CellData, _slideDirection);
            if (straightNext != null)
            {
                UpdateDestination(straightNext, movable);
                return;
            }

            // 進めないなら直進モード終了
            _isSlidingByDirectionTile = false;

            // 止まった先のセルがまた方向タイルなら、新しい向きで再スタート
            if (TryStartDirectionTileMove(_lastSteppedCell.CellData, movable))
            {
                return;
            }

            // 通常移動に戻る
            var nextCell = _lastSteppedCell.CellData.NextCellToCastle;
            if (nextCell == null)
            {
                Debug.LogError("next cell is null");
                return;
            }

            UpdateDestination(nextCell, movable);
            return;
        }

        // 通常移動中
        var currentCell = GetBelowGridCell(movable.transform);
        if (currentCell == null)
        {
            Debug.LogError("Could not find current cell.");
            return;
        }

        _lastSteppedCell = currentCell;

        // Castleに到達していたら、移動終了
        if (ReferenceEquals(_lastSteppedCell.CellData, GameManager.Instance.GridManager.Castle))
        {
            _isReachedCastle = true;
            return;
        }

        // 方向タイルを踏んだら直進モード開始
        if (TryStartDirectionTileMove(_lastSteppedCell.CellData, movable))
        {
            return;
        }

        var nextCellToCastle = _lastSteppedCell.CellData.NextCellToCastle;
        if (nextCellToCastle == null)
        {
            Debug.LogError("next cell is null");
            return;
        }

        UpdateDestination(nextCellToCastle, movable);
    }

    private void UpdateDestination(CellData destinationCell, IMovable movable)
    {
        if (destinationCell == null)
        {
            Debug.LogError("destinationCell is null");
            return;
        }

        var destinationCellPosition = destinationCell.GridCell.transform.position;
        destinationCellPosition.y = movable.transform.position.y; // y座標は変えない
        _destination = destinationCellPosition;

        destinationCell.OnCellDataChanged += () => HandleUnreachableDestination(movable);
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

        CellData nextCell = null;

        if (_isSlidingByDirectionTile)
        {
            nextCell = GetNextCellInDirection(_lastSteppedCell.CellData, _slideDirection);

            if (nextCell == null)
            {
                _isSlidingByDirectionTile = false;
                nextCell = _lastSteppedCell.CellData.NextCellToCastle;
            }
        }
        else
        {
            nextCell = _lastSteppedCell.CellData.NextCellToCastle;
        }

        if (nextCell == null)
        {
            Debug.LogError("next cell is null");
            return;
        }

        UpdateDestination(nextCell, movable);
    }

    /// <summary>
    /// 今いるセルが方向タイルなら、直進モードに入る
    /// </summary>
    private bool TryStartDirectionTileMove(CellData cellData, IMovable movable)
    {
        if (cellData == null) return false;
        if (!cellData.HasDirectionTile) return false;

        // 使う向きを先に退避
        Vector2Int direction = cellData.Direction;

        // 残り使用回数がないなら壊れて通常マス化
        if (cellData.DirectionTileRemainingUses <= 0)
        {
            cellData.HasDirectionTile = false;
            cellData.Direction = Vector2Int.zero;
            return false;
        }

        // 1回使用
        cellData.DirectionTileRemainingUses--;

        Debug.Log($"Direction tile used at ({cellData.X}, {cellData.Z}), remaining={cellData.DirectionTileRemainingUses}");

        // 今回の使用で壊れる
        if (cellData.DirectionTileRemainingUses <= 0)
        {
            cellData.HasDirectionTile = false;
            cellData.Direction = Vector2Int.zero;
            Debug.Log($"Direction tile broken at ({cellData.X}, {cellData.Z})");
        }

        _slideDirection = direction;
        _isSlidingByDirectionTile = true;

        var nextCell = GetNextCellInDirection(cellData, _slideDirection);
        if (nextCell == null)
        {
            _isSlidingByDirectionTile = false;
            return false;
        }

        UpdateDestination(nextCell, movable);
        return true;
    }

    /// <summary>
    /// 指定方向に1マス進めるならそのセルを返す
    /// </summary>
    private CellData GetNextCellInDirection(CellData currentCell, Vector2Int dir)
    {
        var next = GetAdjacentCell(currentCell, dir);
        if (next == null) return null;
        if (!CanMoveInto(next)) return null;
        return next;
    }

    /// <summary>
    /// 隣接セルを取得
    /// </summary>
    private CellData GetAdjacentCell(CellData cell, Vector2Int dir)
    {
        if (cell == null) return null;

        int x = cell.X + dir.x;
        int z = cell.Z + dir.y;

        GridManager gridManager = GameManager.Instance.GridManager;

        if (x < 0 || x >= gridManager.Width || z < 0 || z >= gridManager.Height)
        {
            return null;
        }

        return gridManager.GetCellData(x, z);
    }

    /// <summary>
    /// そのセルに進入できるか
    /// </summary>
    private bool CanMoveInto(CellData cell)
    {
        if (cell == null) return false;

        return cell.CanEnter;
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