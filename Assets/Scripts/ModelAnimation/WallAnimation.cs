using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class WallAnimation : MonoBehaviour, IDefenceAnimation
{
    [SerializeField]
    // x軸方向の壁
    private GameObject _wall;
    [SerializeField]
    // 十字方向の壁
    private GameObject _crossingWall;

    [field: SerializeField]
    public Image UnitImage { get; }

    private CellData _position;

    private bool[,] _isNextToWall = new bool[3, 3];

    private List<(CellData cellData, Action<CellData> action)> subscribingEvents = new();

    [Flags]
    public enum Direction
    {
        None = 0,
        Left = 1 << 0,
        Right = 1 << 1,
        Down = 1 << 2,
        Up = 1 << 3
    }

    private Direction _wallState = Direction.None;

    public void SetPotion(CellData position)
    {
        _position = position;

        var gridManager = GameManager.Instance.GridManager;

        // X -方向
        if (gridManager.TryGetCellData(position.X - 1, position.Z, out var cellData))
        {
            LoadNextCellState(Direction.Left, cellData);
            var action = (Action<CellData>)(updated => LoadNextCellState(Direction.Left, updated));
            cellData.OnCellDataChanged += action;
            subscribingEvents.Add((cellData, action));
        }
        // X +方向
        if (gridManager.TryGetCellData(position.X + 1, position.Z, out cellData))
        {
            LoadNextCellState(Direction.Right, cellData);
            var action = (Action<CellData>)(updated => LoadNextCellState(Direction.Right, updated));
            cellData.OnCellDataChanged += action;
            subscribingEvents.Add((cellData, action));
        }
        // Z -方向
        if (gridManager.TryGetCellData(position.X, position.Z - 1, out cellData))
        {
            LoadNextCellState(Direction.Down, cellData);
            var action = (Action<CellData>)(updated => LoadNextCellState(Direction.Down, updated));
            cellData.OnCellDataChanged += action;
            subscribingEvents.Add((cellData, action));
        }
        // Z +方向
        if (gridManager.TryGetCellData(position.X, position.Z + 1, out cellData))
        {
            LoadNextCellState(Direction.Up, cellData);
            var action = (Action<CellData>)(updated => LoadNextCellState(Direction.Up, updated));
            cellData.OnCellDataChanged += action;
            subscribingEvents.Add((cellData, action));
        }
    }

    private void LoadNextCellState(Direction direction, CellData cellData)
    {
        bool isWall = cellData.PlacedDefenderUnit != null && cellData.PlacedDefenderUnit.IsWall;

        bool alreadySet = (_wallState & direction) != 0;

        // 状態が変わったときだけ更新
        if (isWall != alreadySet)
        {
            if (isWall)
            {
                _wallState |= direction;   // ON
            }
            else
            {
                _wallState &= ~direction;  // OFF
            }

            UpdateTexture();
        }
    }

    // _isNextToWallに基づいてGameOjcectのアクティブ状態を切り替えて、壁の見た目を更新する
    private void UpdateTexture()
    {
        if (_wall == null || _crossingWall == null)
        {
            Debug.LogError("WallAnimation: _wall or _crossingWall is not assigned.");
            return;
        }

        if (_position == null)
        {
            Debug.LogError("WallAnimation: Position is not set.");
            return;
        }

        _wall.SetActive(false);
        _crossingWall.SetActive(false);

        bool left = (_wallState & Direction.Left) != 0;
        bool right = (_wallState & Direction.Right) != 0;
        bool up = (_wallState & Direction.Up) != 0;
        bool down = (_wallState & Direction.Down) != 0;

        int horizontal = (left ? 1 : 0) + (right ? 1 : 0);
        int vertical = (up ? 1 : 0) + (down ? 1 : 0);

        // 十字（完全一致のみ）
        if (horizontal >= 1 && vertical >= 1)
        {
            _crossingWall.SetActive(true);
            return;
        }

        // 多い方向を優先
        if (horizontal >= vertical)
        {
            if (horizontal > 0)
            {
                _wall.SetActive(true);
                _wall.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else
        {
            if (vertical > 0)
            {
                _wall.SetActive(true);
                _wall.transform.localRotation = Quaternion.Euler(0, 90, 0);
            }
        }
    }

    private void OnDestroy()
    {
        foreach (var (cellData, action) in subscribingEvents)
        {
            cellData.OnCellDataChanged -= action;
        }
    }
}
