using UnityEngine;

public class GridCell : MonoBehaviour
{
    public int X { get; private set; }
    public int Z { get; private set; }
    public bool IsOccupied { get; set; }

    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Color _defaultColor = Color.white;
    [SerializeField] private Color _hoverColor = Color.cyan;
    [SerializeField] private Color _occupiedHoverColor = Color.red;

    private CellData _cellData;
    public CellData CellData
    {
        get
        {
            if ( _cellData == null)
            {
                _cellData = GameManager.Instance.GridManager.GetCellData(X, Z);
            }
            
            if (_cellData == null)
            {
                Debug.LogError($"CellData for GridCell at ({X}, {Z}) not found");
            }

            return _cellData;
        }
        set { _cellData = value; }
    }

    private void Awake()
    {
        if (_renderer == null)
            _renderer = GetComponentInChildren<MeshRenderer>();
    }

    public void Initialize(int x, int z)
    {
        X = x;
        Z = z;
        IsOccupied = false;

        // Ensure MaterialPropertyBlock is ready
        if (_propBlock == null) _propBlock = new MaterialPropertyBlock();

        SetColor(_defaultColor);
    }

    private MaterialPropertyBlock _propBlock;

    public void SetOutlineSettings(float thickness, Color outlineColor, float resolution, float cellSize)
    {
        if (_renderer == null) _renderer = GetComponentInChildren<MeshRenderer>();
        if (_renderer == null) return;

        if (_propBlock == null) _propBlock = new MaterialPropertyBlock();
        _renderer.GetPropertyBlock(_propBlock);

        _propBlock.SetFloat("_OutlineThickness", thickness);
        _propBlock.SetColor("_OutlineColor", outlineColor);
        _propBlock.SetFloat("_VoxelResolution", resolution);
        _propBlock.SetFloat("_CellSize", cellSize);

        _renderer.SetPropertyBlock(_propBlock);
    }

    public void OnHoverEnter()
    {
        SetColor(IsOccupied ? _occupiedHoverColor : _hoverColor);
    }

    public void OnHoverExit()
    {
        SetColor(_defaultColor);
    }

    private void SetColor(Color color)
    {
        if (_renderer == null) _renderer = GetComponentInChildren<MeshRenderer>();
        if (_renderer != null)
        {
            if (_propBlock == null) _propBlock = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(_propBlock);

            // Our custom shader uses _BaseColor
            _propBlock.SetColor("_BaseColor", color);
            // In case we fall back to a standard material
            _propBlock.SetColor("_Color", color);

            _renderer.SetPropertyBlock(_propBlock);
        }
    }
}
