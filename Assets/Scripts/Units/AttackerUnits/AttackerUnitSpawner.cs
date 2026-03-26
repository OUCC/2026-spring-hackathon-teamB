using System.Collections.Generic;

using R3;

using UnityEngine;

public class AttackerUnitSpawner : MonoBehaviour
{
    [SerializeField]
    /// <summary>
    /// 作成可能な攻撃ユニットのデータのリスト
    /// </summary>
    /// <remarks>
    /// 実行時に変更してはならない。
    /// </remarks>
    private List<AttackerUnitData> _attackerUnitData;

    public IReadOnlyList<AttackerUnitData> AttackerUnitData => _attackerUnitData;

    private Dictionary<string, AttackerUnitData> _attackerUnitDataDict = new();

    /// <summary>
    /// 各ユニット種別の残クールダウン（tick数）
    /// </summary>
    private Dictionary<string, int> _coolDownRemaining = new();

    public IReadOnlyDictionary<string, int> CoolDownRemaining => _coolDownRemaining;

    /// <summary>
    /// 各ユニット種別のクールダウン割合 (0.0 ~ 1.0)
    /// </summary>
    private Dictionary<string, ReactiveProperty<float>> _coolDownRate = new();

    /// <summary>
    /// 各ユニット種別のクールダウン割合を購読用に公開
    /// </summary>>
    public ReadOnlyReactiveProperty<float> GetCoolDownRate(string unitName)
        => _coolDownRate[unitName].ToReadOnlyReactiveProperty();

    public ReadOnlyReactiveProperty<float> GetCoolDownRate(AttackerUnitData data)
        => GetCoolDownRate(data.UnitName);

    /// <summary>
    /// 各ユニット種別の現在の盤面上の数
    /// </summary>
    private Dictionary<string, int> _activeUnitCount = new();


    private void Awake()
    {
        foreach (var data in _attackerUnitData)
        {
            _attackerUnitDataDict.Add(data.UnitName, data);
            _coolDownRemaining[data.UnitName] = 0;
            _activeUnitCount[data.UnitName] = 0;
            _coolDownRate[data.UnitName] = new ReactiveProperty<float>(0f);
        }
    }

    private void Start()
    {
        GameManager.Instance.OnTickEvent += OnTick;
    }

    private void OnDestroy()
    {
        // GameManagerより先に破棄された場合に備えてnullチェック
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTickEvent -= OnTick;
        }

