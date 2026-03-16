using System;
using UnityEngine;

[Serializable]
public class NTickTimer
{
    [SerializeField] private float durationTicks;
    [SerializeField] private float remainingTicks;
    [SerializeField] private bool isRunning;

    public float DurationTicks => durationTicks;
    public float RemainingTicks => remainingTicks;
    public bool IsRunning => isRunning;
    public bool IsReady => !isRunning && remainingTicks <= 0f;
//円の描画のため
    public float NormalizedRemaining
    {
        get
        {
            if (durationTicks <= 0f) return 0f;
            return remainingTicks / durationTicks;
        }
    }

    public event Action OnCompleted;
    public event Action<float, float> OnTickChanged;

    public void StartTimer(float ticks)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTickEvent-= Tick;
        }
        durationTicks = Mathf.Max(0f, ticks);
        remainingTicks = durationTicks;
        isRunning = durationTicks > 0f;

        OnTickChanged?.Invoke(remainingTicks, durationTicks);

        if (durationTicks <= 0f)
        {
            Complete();
            return;
        }
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnTickEvent += Tick;
        }
    }

    public void Tick()
    {
        if (!isRunning) return;

        remainingTicks -= 1f;
        if (remainingTicks < 0f) remainingTicks = 0f;

        OnTickChanged?.Invoke(remainingTicks, durationTicks);

        if (remainingTicks <= 0f)
        {
            Complete();
        }
    }

    private void Complete()
    {
        isRunning = false;
        remainingTicks = 0f;
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnTickEvent -= Tick;
        }
        OnCompleted?.Invoke();
    }
    
}