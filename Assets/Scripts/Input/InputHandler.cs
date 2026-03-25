using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [Header("このプレイヤーのチーム")]
    [SerializeField] private GameManager.TeamType _team; //

    [Header("連携するコンポーネント")]
    [SerializeField] private PlayerCursorController _cursorController; //
    
    public void OnNext(InputValue value)
    {
        // カーソルコントローラーに「次へ」と伝える
        if (value.isPressed && _cursorController != null)
        {
            _cursorController.HandleUINext(); 
        }
    }

    public void OnPrevious(InputValue value)
    {
        // カーソルコントローラーに「前へ」と伝える
        if (value.isPressed && _cursorController != null)
        {
            _cursorController.HandleUIPrevious();
        }
    }

    /// <summary>
    /// 移動入力 (Action: Move)
    /// </summary>
    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>(); //

        // カーソルコントローラーに入力を伝える。1マス移動の判定はあちらで行う。
        if (_cursorController != null)
        {
            _cursorController.HandleMove(input); //
        }
    }


    /// <summary>
    /// 決定ボタン (Action: Jump)
    /// </summary>
    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log($"[InputHandler] {_team} の決定ボタン受信"); //
            if (_cursorController != null)
            {
                // カーソル側に「今選んでいる場所で決定」と伝える
                _cursorController.HandleSelect(); //
            }
        }
    }

    /// <summary>
    /// 攻撃アクション (Action: Attack)
    /// 現状はログ出力のみ。必要に応じて機能を追加。
    /// </summary>
    public void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log($"[InputHandler] {_team} の攻撃アクション(Attack)受信"); //
        }
    }

    // --- 古い HandleSelect や不要なフィールド (_unitListUI, _spawner) は削除しました ---
}