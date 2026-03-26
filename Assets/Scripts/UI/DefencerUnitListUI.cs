using System;
using System.Collections.Generic;
using System.Linq; // ← これを追加！(LINQを使うため)
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

    // [変更点1] DefencerUnitData のリストから、共通の箱のリストに変更
    private List<DefenderUIObject> defenderUIObjects = new();

    private VisualElement _container;
    private CompositeDisposable _disposables = new();
    private int currentIndex = 0;
    private List<VisualElement> cardElements = new List<VisualElement>();

    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        _container = root.Q<VisualElement>("DefenseUnitList");

        FetchData(); // メソッド名を少し変更しました
        GenerateCards();
    
        if (cardElements.Count > 0) SelectCardByIndex(0);
    }

    // [変更点2] ユニットとタイルの両方をリストに詰める
    private void FetchData()
    {
        defenderUIObjects.Clear();

        // 1. 防衛ユニットのデータを箱（DefenderUIObject）に変換して追加
        if (defencerSpawner != null)
        {
            var unitObjects = defencerSpawner.DefencerUnitData.Select(d => new DefenderUIObject
            {
                name = d.UnitName,
                summonCost = d.SummonCost,
                type = ItemType.Unit,
                unitData = d // 後で配置できるように元のデータを保持
            });
            defenderUIObjects.AddRange(unitObjects);
        }
        else
        {
            Debug.LogError("DefencerSpawner がインスペクターで設定されていません！");
        }

        // 2. 方向タイルのデータを手動で追加（例として右と上を追加）
        // ※コストは仮で50にしています。ゲームバランスに合わせて調整してください。
        defenderUIObjects.Add(new DefenderUIObject
        {
            name = "Right Tile",
            summonCost = 50,
            type = ItemType.Tile,
            tileDirection = DirectionTileSpawner.DirectionType.Right
        });

        defenderUIObjects.Add(new DefenderUIObject
        {
            name = "Up Tile",
            summonCost = 50,
            type = ItemType.Tile,
            tileDirection = DirectionTileSpawner.DirectionType.Up
        });
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

    // [変更点3] 箱（DefenderUIObject）のデータを使ってUIを生成する
    private void GenerateCards()
    {
        if (_container == null || unitCardTemplate == null) return;
        
        _disposables.Dispose();
        _disposables = new();
        _container.Clear();
        cardElements.Clear();

        for (int i = 0; i < defenderUIObjects.Count; i++)
        {
            var itemData = defenderUIObjects[i]; // ここで箱を取り出す
            var newCard = unitCardTemplate.Instantiate();
            int index = i;

            var nameLabel = newCard.Q<Label>("UnitNameLabel");
            if (nameLabel != null) nameLabel.text = itemData.name; // 箱の名前

            var costLabel = newCard.Q<Label>("SummonCostLabel");
            if (costLabel != null) costLabel.text = itemData.summonCost.ToString(); // 箱のコスト

            var unplaceableOverlay = newCard.Q<VisualElement>("UnplacebleColor");
            var progressBar = newCard.Q<ProgressBar>("IntervalProgressBar");
            
            if (progressBar != null) 
            {
                progressBar.value = 100f; 
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
                    // 箱のコストを使って判定
                    bool canAfford = GameManager.Instance.CanSpendMoney(team, itemData.summonCost);
                    
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

    // [変更点4] 選択された「箱（DefenderUIObject）」自体を返すように変更
    public DefenderUIObject GetSelectedUIObject()
    {
        if (defenderUIObjects != null && currentIndex >= 0 && currentIndex < defenderUIObjects.Count)
        {
            return defenderUIObjects[currentIndex];
        }
        return null;
    }
}