using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using R3;

public class UnitListUI : MonoBehaviour
{
    [Header("Team Setting")]
    [SerializeField] private GameManager.TeamType team; // Attack か Defense かを選択

    [Header("UI Resources")]
    [SerializeField] private VisualTreeAsset unitCardTemplate;

    [Header("References")]
    [SerializeField] private AttackerUnitSpawner unitSpawner;

    private VisualElement unitListContainer;
    private List<VisualElement> cardElements = new List<VisualElement>();
    private List<AttackerUnitData> unitDataList = new List<AttackerUnitData>();
    private int currentIndex = 0;
    
    private CompositeDisposable _disposables = new();

    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // UXML内のコンテナ名 (AttackUnitList / DefenseUnitList など) を特定して取得
        string containerName = (team == GameManager.TeamType.Attack) ? "AttackUnitList" : "DefenseUnitList";
        unitListContainer = root.Q<VisualElement>(containerName);

        FetchUnitData();
        GenerateCards();

        if (cardElements.Count > 0)
        {
            SelectCardByIndex(0);
        }
    }

    private void FetchUnitData()
    {
        if (unitSpawner == null)
        {
            unitSpawner = UnityEngine.Object.FindFirstObjectByType<AttackerUnitSpawner>();
        }

        if (unitSpawner != null)
        {
            unitDataList = new List<AttackerUnitData>(unitSpawner.AttackerUnitData);
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
        if (unitCardTemplate == null || unitListContainer == null) return;

        _disposables.Dispose();
        _disposables = new();
        unitListContainer.Clear();
        cardElements.Clear();

        for (int i = 0; i < unitDataList.Count; i++)
        {
            var unitData = unitDataList[i];
            var newCard = unitCardTemplate.Instantiate();
            int index = i;

            // --- A. ラベルセット処理 ---
            var nameLabel = newCard.Q<Label>("UnitNameLabel");
            if (nameLabel != null) nameLabel.text = unitData.UnitName;

            var costLabel = newCard.Q<Label>("SummonCostLabel");
            if (costLabel != null) costLabel.text = unitData.SummonCost.ToString();

            // 資金不足時のオーバーレイ（幕）要素を取得
            var unplaceableOverlay = newCard.Q<VisualElement>("UnplacebleColor");

            // --- B. R3によるクールダウンのバインド ---
            var progressBar = newCard.Q<ProgressBar>("IntervalProgressBar");

            if (unitSpawner != null && progressBar != null)
            {
                unitSpawner.GetCoolDownRate(unitData)
                    .Subscribe(rate =>
                    {
                        float currentVal = (1.0f - rate) * 100f;
                        progressBar.value = currentVal;

                        // valueが100 (high-value) に達した場合は非表示にする
                        progressBar.style.display = currentVal >= 100f ? DisplayStyle.None : DisplayStyle.Flex;

                        // クールダウン中はカードを半透明にし、操作を無効化
                        newCard.style.opacity = (rate > 0f) ? 0.5f : 1.0f;
                        newCard.SetEnabled(rate <= 0f);
                    })
                    .AddTo(_disposables);
            }

            // --- B-2. 資金不足による見た目の切り替え ---
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
                        // お金が足りないとき、UnplacebleColor にクラスを適用して暗くする
                        unplaceableOverlay.AddToClassList("unit-card-unplaceable");
                    }
                })
                .AddTo(_disposables);
            }

            // --- C. イベントとリストへの追加 ---
            newCard.RegisterCallback<ClickEvent>(evt => SelectCardByIndex(index));

            unitListContainer.Add(newCard);
            cardElements.Add(newCard);
        }
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

    public void SelectCardByIndex(int index)
    {
        if (index < 0 || index >= cardElements.Count) return;

        if (currentIndex < cardElements.Count)
        {
            cardElements[currentIndex].RemoveFromClassList("unit-card-selected");
        }

        currentIndex = index;
        cardElements[currentIndex].AddToClassList("unit-card-selected");

        Debug.Log($"{team} チーム: {unitDataList[currentIndex].UnitName} を選択中");
    }

    public AttackerUnitData GetSelectedUnitData()
    {
        if (currentIndex < unitDataList.Count)
        {
            return unitDataList[currentIndex];
        }
        return null;
    }
}