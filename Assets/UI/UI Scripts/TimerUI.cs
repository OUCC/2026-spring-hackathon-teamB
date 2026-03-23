using UnityEngine;
using UnityEngine.UIElements;

public class TimerUI : MonoBehaviour
{
    private Label timeLabel;

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        timeLabel = root.Q<Label>("TimeLabel");
    }


    /// <summary>
    /// 残り時間を秒数で受け取り、UIの表示を更新します。
    /// remainingSeconds に 150 を渡すと "02:30" になります。
    /// 時間を管理するスクリプトから呼び出す
    /// </summary>
    public void UpdateTimer(float remainingSeconds)
    {
        if (timeLabel == null) return;

        // 計算して文字列にする
        int minutes = Mathf.FloorToInt(remainingSeconds / 60F);
        int seconds = Mathf.FloorToInt(remainingSeconds - minutes * 60);

        timeLabel.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}