using System.Collections.Generic;

using UnityEngine;

[ExecuteAlways]
public class GridManager : MonoBehaviour
{
    private int _ground_layer;

    [Header("Grid Settings")]
    [SerializeField] private int _width = 10;
    [SerializeField] private int _height = 10;
    [SerializeField] private float _cellSize = 1f;
    [SerializeField] private GridCell _cellPrefab;
    [Header("Grid Visual Settings")]
    [SerializeField] private float _outlineThickness = 0.5f; // In Voxel Units
    [SerializeField] private Color _outlineColor = new Color(0f, 0f, 0f, 0.5f);
    [SerializeField] private float _voxelResolution = 16f;

    public int Width => _width;
    public int Height => _height;
    public float CellSize => _cellSize;

    private GridCell[,] _visualGrid;
    private CellData[,] _logicalGrid;

    public event System.Action<int, int, PlaceableItemSO> OnObjectPlaced;
    public event System.Action<int, int> OnObjectRemoved;

    private int _prevWidth = -1;
    private int _prevHeight = -1;
    private float _prevCellSize = -1f;

    private void Awake()
    {
        GenerateGrid();
        OnObjectPlaced += (x, z, item) => CalcDirectionToCell(x, z);
        OnObjectRemoved += (x, z) => CalcDirectionToCell(x, z);
    }

    private void OnValidate()
    {
        if (_width != _prevWidth || _height != _prevHeight || _cellSize != _prevCellSize)
        {
            _prevWidth = _width;
            _prevHeight = _height;
            _prevCellSize = _cellSize;

            // Delay generation slightly to avoid warnings when destroying/instantiating in Editor
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this == null) return;
                GenerateGrid();
            };
#endif
        }
        else
        {
            // Visual changes can be applied immediately
            UpdateGridVisuals();
        }
    }

    public void GenerateGrid()
    {
        _ground_layer = LayerMask.NameToLayer("Ground");

        if (_cellPrefab == null)
        {
            Debug.LogError("GridManager: Cell Prefab is not assigned.");
            return;
        }

        ClearGrid();

        _visualGrid = new GridCell[_width, _height];
        _logicalGrid = new CellData[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                Vector3 position = new Vector3(x * _cellSize, 0, z * _cellSize);
                GridCell cell = Instantiate(_cellPrefab, position, Quaternion.identity, transform);
                cell.gameObject.layer = _ground_layer;
                cell.name = $"Cell_{x}_{z}";
                cell.transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);
                cell.Initialize(x, z);
                cell.SetOutlineSettings(_outlineThickness, _outlineColor, _voxelResolution, _cellSize);

                _visualGrid[x, z] = cell;
                _logicalGrid[x, z] = new CellData(x, z, cell);
                cell.CellData = _logicalGrid[x, z];
            }
        }

        CalcDirectionToCell(-1, -1);
    }

    private void UpdateGridVisuals()
    {
        // Even if Array is lost due to script recompile, grab children directly 
        foreach (var cell in GetComponentsInChildren<GridCell>())
        {
            cell.transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);

            // Re-center just in case cell size changed and X/Z indexes exist
            Vector3 position = new(cell.X * _cellSize, 0, cell.Z * _cellSize);
            cell.transform.position = position;

            cell.SetOutlineSettings(_outlineThickness, _outlineColor, _voxelResolution, _cellSize);
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.SceneView.RepaintAll();
        }
