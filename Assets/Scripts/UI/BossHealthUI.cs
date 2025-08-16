using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    public Health boss;
    public Slider slider;

    void Start()
    {
        if (boss && slider)
        {
            slider.minValue = 0;
            slider.maxValue = boss.maxHP;
            slider.value = boss.currentHP;
            boss.onDamaged.AddListener(UpdateBar);
            boss.onHealed.AddListener(UpdateBar);
        }
    }

    void UpdateBar()
    {
        if (boss && slider) slider.value = boss.currentHP;
    }
}

