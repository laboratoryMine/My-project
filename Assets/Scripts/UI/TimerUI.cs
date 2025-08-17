using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    public CPUCountdown countdown;
    public TextMeshProUGUI label;

    void Update()
    {
        if (!countdown || !label) return;
        label.text = Mathf.CeilToInt(countdown.Remaining).ToString();
    }
}
