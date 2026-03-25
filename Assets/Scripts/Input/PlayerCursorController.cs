using UnityEngine;

public class PlayerCursorController : MonoBehaviour
{
    [Header("Team Settings")]
    [SerializeField] private GameManager.TeamType _team; 

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
            _attackerSpawner.Spawn(transform.position, data); 
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

                transform.position += moveVector;
                _canMove = false;
                Debug.Log($"{_team} カーソル: {GetGridPosition()}");
            }
        }
        else
        {
            _canMove = true;
        }
    }

    private Vector2Int GetGridPosition()
    {
        return new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.z)
        );
    }
}
