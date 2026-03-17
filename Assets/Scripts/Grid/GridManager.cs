using System.Collections.Generic;

using UnityEngine;

[ExecuteAlways]
public class GridManager : MonoBehaviour
{
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

    public System.Action<int, int, PlaceableItemSO> OnObjectPlaced;
    public System.Action<int, int> OnObjectRemoved;

    private int _prevWidth = -1;
    private int _prevHeight = -1;
    private float _prevCellSize = -1f;

    private void Start()
    {
        GenerateGrid();
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
                Vector3 position = new(x * _cellSize, 0, z * _cellSize);
                GridCell cell = Instantiate(_cellPrefab, position, Quaternion.identity, transform);
                cell.name = $"Cell_{x}_{z}";
                cell.transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);
                cell.Initialize(x, z);
                cell.SetOutlineSettings(_outlineThickness, _outlineColor, _voxelResolution, _cellSize);

                _visualGrid[x, z] = cell;
                _logicalGrid[x, z] = new CellData(x, z);
            }
        }
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
                _logicalGrid[cell.X, cell.Z] = new CellData(cell.X, cell.Z);
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
}
