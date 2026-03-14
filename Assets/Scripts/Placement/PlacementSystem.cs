using UnityEngine;
using UnityEngine.InputSystem;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private PlaceableItemSO _currentItem;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private GridManager _gridManager;

    private GridCell _currentHoveredCell;
    private GameObject _previewObject;
    private PlaceableItemSO _lastPreviewedItem;

    private void Start()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleMouseInteraction();
        UpdatePreview();
    }

    private void HandleMouseInteraction()
    {
        if (_mainCamera == null || Mouse.current == null) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = _mainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GridCell cell = hit.collider.GetComponentInParent<GridCell>();
            
            // Handle hover state changes
            if (cell != _currentHoveredCell)
            {
                if (_currentHoveredCell != null)
                {
                    _currentHoveredCell.OnHoverExit();
                }

                _currentHoveredCell = cell;
                
                if (_currentHoveredCell != null)
                {
                    _currentHoveredCell.OnHoverEnter();
                }
            }

            // Handle clicking to place object
            if (cell != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                PlaceObjectOnCell(cell);
            }
        }
        else
        {
            // Mouse is not over any cell
            if (_currentHoveredCell != null)
            {
                _currentHoveredCell.OnHoverExit();
                _currentHoveredCell = null;
            }
        }
    }

    private void UpdatePreview()
    {
        if (_currentItem == null || _currentHoveredCell == null)
        {
            if (_previewObject != null) _previewObject.SetActive(false);
            return;
        }

        // If the item changed, destroy the old preview
        if (_lastPreviewedItem != _currentItem)
        {
            if (_previewObject != null) Destroy(_previewObject);
            _previewObject = null;
            _lastPreviewedItem = _currentItem;
        }

        if (_previewObject == null)
        {
            GameObject prefab = _currentItem.PreviewPrefab != null ? _currentItem.PreviewPrefab : _currentItem.Prefab;
            _previewObject = Instantiate(prefab);
            
            // Disable all colliders on the preview so it doesn't block the raycast
            foreach (var coll in _previewObject.GetComponentsInChildren<Collider>())
            {
                coll.enabled = false;
            }

            // Optional: You could apply a semi-transparent material here
        }

        _previewObject.SetActive(true);
        _previewObject.transform.position = _currentHoveredCell.transform.position + GetPlacementOffset();
    }

    private Vector3 GetPlacementOffset()
    {
        float cellSize = _gridManager != null ? _gridManager.CellSize : 1.0f;
        return new Vector3(0, cellSize, 0);
    }

    private void PlaceObjectOnCell(GridCell cell)
    {
        if (_currentItem == null)
        {
            Debug.LogWarning("PlacementSystem: No item selected for placement.");
            return;
        }
        if (_gridManager == null)
        {
            Debug.LogError("PlacementSystem: GridManager reference is missing.");
            return;
        }

        CellData data = _gridManager.GetCellData(cell.X, cell.Z);
        if (!_currentItem.CanPlace(data))
        {
            // Optionally provide visual feedback why it failed
            return;
        }

        GameObject newObject = Instantiate(_currentItem.Prefab, cell.transform.position + GetPlacementOffset(), Quaternion.identity);
        
        // Disable all colliders on the placed object so it doesn't block raycasts to other cells
        foreach (var coll in newObject.GetComponentsInChildren<Collider>())
        {
            coll.enabled = false;
        }

        // Notify logical grid
        _gridManager.PlaceObject(cell.X, cell.Z, _currentItem, newObject);
        
        // Handle interface if present
        IPlaceable placeable = newObject.GetComponent<IPlaceable>();
        if (placeable != null)
        {
            placeable.OnPlaced(new Vector2Int(cell.X, cell.Z));
        }

        // Update hover visual for the now occupied cell
        cell.OnHoverEnter(); 
    }
}
