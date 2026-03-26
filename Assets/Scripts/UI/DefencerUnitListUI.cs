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
    private List<DefencerUnitData> defencerUnits;

    private VisualElement _container;
    private CompositeDisposable _disposables = new();
    private int currentIndex = 0;
    private List<VisualElement> cardElements = new List<VisualElement>();

    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        // アタッカー側の仕様に合わせ、コンテナ名を直接指定
        _container = root.Q<VisualElement>("DefenseUnitList");

        FetchUnitData();
        GenerateCards();
    
        if (cardElements.Count > 0) SelectCardByIndex(0);
    }

    private void FetchUnitData()
    {
        if (defencerSpawner != null)
        {
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
        _disposables = new();
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }

    private void GenerateCards()
    {
        if (_container == null || unitCardTemplate == null) return;
        
        _disposables.Dispose();
        _disposables = new();
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

            // 資金不足時のオーバーレイ（幕）要素を取得
            var unplaceableOverlay = newCard.Q<VisualElement>("UnplacebleColor");

            var progressBar = newCard.Q<ProgressBar>("IntervalProgressBar");
            if (progressBar != null) 
            {
                // 現在のDefencerUnitSpawnerにはクールダウン機能がないため、常に満タンとして処理
                progressBar.value = 100f; 
                // 値が100(High Value)なので非表示にする
                progressBar.style.display = DisplayStyle.None;
            }

            // --- 資金不足判定 ---
            if (GameManager.Instance != null && unplaceableOverlay != null)
            {
                Observable.FromEvent(
                    h => GameManager.Instance.OnResourceChanged += h,
                    h => GameManager.Instance.OnResourceChanged -= h
                )
                .Prepend(Unit.Default)
                .Subscribe(_ =>
                {
                    bool canAfford = GameManager.Instance.CanSpendMoney(team, unitData.SummonCost);
                    
                    if (canAfford) 
                    {
                        unplaceableOverlay.RemoveFromClassList("unit-card-unplaceable");
                    }
                    else 
                    {
                        unplaceableOverlay.AddToClassList("unit-card-unplaceable");
                    }
                })
                .AddTo(_disposables);
            }

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