using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class UnitListUI : MonoBehaviour
{
    [Header("Team Setting")]
    [SerializeField] private GameManager.TeamType team; // Attack か Defense かを選択

    [Header("UI Resources")]
    [SerializeField] private VisualTreeAsset unitCardTemplate; // カードの見た目 (UXML)
    
    [Header("References")]
    [SerializeField] private AttackerUnitSpawner unitSpawner; // ユニット生成器（データ取得用）

    private VisualElement unitListContainer;
    private List<VisualElement> cardElements = new List<VisualElement>();
    private List<AttackerUnitData> unitDataList = new List<AttackerUnitData>();
    private int currentIndex = 0;

    private void OnEnable()
    {
        // UIのルートを取得
        var root = GetComponent<UIDocument>().rootVisualElement;

        // UXML内のコンテナ名 (AttackUnitList / DefenseUnitList) を特定して取得
        string containerName = (team == GameManager.TeamType.Attack) ? "AttackUnitList" : "DefenseUnitList";
        unitListContainer = root.Q<VisualElement>(containerName);

        // スパナーからユニットデータのリストを同期
        FetchUnitData();
        
        // カードを生成して並べる
        GenerateCards();
        
        // 初期選択状態（0番目）にする
        if (cardElements.Count > 0)
        {
            SelectCardByIndex(0);
        }
    }

    /// <summary>
    /// AttackerUnitSpawner のプロパティから最新のユニットリストを取得します
    /// </summary>
    private void FetchUnitData()
    {
        if (unitSpawner == null)
        {
            unitSpawner = Object.FindFirstObjectByType<AttackerUnitSpawner>();
        }

        if (unitSpawner != null)
        {
            // スパナーが持つ IReadOnlyList から新しいリストを作成
            unitDataList = new List<AttackerUnitData>(unitSpawner.AttackerUnitData);
        }
    }

    /// <summary>
    /// 取得したデータに基づいてカードを生成し、UIコンテナに追加します
    /// </summary>
    private void GenerateCards()
    {
        if (unitCardTemplate == null || unitListContainer == null) return;

        unitListContainer.Clear();
        cardElements.Clear();

        for (int i = 0; i < unitDataList.Count; i++)
        {
            var unitData = unitDataList[i];
            var newCard = unitCardTemplate.Instantiate();
            int index = i;

            // ユニット名とコストを表示に反映
            var nameLabel = newCard.Q<Label>("NameLabel");
            if (nameLabel != null) nameLabel.text = unitData.UnitName;

            var costLabel = newCard.Q<Label>("CostLabel");
            if (costLabel != null) costLabel.text = unitData.SummonCost.ToString();

            // マウスでのクリック選択も一応残しておく
            newCard.RegisterCallback<ClickEvent>(evt => SelectCardByIndex(index));

            unitListContainer.Add(newCard);
            cardElements.Add(newCard);
        }
    }

    /// <summary>
    /// 次のカード（右方向）を選択します。コントローラーの R ボタン等から呼び出します。
    /// </summary>
    public void SelectNext()
    {
        if (cardElements.Count == 0) return;
        // リストの端まで行ったら最初（0）に戻る
        int nextIndex = (currentIndex + 1) % cardElements.Count;
        SelectCardByIndex(nextIndex);
    }

    /// <summary>
    /// 前のカード（左方向）を選択します。コントローラーの L ボタン等から呼び出します。
    /// </summary>
    public void SelectPrevious()
    {
        if (cardElements.Count == 0) return;
        // 負の数にならないよう、リストの数を足してから余りを計算
        int prevIndex = (currentIndex - 1 + cardElements.Count) % cardElements.Count;
        SelectCardByIndex(prevIndex);
    }

    /// <summary>
    /// 指定したインデックスのカードを選択状態（強調表示）にします
    /// </summary>
    public void SelectCardByIndex(int index)
    {
        if (index < 0 || index >= cardElements.Count) return;

        // 前の選択の強調クラスを削除
        if (currentIndex < cardElements.Count)
        {
            cardElements[currentIndex].RemoveFromClassList("unit-card-selected");
        }

        currentIndex = index;

        // 新しい選択に強調クラスを追加
        cardElements[currentIndex].AddToClassList("unit-card-selected");

        Debug.Log($"{team} チーム: {unitDataList[currentIndex].UnitName} を選択中");
    }

    /// <summary>
    /// 現在選択されているユニットのデータを取得します
    /// </summary>
    public AttackerUnitData GetSelectedUnitData()
    {
        if (currentIndex < unitDataList.Count)
        {
            return unitDataList[currentIndex];
        }
        return null;
    }
}