using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UnitListUI : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset unitCardTemplate; // 設計図 (UXML)
    [SerializeField] private List<UnitData> unitDataList;      // データのリスト (SO)

    private VisualElement unitList; // カードを並べる親要素の"UnitList"VisualElemntを入れる
    private VisualElement selectedCard;

    private void OnEnable()
    {
        // UIの根本(Root)を取得
        var root = GetComponent<UIDocument>().rootVisualElement;

        // rootを持つ親要素取得
        unitList = root.Q<VisualElement>("UnitList");

        GenerateCards();
    }

    private void GenerateCards()
    {
        if (unitCardTemplate == null || unitList == null) return;

        // 重複しないように一度中身を空にする
        unitList.Clear();

        foreach (var unitData in unitDataList)
        {
            // 設計図から実体(TemplateContainer)を生成
            var newCard = unitCardTemplate.Instantiate();

            // UXML内のパーツを探して、SOのデータを流し込む
            // "CostLabel"Labelにコストを入れる
            newCard.Q<Label>("CostLabel").text = unitData.cost.ToString();
            // "UnitImage"VisualElementのバックグラウンド画像としてユニット画像を入れる
            if (unitData.unitImage != null)
            {
                newCard.Q<VisualElement>("UnitImage").style.backgroundImage =
                    new StyleBackground(unitData.unitImage);
            }
            // クリックされたときの処理
            newCard.RegisterCallback<ClickEvent>(evt => SelectCard(newCard));
            // 親要素に追加して画面に表示
            unitList.Add(newCard);
        }
    }

    private void SelectCard(VisualElement targetCard)
    {
        if (selectedCard != null)
        {
            selectedCard.RemoveFromClassList("unit-card-selected");
        }
        selectedCard = targetCard;
        selectedCard.AddToClassList("unit-card-selected");
        // 選択されたカードを前面に表示するが、これだと最後尾に移動してしまう
        //selectedCard.BringToFront(); 
    }
}
