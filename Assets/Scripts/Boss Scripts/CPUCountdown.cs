using UnityEngine;
using UnityEngine.Events;

public class CPUCountdown : MonoBehaviour
{
    [Min(1f)] public float duration = 90f;
    public UnityEvent onTimerStarted;
    public UnityEvent onTimeExpired;
    public UnityEvent<float> onTick;

    float remaining;
    bool running;

    public void StartTimer()
    {
        remaining = duration;
        running = true;
        onTimerStarted?.Invoke();
    }

    public void StopTimer() => running = false;

    void Update()
    {
        if (!running) return;
        remaining -= Time.deltaTime;
        onTick?.Invoke(Mathf.Max(remaining, 0f));
        if (remaining <= 0f)
        {
            running = false;
            onTimeExpired?.Invoke();
        }
    }

    public float Remaining => Mathf.Max(remaining, 0f);
}
