using UnityEngine;
using UnityEngine.UIElements;

public class MoneyUI : MonoBehaviour
{
    [Header("Team Setting")]
    [SerializeField] private GameManager.TeamType _team; // インスペクターで Attack か Defense を選ぶ

    private Label _moneyLabel;

    private void Start()
    {
        // 1. UIのルートを取得
        var root = GetComponent<UIDocument>().rootVisualElement;

        // 2. チームに応じて UXML 内のラベル名を決定
        // 攻撃側なら "AttackMoneyLabel"、防御側なら "DefenseMoneyLabel" を探す
        string labelName = (_team == GameManager.TeamType.Attack) ? "AttackMoneyLabel" : "DefenseMoneyLabel";
        _moneyLabel = root.Q<Label>(labelName);

        if (_moneyLabel == null)
        {
            Debug.LogWarning($"[ResourceUI] {labelName} が UXML 内で見つかりません。名前を確認してください。");
        }

        // 3. GameManager のイベントを購読
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnResourceChanged += RefreshDisplay;
            // 初期表示を反映
            RefreshDisplay();
        }
    }

    private void OnDisable()
    {
        // 購読解除（メモリリーク防止）
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnResourceChanged -= RefreshDisplay;
        }
    }

    /// <summary>
    /// お金が増減したときにUIを更新する
    /// </summary>
    private void RefreshDisplay()
    {
        if (_moneyLabel == null || GameManager.Instance == null) return;

        // 選択されたチームに応じた所持金を GameManager から取得
        int currentMoney = (_team == GameManager.TeamType.Attack) 
            ? GameManager.Instance.AttackMoney 
            : GameManager.Instance.DefenseMoney;

        // ラベルに反映
        _moneyLabel.text = currentMoney.ToString();
    }
}
