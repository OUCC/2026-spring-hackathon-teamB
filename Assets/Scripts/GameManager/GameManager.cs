using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState
    {
        Ready,
        Playing,
        GameEnd,
        Result
    }
    public enum TeamType
    {
        Attack,
        Defense
    }
    public enum WinType
    {
        None,
        AttackWin,
        DefenseWin,
        Surrender
    }
    [Header("Game Time")]
    [SerializeField] private float timeLimit = 180f;   // 3分
    [SerializeField] private float elapsedTime = 0f;

    [Header("Tick")]
    [SerializeField] private float tickInterval = 1f;
    private float tickTimer = 0f;

    [Header("Attack Team")]
    [SerializeField] private int attackMoney = 100;
    [SerializeField] private int attackIncome = 10;

    [Header("Defense Team")]
    [SerializeField] private int defenseMoney = 100;
    [SerializeField] private int defenseIncome = 10;

    [Header("State")]
    [SerializeField] private GameState currentState = GameState.Ready;
    [SerializeField] private WinType winType = WinType.None;

    public int AttackMoney => attackMoney;
    public int AttackIncome => attackIncome;
    public int DefenseMoney => defenseMoney;
    public int DefenseIncome => defenseIncome;
    public float ElapsedTime => elapsedTime;
    public float RemainingTime => Mathf.Max(0f, timeLimit - elapsedTime);
    public GameState CurrentState => currentState;
    public WinType CurrentWinType => winType;

    public event Action OnResourceChanged;
    public event Action OnTimeChanged;
    public event Action<GameState> OnStateChanged;
    public event Action<WinType> OnGameEnded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        ChangeState(GameState.Ready);
    }

    private void FixedUpdate()
    {
        if (currentState != GameState.Playing) return;

        float dt = Time.fixedDeltaTime;

        elapsedTime += dt;
        OnTimeChanged?.Invoke();

        if (elapsedTime >= timeLimit)
        {
            EndGame(WinType.DefenseWin);
            return;
        }

        tickTimer += dt;
        while (tickTimer >= tickInterval)
        {
            tickTimer -= tickInterval;
            OnTick();
        }
    }

    public void StartGame()
    {
        elapsedTime = 0f;
        tickTimer = 0f;
        winType = WinType.None;

        ChangeState(GameState.Playing);
        OnResourceChanged?.Invoke();
        OnTimeChanged?.Invoke();
    }

    private void OnTick()
    {
        attackMoney += attackIncome;
        defenseMoney += defenseIncome;

        OnResourceChanged?.Invoke();
    }

    public bool CanSpendMoney(TeamType team, int amount)
    {
        if (amount < 0) return false;

        return team switch
        {
            TeamType.Attack => attackMoney >= amount,
            TeamType.Defense => defenseMoney >= amount,
            _ => false
        };
    }

    public bool SpendMoney(TeamType team, int amount)
    {
        if (currentState != GameState.Playing) return false;
        if (amount < 0) return false;
        if (!CanSpendMoney(team, amount)) return false;

        switch (team)
        {
            case TeamType.Attack:
                attackMoney -= amount;
                break;
            case TeamType.Defense:
                defenseMoney -= amount;
                break;
        }

        OnResourceChanged?.Invoke();
        return true;
    }

    public void AddMoney(TeamType team, int amount)
    {
        if (amount < 0) return;

        switch (team)
        {
            case TeamType.Attack:
                attackMoney += amount;
                break;
            case TeamType.Defense:
                defenseMoney += amount;
                break;
        }

        OnResourceChanged?.Invoke();
    }

    public void SetIncome(TeamType team, int value)
    {
        value = Mathf.Max(0, value);

        switch (team)
        {
            case TeamType.Attack:
                attackIncome = value;
                break;
            case TeamType.Defense:
                defenseIncome = value;
                break;
        }

        OnResourceChanged?.Invoke();
    }

    public void AddIncome(TeamType team, int amount)
    {
        switch (team)
        {
            case TeamType.Attack:
                attackIncome = Mathf.Max(0, attackIncome + amount);
                break;
            case TeamType.Defense:
                defenseIncome = Mathf.Max(0, defenseIncome + amount);
                break;
        }

        OnResourceChanged?.Invoke();
    }

    public void NotifyWhiteCoreDestroyed()
    {
        if (currentState != GameState.Playing) return;
        EndGame(WinType.AttackWin);
    }

    public void Surrender(TeamType surrenderTeam)
    {
        if (currentState != GameState.Playing) return;

        if (surrenderTeam == TeamType.Attack)
        {
            EndGame(WinType.DefenseWin);
        }
        else
        {
            EndGame(WinType.AttackWin);
        }
    }

    private void EndGame(WinType result)
    {
        if (currentState == GameState.GameEnd) return;

        winType = result;
        ChangeState(GameState.GameEnd);
        OnGameEnded?.Invoke(winType);
    }

    private void ChangeState(GameState newState)
    {
        currentState = newState;
        OnStateChanged?.Invoke(currentState);
    }
}