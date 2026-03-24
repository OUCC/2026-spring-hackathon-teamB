using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [Header("このプレイヤーのチーム")]
    [SerializeField] private GameManager.TeamType _team;

    [Header("連携するコンポーネント")]
    [SerializeField] private PlayerCursorController _cursorController;
    [SerializeField] private UnitListUI myUnitListUI;

    [Header("References")]
    [SerializeField] private UnitListUI _unitListUI; // HUDから選択中のデータを取るため
    [SerializeField] private AttackerUnitSpawner _spawner; // 実際に生成する工場
    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        if (_cursorController != null && input.sqrMagnitude > 0.01f)
        {
            _cursorController.HandleMove(input);
        }
    }
    public void OnNext(InputValue value)
    {
        if (value.isPressed && myUnitListUI != null)
        {
            myUnitListUI.SelectNext();
        }
    }
    public void OnPrevious(InputValue value)
    {
        if (value.isPressed && myUnitListUI != null)
        {
            myUnitListUI.SelectPrevious();
        }
    }
    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log($"[InputHandler] {_team} の決定アクション(Jump)受信");
            if (_cursorController != null)
            {
                _cursorController.HandleSelect();
            }
        }
    }

    public void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log($"[InputHandler] {_team} の攻撃アクション(Attack)受信");
            // 必要に応じてここに召喚処理などを追加
        }
    }
    
    public void HandleSelect()
    {
        // 1. UIから現在選んでいるユニットの設計図を取得
        AttackerUnitData selectedData = _unitListUI.GetSelectedUnitData();
        if (selectedData == null) return;

        // 2. お金が足りるかチェックし、足りるなら消費する
        // SpendMoney は成否を bool で返してくれるので便利です
        bool canAfford = GameManager.Instance.SpendMoney(_team, selectedData.SummonCost);

        if (canAfford)
        {
            // 3. お金が払えたので、スポナーに召喚を依頼する
            // 第1引数は現在のカーソルの座標、第2引数はユニットのデータ
            _spawner.Spawn(transform.position, selectedData); 

            Debug.Log($"[Placement] {_team} が {selectedData.UnitName} を召喚しました！残り金: {GameManager.Instance.AttackMoney}");
        }
        else
        {
            // お金が足りない時の演出（SEを鳴らすなど）をここに入れる
            Debug.LogWarning($"[Placement] {_team} のお金が足りません！必要: {selectedData.SummonCost}");
        }
    }

    public void HandleMove(Vector2 direction)
    {
        // ここには既存のカーソル移動ロジックが入っている想定
        // transform.position += (Vector3)direction; など
    }
}