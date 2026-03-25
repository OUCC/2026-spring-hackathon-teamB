using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using R3;

public class DefencerUnitListUI : MonoBehaviour
{
    [Header("Team Setting")]
    [SerializeField] private GameManager.TeamType team = GameManager.TeamType.Defense;

    [Header("References")]
    [SerializeField] private VisualTreeAsset unitCardTemplate;
    [SerializeField] private DefencerUnitSpawner defencerSpawner; 

    [Header("Data")]
    private List<DefencerUnitData> defencerUnits; // 修正済み

    private VisualElement _container;
    private readonly CompositeDisposable _disposables = new();
    private int currentIndex = 0;
    private List<VisualElement> cardElements = new List<VisualElement>();

    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _container = root.Q<VisualElement>("DefenseUnitList");

        // 1. スポナーからデータを同期する
        FetchUnitData();

         // 2. そのデータに基づいてカードを作る
        GenerateCards();
    
        if (cardElements.Count > 0) SelectCardByIndex(0);
    }
    private void FetchUnitData()
    {
        if (defencerSpawner != null)
        {
            // スポナーが持っている IReadOnlyList からリストを作成
            defencerUnits = new List<DefencerUnitData>(defencerSpawner.DefencerUnitData);
        }
        else
        {
            Debug.LogError("DefencerSpawner がインスペクターで設定されていません！");
        }
    }

    private void OnDisable()
    {
        _disposables.Dispose();
    }

    private void GenerateCards()
    {
        if (_container == null) return;
        _container.Clear();
        cardElements.Clear();

        for (int i = 0; i < defencerUnits.Count; i++)
        {
            var unitData = defencerUnits[i];
            var newCard = unitCardTemplate.Instantiate();
            int index = i;

            var nameLabel = newCard.Q<Label>("UnitNameLabel");
            if (nameLabel != null) nameLabel.text = unitData.UnitName;

            var costLabel = newCard.Q<Label>("SummonCostLabel");
            if (costLabel != null) costLabel.text = unitData.SummonCost.ToString();

            // ※ 現在のDefencerUnitSpawnerにはクールダウン機能がないため、プログレスバーは常に満タンか非表示にします
            var progressBar = newCard.Q<ProgressBar>("IntervalProgressBar");
            if (progressBar != null) progressBar.value = 100f; 

            // --- 資金不足判定のみ行う（GameManagerを使用） ---
            if (GameManager.Instance != null)
            {
                Observable.FromEvent(
                    h => GameManager.Instance.OnResourceChanged += h,
                    h => GameManager.Instance.OnResourceChanged -= h
                )
                .Prepend(Unit.Default)
                .Subscribe(_ =>
                {
                    bool canAfford = GameManager.Instance.CanSpendMoney(team, unitData.SummonCost);
                    if (canAfford) newCard.RemoveFromClassList("unit-card-unplaceable");
                    else newCard.AddToClassList("unit-card-unplaceable");
                })
                .AddTo(_disposables);
            }

            // クリックで選択
            newCard.RegisterCallback<ClickEvent>(evt => SelectCardByIndex(index));

            _container.Add(newCard);
            cardElements.Add(newCard);
        }
    }

    public void SelectCardByIndex(int index)
    {
        if (index < 0 || index >= cardElements.Count) return;

        if (currentIndex < cardElements.Count)
        {
            cardElements[currentIndex].RemoveFromClassList("unit-card-selected");
        }

        currentIndex = index;
        cardElements[currentIndex].AddToClassList("unit-card-selected");
    }


    public void SelectNext()
    {
        if (cardElements.Count == 0) return;
        int nextIndex = (currentIndex + 1) % cardElements.Count;
        SelectCardByIndex(nextIndex);
    }

    public void SelectPrevious()
    {
        if (cardElements.Count == 0) return;
        int prevIndex = (currentIndex - 1 + cardElements.Count) % cardElements.Count;
        SelectCardByIndex(prevIndex);
    }

    public DefencerUnitData GetSelectedUnitData()
    {
        if (defencerUnits != null && currentIndex >= 0 && currentIndex < defencerUnits.Count)
        {
            return defencerUnits[currentIndex];
        }
        return null;
    }
}