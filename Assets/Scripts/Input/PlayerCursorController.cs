using UnityEngine;

public class PlayerCursorController : MonoBehaviour
{
    [SerializeField] private GridPlacementSystem _placementSystem;
    [SerializeField] private Vector2Int _currentPos = Vector2Int.zero;
    private bool _canMove = true;

    private void Start()
    {
        // 開始時に一度だけ呼び出して、初期位置にプレビューを表示させる
        Invoke(nameof(InitialSync), 0.1f); // GridManagerの生成を待つために少し遅らせる
    }

    private void InitialSync()
    {
        if (_placementSystem != null)
            _placementSystem.UpdateCursorPosition(_currentPos.x, _currentPos.y);
    }

    public void HandleMove(Vector2 input)
    {
        if (!_canMove)
        {
            if (input.magnitude < 0.2f) _canMove = true;
            return;
        }

        if (input.magnitude > 0.5f)
        {
            Vector2Int oldPos = _currentPos;
            // 上下左右の移動判定
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                _currentPos.x += (input.x > 0) ? 1 : -1;
            else
                _currentPos.y += (input.y > 0) ? 1 : -1;

            // 0〜9の範囲に制限（GridManagerのWidth/Heightに合わせるのが理想）
            _currentPos.x = Mathf.Clamp(_currentPos.x, 0, 9);
            _currentPos.y = Mathf.Clamp(_currentPos.y, 0, 9);

            Debug.Log($"[Cursor] {gameObject.name} 座標更新: {oldPos} -> {_currentPos}");

            if (_placementSystem != null)
            {
                _placementSystem.UpdateCursorPosition(_currentPos.x, _currentPos.y);
                _canMove = false;
            }
        }
    }

    public void HandleSelect() => _placementSystem.RequestPlacement();
}
