using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int maxHP = 300;
    public int currentHP;
    public UnityEvent onDamaged;
    public UnityEvent onHealed;
    public UnityEvent onDied;

    void Awake() => currentHP = maxHP;

    public void TakeDamage(int amount)
    {
        if (currentHP <= 0) return;
        currentHP -= Mathf.Max(1, amount);
        onDamaged?.Invoke();
        if (currentHP <= 0)
        {
            currentHP = 0;
            onDied?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        int before = currentHP;
        currentHP = Mathf.Min(maxHP, currentHP + Mathf.Max(1, amount));
        if (currentHP > before) onHealed?.Invoke();
    }

    public float Normalized => maxHP > 0 ? (float)currentHP / maxHP : 0f;
}