#endif
    }

    public void ClearGrid()
    {
        // Destroy all current child objects
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (Application.isPlaying)
            {
                Destroy(child);
            }
            else
            {
                DestroyImmediate(child);
            }
        }

        if (_visualGrid != null)
        {
            _visualGrid = null;
        }
        if (_logicalGrid != null)
        {
            _logicalGrid = null;
        }
    }

    [System.Obsolete("Use GetGridCell instead. This method will be removed in future versions.")]
    public GridCell GetVisualCell(int x, int z) => GetGridCell(x, z);

    public GridCell GetGridCell(int x, int z)
    {
        if (_visualGrid == null) RebuildGridArrays();
        return _visualGrid[x, z];
    }

    public CellData GetCellData(int x, int z)
    {
        if (_logicalGrid == null) RebuildGridArrays();
        return _logicalGrid[x, z];
    }

    private void RebuildGridArrays()
    {
        _visualGrid = new GridCell[_width, _height];
        _logicalGrid = new CellData[_width, _height];

        foreach (var cell in GetComponentsInChildren<GridCell>())
        {
            if (_visualGrid == null)
                _visualGrid[cell.X, cell.Z] = cell;

            if (_logicalGrid == null)
            {
                _logicalGrid[cell.X, cell.Z] = new CellData(cell.X, cell.Z, cell);
                cell.CellData = _logicalGrid[cell.X, cell.Z];
            }
        }
    }

    public void PlaceObject(int x, int z, PlaceableItemSO item, GameObject spawnedObject)
    {
        CellData data = GetCellData(x, z);
        if (data == null) return;

        data.PlacedObject = spawnedObject;
        data.ItemType = item;

        GridCell visual = GetGridCell(x, z);
        if (visual != null) visual.IsOccupied = true;

        OnObjectPlaced?.Invoke(x, z, item);
    }

    public void RemoveObject(int x, int z)
    {
        CellData data = GetCellData(x, z);
        if (data == null || data.PlacedObject == null) return;

        GameObject objToDestroy = data.PlacedObject;

        // Handle interface notify
        IPlaceable placeable = objToDestroy.GetComponent<IPlaceable>();
        placeable?.OnRemoved();

        if (Application.isPlaying) Destroy(objToDestroy);
        else DestroyImmediate(objToDestroy);

        data.PlacedObject = null;
        data.ItemType = null;

        GridCell visual = GetGridCell(x, z);
        if (visual != null) visual.IsOccupied = false;

        OnObjectRemoved?.Invoke(x, z);
    }

    public Vector3 GetGridCenter()
    {
        return new Vector3((_width - 1) * _cellSize / 2f, 0, (_height - 1) * _cellSize / 2f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        // Draw each cell as a wireframe cube
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                Vector3 position = new(x * _cellSize, 0, z * _cellSize);
                // The position is at the center of the cell
                Gizmos.DrawWireCube(position, new Vector3(_cellSize, _cellSize, _cellSize));
            }
        }
    }


    /// <summary>
    /// _logicalGridの各CellData.NextCellToCastleを再計算。
    /// </summary>
    /// <param name="x">更新されたタイルのX。<see langword="-1"/>の時は、全て再計算が必要</param>
    /// <param name="z">更新されたタイルのZ。<see langword="-1"/>の時は、全て再計算が必要</param>
    private void CalcDirectionToCell(int x, int z)
    {
        if (_logicalGrid == null) return;
        CellData castleCell = Castle;
        /*for(int i = 0; i < _width; i++)
        {
            for(int j=0; j< _height; j++)
            {
                _logicalGrid[i, j].NextCellToCastle = null;
            }
            
        }
        */
        Queue<CellData> queue = new Queue<CellData>();
        queue.Enqueue(castleCell);
        int[,] directions =
        {
            {1,0},
            {-1,0},
            {0,1},
            {0,-1}
        };
        while (queue.Count > 0)
        {
            CellData nownode = queue.Dequeue();
            for (int dir = 0; dir < 4; dir++)
            {
                int nextX = nownode.X + directions[dir, 0];
                int nextZ = nownode.Z + directions[dir, 1];
                if (nextX < 0 || nextX >= _width || nextZ < 0 || nextZ >= _height)
                {
                    continue;
                }
                if (CanEnter(nextX, nextZ))
                {
                    CellData nextNode = GetCellData(nextX, nextZ);
                    if (nextNode.NextCellToCastle == null && nextNode != castleCell)
                    {
                        nextNode.NextCellToCastle = nownode;
                        queue.Enqueue(nextNode);
                    }
                }
            }
        }
        // TODO: _logicalGridの各CellData.NextCellToCastleに、城へ向かうための次のセルを設定する。
    }
    public CellData Castle
    {
        get
        {
            return GetCellData(0, _height / 2);
        }
    }
    public bool CanEnter(int x, int z)
    {
        return _logicalGrid[x, z].CanEnter;
    }
}