        // ReactivePropertyを破棄
        foreach (var rp in _coolDownRate.Values)
            rp.Dispose();
    }

    /// <summary>
    /// 毎tickクールダウンをカウントダウンする
    /// </summary>
    private void OnTick()
    {
        foreach (var data in _attackerUnitData)
        {
            if (_coolDownRemaining[data.UnitName] > 0)
                _coolDownRemaining[data.UnitName]--;

            _coolDownRate[data.UnitName].Value = data.SummonCoolTime == 0
                ? 0f
                : (float)_coolDownRemaining[data.UnitName] / data.SummonCoolTime;
        }
    }

    /// <summary>
    /// 召喚可否の判定結果
    /// </summary>
    public enum SpawnCheckResult
    {
        /// <summary>召喚可能</summary>
        Ok,
        /// <summary>クールダウン中</summary>
        CoolDown,
        /// <summary>召喚上限に達している</summary>
        LimitReached,
        /// <summary>資金不足</summary>
        NotEnoughMoney,
        /// <summary>Prefabが未設定</summary>
        PrefabNotSet,
    }

    /// <summary>
    /// 召喚可否を理由付きで返す
    /// </summary>
    public SpawnCheckResult CheckSpawn(AttackerUnitData data)
    {
        if (data.Prefab == null) return SpawnCheckResult.PrefabNotSet;
        if (_coolDownRemaining[data.UnitName] > 0) return SpawnCheckResult.CoolDown;
        if (_activeUnitCount[data.UnitName] >= data.SummonLimit) return SpawnCheckResult.LimitReached;
        if (GameManager.Instance.AttackMoney < data.SummonCost) return SpawnCheckResult.NotEnoughMoney;
        return SpawnCheckResult.Ok;
    }


    /// <summary>
    /// 召喚可能かどうかを返す
    /// </summary>
    public bool CanSpawn(AttackerUnitData data)
    {
        return CheckSpawn(data) == SpawnCheckResult.Ok;
    }

    public BasicAttackerUnit Spawn(Vector3 position, string name)
    {
        if (_attackerUnitDataDict.TryGetValue(name, out var data))
        {
            return Spawn(position, data);
        }
        else
        {
            Debug.LogError($"no such {nameof(AttackerUnitData)}: name == {name}");
            return null;
        }
    }

    public BasicAttackerUnit Spawn(Vector3 position, AttackerUnitData attackerUnitData)
    {
        switch (CheckSpawn(attackerUnitData))
        {
            case SpawnCheckResult.PrefabNotSet:
                Debug.LogError($"Prefab is not set in AttackerUnitData: {attackerUnitData.UnitName}");
                return null;

            case SpawnCheckResult.CoolDown:
                Debug.LogWarning($"{attackerUnitData.UnitName} はクールダウン中です。" +
                                 $"残り: {_coolDownRemaining[attackerUnitData.UnitName]} tick");
                return null;

            case SpawnCheckResult.LimitReached:
                Debug.LogWarning($"{attackerUnitData.UnitName} は召喚上限({attackerUnitData.SummonLimit})に達しています。");
                return null;

            case SpawnCheckResult.NotEnoughMoney:
                Debug.LogWarning($"{attackerUnitData.UnitName}  の召喚に必要な資金({attackerUnitData.SummonCost})が不足しています。");
                return null;
        }

        // ── 資金消費 ───────────────────────────────────────
        // CheckSpawn で残金チェック済みだが、SpendMoney は排他制御も兼ねる
        if (!GameManager.Instance.SpendMoney(GameManager.TeamType.Attack, attackerUnitData.SummonCost))
        {
            Debug.LogWarning($"{attackerUnitData.UnitName}  の資金消費に失敗しました。");
            Debug.LogWarning($"残金: {GameManager.Instance.AttackMoney}, 必要金額: {attackerUnitData.SummonCost}");
            return null;
        }


        var unitInstance = Instantiate(attackerUnitData.Prefab, position, Quaternion.identity);
        unitInstance.transform.localScale = new Vector3(attackerUnitData.Scale, attackerUnitData.Scale, attackerUnitData.Scale);

        var attackerUnitInstance = unitInstance.GetComponentInChildren<BasicAttackerUnit>();

        if (attackerUnitInstance == null)
        {
            Debug.LogError($"Prefab {attackerUnitData.Prefab.name} does not have a BasicAttackerUnit component in its children.");
            return null;
        }


        _coolDownRemaining[attackerUnitData.UnitName] = attackerUnitData.SummonCoolTime;
        _activeUnitCount[attackerUnitData.UnitName]++;

        attackerUnitInstance.OnDied += _ =>
        {
            _activeUnitCount[attackerUnitData.UnitName]--;
        };

        if (attackerUnitData.MoveStrategy == null)
        {
            Debug.LogWarning($"MoveStrategy is not set in AttackerUnitData: {attackerUnitData.UnitName}");
        }

        IMoveStrategy moveStrategy = attackerUnitData.MoveStrategy != null
            ? attackerUnitData.MoveStrategy.CreateInstance()
            : null;

        if (attackerUnitData.AttackStrategy == null)
        {
            Debug.LogWarning($"AttackStrategy is not set in AttackerUnitData: {attackerUnitData.UnitName}");
        }

        IAttackStrategy attackStrategy = attackerUnitData.AttackStrategy != null
           ? attackerUnitData.AttackStrategy.CreateInstance()
           : null;

        attackerUnitInstance.Initialize(attackerUnitData, moveStrategy, attackStrategy);

        return attackerUnitInstance;
    }
}
