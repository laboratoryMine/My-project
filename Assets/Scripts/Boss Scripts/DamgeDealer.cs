using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public int damage = 10;
    public float life = 5f;
    public string targetTag = "Player";

    void Start() => Destroy(gameObject, life);

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;
        var hp = other.GetComponentInParent<Health>();
        if (hp) hp.TakeDamage(damage);
        Destroy(gameObject);
    }
}
