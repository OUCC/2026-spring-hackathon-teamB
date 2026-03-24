using UnityEngine;

public class PlayerCursorController : MonoBehaviour
{
    [Header("Team Settings")]
    [SerializeField] private GameManager.TeamType _team; // Attack か Defense かを指定

    [Header("References")]
    [SerializeField] private UnitListUI _unitListUI; // HUDで選択中のユニットデータを取得
    [SerializeField] private AttackerUnitSpawner _spawner; // ユニット生成を実行するコンポーネント

    [Header("Movement Settings")]
    [SerializeField] private float _moveThreshold = 0.5f; // スティックをどれくらい倒したら動くか
    private bool _canMove = true; // 押しっぱなしによる連続移動を防ぐフラグ

    /// <summary>
    /// InputHandler の決定ボタン (OnJump) から呼ばれる設置リクエスト
    /// </summary>
    public void HandleSelect()
    {
        // 1. 現在のグリッド座標を計算してログに表示
        Vector2Int gridPos = GetGridPosition();
        Debug.Log($"<color=cyan>[Select]</color> {_team} チームがタイル {gridPos} で決定ボタンを押しました。");

        // 2. UIから現在選択中のユニット情報を取得
        AttackerUnitData selectedData = _unitListUI.GetSelectedUnitData();
        
        if (selectedData == null) 
        {
            Debug.LogWarning($"[PlayerCursorController] {_team}: 選択されているユニットがありません。");
            return;
        }

        // 3. スポナーに生成を依頼（お金のチェック等はスポナー側で実行される前提）
        // 現在のカーソル位置 (transform.position) をそのまま渡します
        _spawner.Spawn(transform.position, selectedData);
    }

    /// <summary>
    /// スティックまたは十字キー入力による「1マスずつ」の移動処理
    /// </summary>
    /// <param name="direction">入力ベクトル</param>
    public void HandleMove(Vector2 direction)
    {
        // 入力の強さがしきい値を超えているか
        if (direction.magnitude > _moveThreshold)
        {
            if (_canMove)
            {
                // 入力が大きい方の軸（上下 or 左右）を判定して1マス分動かす
                Vector3 moveVector = Vector3.zero;
                
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    // 左右移動を優先
                    moveVector.x = direction.x > 0 ? 1 : -1;
                }
                else
                {
                    // 上下移動を優先 (Unityの平面ならZ軸)
                    moveVector.z = direction.y > 0 ? 1 : -1;
                }

                // 座標を更新
                transform.position += moveVector;

                // 連続移動をロック
                _canMove = false;

                // 移動後の座標をコンソールに表示
                Debug.Log($"<color=white>[Move]</color> {_team} カーソル位置: {GetGridPosition()}");
            }
        }
        else
        {
            // スティックが中央付近に戻ったら、再び移動できるようにする
            _canMove = true;
        }
    }

    /// <summary>
    /// 現在のワールド座標を四捨五入して、整数のグリッド座標を返します
    /// </summary>
    private Vector2Int GetGridPosition()
    {
        return new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.z)
        );
    }
}
