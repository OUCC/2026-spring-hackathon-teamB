using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerCursorController : MonoBehaviour

{
    [Header("Team Settings")]
    [SerializeField] private GameManager.TeamType _team;
    
    [Header("Map Settings")]
    // インスペクターからマップの横幅・縦幅を設定（例: 20x15）
    [SerializeField] private Vector2Int _mapSize = new Vector2Int(20, 15);
    // マップの開始地点（通常は 0,0）
    [SerializeField] private Vector2Int _mapOrigin = new Vector2Int(0, 0);

    [Header("References (Common)")]
    [SerializeField] private float _moveThreshold = 0.5f;
    private bool _canMove = true;

    [Header("References (Attack Team)")]
    [SerializeField] private UnitListUI _attackerUI;
    [SerializeField] private AttackerUnitSpawner _attackerSpawner;

    [Header("References (Defense Team)")]
    // 防御側専用のUIスクリプト（前回作成したもの）
    [SerializeField] private DefencerUnitListUI _defencerUI;
    [SerializeField] private DefencerUnitSpawner _defencerSpawner;

    private void Update()
    {
        // テスト用：マウスの左クリックを検知
        if (Mouse.current == null) return;
        if (_team == GameManager.TeamType.Attack)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Debug.Log("<color=cyan>[Attack Team]</color> アタック側左クリックで設置");
                HandleMouseClickSpawn();
            }
        }
        else if (_team == GameManager.TeamType.Defense)
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                Debug.Log("<color=green>[Defense Team]</color> ディフェンス側右クリックで設置");
                HandleMouseClickSpawn();
            }
        }
    }
    /// <summary>
    /// マウスのクリック位置を制限してスナップさせる
    /// </summary>
        private void HandleMouseClickSpawn()
    {
        // 1. スクリーン座標をワールド座標に変換
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        
        // XZ平面（地面）との当たり判定を計算
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // 2. クリックした位置をグリッドにスナップさせる
            Vector3 gridPosition = new Vector3(
                Mathf.RoundToInt(hit.point.x),
                transform.position.y, // カーソルと同じ高さ
                Mathf.RoundToInt(hit.point.z)
            );

            // 3. カーソルの位置をその場所に瞬間移動させる（視覚的なフィードバック）
            transform.position = gridPosition;

            // 4. 既存の設置処理を呼び出す
            Debug.Log($"<color=yellow>[Debug Click]</color> {gridPosition} にクリック設置を試みます。");
            HandleSelect(); // 既存のお金チェックや生成ロジックをそのまま利用
        }
    }


    public void HandleUINext()
    {
        if (_team == GameManager.TeamType.Attack && _attackerUI != null)
        {
            _attackerUI.SelectNext(); // アタッカー側を操作
        }
        else if (_team == GameManager.TeamType.Defense && _defencerUI != null)
        {
            _defencerUI.SelectNext(); // ディフェンダー側を操作
        }
    }

    public void HandleUIPrevious()
    {
        if (_team == GameManager.TeamType.Attack && _attackerUI != null)
        {
            _attackerUI.SelectPrevious();
        }
        else if (_team == GameManager.TeamType.Defense && _defencerUI != null)
        {
            _defencerUI.SelectPrevious();
        }
    }

    /// <summary>
    /// 決定ボタンが押された時の処理
    /// </summary>
    public void HandleSelect()
    {
        Vector2Int gridPos = GetGridPosition();

        if (_team == GameManager.TeamType.Attack)
        {
            HandleAttackSpawn(gridPos);
        }
        else
        {
            HandleDefenseSpawn(gridPos);
        }
    }

    private void HandleAttackSpawn(Vector2Int gridPos)
    {
        if (_attackerUI == null || _attackerSpawner == null) return;

        AttackerUnitData data = _attackerUI.GetSelectedUnitData();
        if (data != null)
        {
            // アタッカーを生成
            _attackerSpawner.Spawn(transform.position + new Vector3(0, 0.1f, 0), data);
        }
    }

    private void HandleDefenseSpawn(Vector2Int gridPos)
    {
        if (_defencerUI == null || _defencerSpawner == null) return;

        // 防御側UIから選択中のデータを取得
        // ※DefencerUnitListUIにGetSelectedData()を実装している前提
        DefencerUnitData data = _defencerUI.GetSelectedUnitData();
        if (data != null)
        {
            // 防御ユニット（タレット等）を生成
            _defencerSpawner.Spawn(transform.position, data);
        }
    }

    /// <summary>
    /// スティック入力による移動処理（共通）
    /// </summary>
/// <summary>
    /// スティックまたは十字キー入力による移動
    /// </summary>
    public void HandleMove(Vector2 direction)
    {
        if (direction.magnitude > _moveThreshold)
        {
            if (_canMove)
            {
                Vector3 moveVector = Vector3.zero;
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                    moveVector.x = direction.x > 0 ? 1 : -1;
                else
                    moveVector.z = direction.y > 0 ? 1 : -1;

                // --- 境界チェックの追加 ---
                Vector3 targetPosition = transform.position + moveVector;
                if (IsWithinBounds(targetPosition))
                {
                    transform.position = targetPosition;
                    Debug.Log($"{_team} カーソル位置: {GetGridPosition()}");
                }
                else
                {
                    Debug.Log("<color=orange>マップ範囲外です</color>");
                }

                _canMove = false;
            }
        }
        else
        {
            _canMove = true;
        }
    }

    /// <summary>
    /// 指定された座標がマップ範囲内にあるか判定
    /// </summary>
    private bool IsWithinBounds(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x);
        int z = Mathf.RoundToInt(position.z);

        return x >= _mapOrigin.x && x < _mapOrigin.x + _mapSize.x &&
               z >= _mapOrigin.y && z < _mapOrigin.y + _mapSize.y;
    }

    private Vector2Int GetGridPosition()
    {
        return new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.z)
        );
    }
}
