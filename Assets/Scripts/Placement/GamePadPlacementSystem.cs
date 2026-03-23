using UnityEngine;

public class GridPlacementSystem : MonoBehaviour
{
    // 攻め・守りそれぞれ別のアイテムを持たせるために Inspector でセット
    [SerializeField] private PlaceableItemSO _currentItem; 
    [SerializeField] private GridManager _gridManager;

    private GridCell _currentHoveredCell;
    private GameObject _previewObject;
    private PlaceableItemSO _lastPreviewedItem;

    // 外部（コントローラー）から呼ばれる
    public void UpdateCursorPosition(int x, int z)
    {
        if (_gridManager == null)
        {
            Debug.LogError($"[Placement] {gameObject.name} の GridManager が未設定です！");
            return;
        }
        
        // 警告対応済みのメソッド名に合わせる
        GridCell cell = _gridManager.GetGridCell(x, z); 

        if (cell == null)
        {
        Debug.LogWarning($"[Placement] 座標({x}, {z}) に対応する GridCell が見つかりません！");
        return;
        }
        
        Debug.Log($"[Placement] {gameObject.name} がマス {cell.name} をホバー中");
        
        if (cell != _currentHoveredCell)
        {
            if (_currentHoveredCell != null) _currentHoveredCell.OnHoverExit();
            _currentHoveredCell = cell;
            if (_currentHoveredCell != null) _currentHoveredCell.OnHoverEnter();
        }
        UpdatePreview();
    }

    public void RequestPlacement()
    {
        if (_currentHoveredCell != null) PlaceObjectOnCell(_currentHoveredCell);
    }

    private void UpdatePreview()
    {
        // ここで止まっていないか確認！
        if (_currentItem == null) {
            Debug.LogWarning($"{gameObject.name}: 表示するアイテム(_currentItem)がセットされていません");
            return;
        }
        if (_currentHoveredCell == null) return;

        if (_previewObject == null || _lastPreviewedItem != _currentItem)
        {
            if (_previewObject != null) Destroy(_previewObject);
            
            GameObject prefab = _currentItem.PreviewPrefab != null ? _currentItem.PreviewPrefab : _currentItem.Prefab;
            _previewObject = Instantiate(prefab);
            
            // プレビューが自分自身のレイキャストを邪魔しないように設定
            foreach (var coll in _previewObject.GetComponentsInChildren<Collider>())
                coll.enabled = false;
            
            _lastPreviewedItem = _currentItem;
        }

        if (!_previewObject.activeSelf) _previewObject.SetActive(true);
        _previewObject.transform.position = _currentHoveredCell.transform.position + GetPlacementOffset();
    }

    private Vector3 GetPlacementOffset() => new Vector3(0, (_gridManager != null ? _gridManager.CellSize : 1.0f), 0);

    private void PlaceObjectOnCell(GridCell cell)
    {
        if (_currentItem == null || _gridManager == null) return;

        CellData data = _gridManager.GetCellData(cell.X, cell.Z);
        if (!_currentItem.CanPlace(data)) return;

        GameObject newObject = Instantiate(_currentItem.Prefab, cell.transform.position + GetPlacementOffset(), Quaternion.identity);
        
        // 設置後もレイキャストを邪魔しないように（必要に応じて）
        foreach (var coll in newObject.GetComponentsInChildren<Collider>())
            coll.enabled = false;

        _gridManager.PlaceObject(cell.X, cell.Z, _currentItem, newObject);
        
        if (newObject.TryGetComponent(out IPlaceable placeable))
            placeable.OnPlaced(new Vector2Int(cell.X, cell.Z));
    }
}