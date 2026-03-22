using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [Header("このプレイヤーのチーム")]
    [SerializeField] private GameManager.TeamType _team;

    // ==========================================
    // Player Input が「Move」アクションを検知した時に自動で呼ばれる
    // ==========================================
    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        
        // スティックが少しでも倒れていたらログを出す
        if (input.sqrMagnitude > 0.01f)
        {
            Debug.Log($"[{_team}] 移動入力: X={input.x:F2}, Y={input.y:F2}");
            // TODO: ゆくゆくはここで「グリッドのカーソルを動かす」処理を呼ぶ
        }
    }

    // ==========================================
    // Player Input が「Jump」アクション（A/×ボタンなど）を検知した時に呼ばれる
    // ==========================================
    public void OnJump(InputValue value) 
    {
        if (value.isPressed) // 押された瞬間か？
        {
            Debug.Log($"[{_team}] 決定ボタンが押されました！");
            // TODO: ゆくゆくはここで「ユニットを配置する」処理を呼ぶ
        }
    }
}