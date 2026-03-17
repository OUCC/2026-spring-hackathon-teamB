using UnityEngine;

public class TimerTest : MonoBehaviour
{
    // ▼TimerUIスクリプトをセット    
    [SerializeField] private TimerUI targetTimerUI;

    // ▼ テスト用の制限時間（180秒）
    private float remainingTime = 180f;

    private void Update()
    {
        if (remainingTime <= 0f) return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            Debug.Log("テスト：タイムアップしました！");
        }

        if (targetTimerUI != null)
        {
            targetTimerUI.UpdateTimer(remainingTime);
        }
    }
}
