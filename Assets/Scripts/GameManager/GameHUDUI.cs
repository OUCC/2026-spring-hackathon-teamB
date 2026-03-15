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

    private GameManager gm;

    private void Start()
    {
        gm = GameManager.Instance;

        if (gm == null)
        {
            Debug.LogError("GameManager.Instance が見つかりません");
            return;
        }

        gm.OnResourceChanged += UpdateMoneyUI;
        gm.OnTimeChanged += UpdateTimerUI;
        gm.OnStateChanged += OnStateChanged;

        RefreshAll();
    }

    private void OnDisable()
    {
        if (gm == null) return;

        gm.OnResourceChanged -= UpdateMoneyUI;
        gm.OnTimeChanged -= UpdateTimerUI;
        gm.OnStateChanged -= OnStateChanged;
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
        if (gm == null) return;

        if (attackMoneyText != null)
            attackMoneyText.text = $"ATK : ${gm.AttackMoney}";

        if (defenseMoneyText != null)
            defenseMoneyText.text = $"DEF : ${gm.DefenseMoney}";
    }

    private void UpdateTimerUI()
    {
        if (gm == null || timerText == null) return;

        float remain = gm.RemainingTime;
        int totalSeconds = Mathf.CeilToInt(remain);

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void SpendAttackMoney()
    {
        if (gm == null) return;

        bool success = gm.SpendMoney(GameManager.TeamType.Attack, attackSpendCost);

        if (!success)
        {
            Debug.Log("Attack側のお金が足りないか、ゲーム中ではありません");
        }
    }

    public void SpendDefenseMoney()
    {
        if (gm == null) return;

        bool success = gm.SpendMoney(GameManager.TeamType.Defense, defenseSpendCost);

        if (!success)
        {
            Debug.Log("Defense側のお金が足りないか、ゲーム中ではありません");
        }
    }

    public void SpendAttackMoney(int cost)
    {
        if (gm == null) return;

        bool success = gm.SpendMoney(GameManager.TeamType.Attack, cost);

        if (!success)
        {
            Debug.Log("Attack側のお金が足りないか、ゲーム中ではありません");
        }
    }

    public void SpendDefenseMoney(int cost)
    {
        if (gm == null) return;

        bool success = gm.SpendMoney(GameManager.TeamType.Defense, cost);

        if (!success)
        {
            Debug.Log("Defense側のお金が足りないか、ゲーム中ではありません");
        }
    }
}