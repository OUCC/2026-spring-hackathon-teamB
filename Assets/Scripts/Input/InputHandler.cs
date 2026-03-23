using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [Header("このプレイヤーのチーム")]
    [SerializeField] private GameManager.TeamType _team;

    [Header("連携するコンポーネント")]
    [SerializeField] private PlayerCursorController _cursorController;

    // 移動入力（スティック）が来たとき
    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        Debug.Log($"[InputHandler] {_team} のスティック入力受信: {input}");

        if (_cursorController != null)
        {
            _cursorController.HandleMove(input);
        }
        else
        {
            Debug.LogError($"[InputHandler] {_team} の CursorController が未設定です！");
        }

        if (input.sqrMagnitude > 0.01f)
        {
            // 座標計算のプロ（CursorController）に「動かして」と頼む
            if (_cursorController != null)
            {
                _cursorController.HandleMove(input);
            }
        }
    }

    // 決定ボタン（A/×ボタンなど）が押されたとき
    public void OnJump(InputValue value) 
    {
        if (value.isPressed)
        {
            Debug.Log($"[InputHandler] {_team} の決定ボタン受信");
            // 設置の実行を頼む
            if (_cursorController != null)
            {
                _cursorController.HandleSelect();
            }
        }
    }
}