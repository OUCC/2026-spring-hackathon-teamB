using UnityEngine;
using UnityEngine.UIElements;

public class TimerUI : MonoBehaviour
{
    private Label timeLabel;

    private void OnEnable()
    {
        // 1. UIの取得
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        timeLabel = root.Q<Label>("TimeLabel");

        // 2. GameManager のイベントを購読（登録）する
        // GameManager が存在することを確認してから登録
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTimeChanged += RefreshDisplay;
            // 登録した瞬間に一度表示を更新しておく
            RefreshDisplay();
        }
    }

    private void OnDisable()
    {
        // 重要：オブジェクトが消えるときはイベントの登録を解除する（メモリリーク防止）
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTimeChanged -= RefreshDisplay;
        }
    }

    /// <summary>
    /// GameManagerから残り時間を取得して表示を更新します。
    /// </summary>
    private void RefreshDisplay()
    {
        if (timeLabel == null || GameManager.Instance == null) return;

        // GameManager のプロパティから直接「残り時間」をもらう
        UpdateTimer(GameManager.Instance.RemainingTime);
    }

    public void UpdateTimer(float remainingSeconds)
    {
        if (timeLabel == null) return;

        // 0以下にならないように保護
        float timeToDisplay = Mathf.Max(0, remainingSeconds);

        int minutes = Mathf.FloorToInt(timeToDisplay / 60F);
        int seconds = Mathf.FloorToInt(timeToDisplay - minutes * 60);

        timeLabel.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}