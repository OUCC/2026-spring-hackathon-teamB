using UnityEngine;
using TMPro;

public class GameHUDUI : MonoBehaviour
{
    [Header("Money Texts")]
    [SerializeField] private TextMeshProUGUI attackMoneyText;
    [SerializeField] private TextMeshProUGUI defenseMoneyText;

    [Header("Timer Text")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Spend Cost")]
    [SerializeField] private int attackSpendCost = 10;
    [SerializeField] private int defenseSpendCost = 10;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnResourceChanged += UpdateMoneyUI;
            GameManager.Instance.OnTimeChanged += UpdateTimerUI;
            GameManager.Instance.OnStateChanged += OnStateChanged;
        }
    }

    private void Start()
    {
        RefreshAll();
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnResourceChanged -= UpdateMoneyUI;
            GameManager.Instance.OnTimeChanged -= UpdateTimerUI;
            GameManager.Instance.OnStateChanged -= OnStateChanged;
        }
    }

    private void OnStateChanged(GameManager.GameState state)
    {
        RefreshAll();
    }

    private void RefreshAll()
    {
        UpdateMoneyUI();
        UpdateTimerUI();
    }

    private void UpdateMoneyUI()
    {
        if (GameManager.Instance == null) return;

        if (attackMoneyText != null)
            attackMoneyText.text = $"ATK : ${GameManager.Instance.AttackMoney}";

        if (defenseMoneyText != null)
            defenseMoneyText.text = $"DEF : ${GameManager.Instance.DefenseMoney}";
    }

    private void UpdateTimerUI()
    {
        if (GameManager.Instance == null || timerText == null) return;

        float remain = GameManager.Instance.RemainingTime;
        int totalSeconds = Mathf.CeilToInt(remain);

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    // =========================
    // Button から呼ぶ用
    // =========================

    public void SpendAttackMoney()
    {
        if (GameManager.Instance == null) return;

        bool success = GameManager.Instance.SpendMoney(
            GameManager.TeamType.Attack,
            attackSpendCost
        );

        if (!success)
        {
            Debug.Log("Attack側のお金が足りないか、ゲーム中ではありません");
        }
    }

    public void SpendDefenseMoney()
    {
        if (GameManager.Instance == null) return;

        bool success = GameManager.Instance.SpendMoney(
            GameManager.TeamType.Defense,
            defenseSpendCost
        );

        if (!success)
        {
            Debug.Log("Defense側のお金が足りないか、ゲーム中ではありません");
        }
    }

    public void SpendAttackMoney(int cost)
    {
        if (GameManager.Instance == null) return;

        bool success = GameManager.Instance.SpendMoney(
            GameManager.TeamType.Attack,
            cost
        );

        if (!success)
        {
            Debug.Log("Attack側のお金が足りないか、ゲーム中ではありません");
        }
    }

    public void SpendDefenseMoney(int cost)
    {
        if (GameManager.Instance == null) return;

        bool success = GameManager.Instance.SpendMoney(
            GameManager.TeamType.Defense,
            cost
        );

        if (!success)
        {
            Debug.Log("Defense側のお金が足りないか、ゲーム中ではありません");
        }
    }
}