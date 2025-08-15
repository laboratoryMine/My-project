using System.Collections;
using UnityEngine;

public class BossAIWithAnimator : MonoBehaviour
{
    [Header("Links")]
    public Animator anim;
    public Transform player;
    public Transform shootOrigin;
    public GameObject bulletPrefab;

    [Header("Tuning")]
    public float attackCooldown = 1.4f;
    public float bulletSpeed = 14f;
    public int burstShots = 6;
    public float burstInterval = 0.18f;
    public float dashSpeed = 18f;
    public float dashStopDistance = 1.8f;
    public int contactDamage = 15;

    Coroutine loop;
    int fireIndex = -1;

    // Animator trigger hashes (rename strings if your controller uses different names)
    int idleID = Animator.StringToHash("idle");
    int walkID = Animator.StringToHash("walk");
    int flyID = Animator.StringToHash("fly");
    int landID = Animator.StringToHash("land");
    int forwardStanceID = Animator.StringToHash("forward_stance");
    int stopFiringID = Animator.StringToHash("stop_firing");
    int fire1ID = Animator.StringToHash("fire1");
    int fire2ID = Animator.StringToHash("fire2");
    int fire3ID = Animator.StringToHash("fire3");
    int dieForwardID = Animator.StringToHash("die_forward_stance");

    void Reset() { anim = GetComponent<Animator>(); }

    public void Begin()
    {
        if (loop != null) StopCoroutine(loop);
        if (anim) anim.SetTrigger(forwardStanceID);
        loop = StartCoroutine(AttackLoop());
    }

    public void Halt()
    {
        if (loop != null) StopCoroutine(loop);
        loop = null;
        if (anim) anim.SetTrigger(stopFiringID);
    }

    IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(0.8f);
        while (true)
        {
            yield return ProjectileBurst();
            yield return new WaitForSeconds(attackCooldown);
            yield return DashAtPlayer();
            yield return new WaitForSeconds(attackCooldown);
        }
    }

    IEnumerator ProjectileBurst()
    {
        if (!player || !bulletPrefab || !shootOrigin) yield break;
        yield return new WaitForSeconds(0.25f);

        for (int i = 0; i < burstShots; i++)
        {
            TriggerNextFire();
            var b = Instantiate(bulletPrefab, shootOrigin.position, Quaternion.identity);
            if (b.TryGetComponent<Rigidbody>(out var rb))
            {
                Vector3 dir = (player.position - shootOrigin.position).normalized;
                rb.linearVelocity = dir * bulletSpeed;
            }
            yield return new WaitForSeconds(burstInterval);
        }
        if (anim) anim.SetTrigger(stopFiringID);
    }

    void TriggerNextFire()
    {
        if (!anim) return;
        fireIndex = (fireIndex + 1) % 3;
        if (fireIndex == 0) anim.SetTrigger(fire1ID);
        else if (fireIndex == 1) anim.SetTrigger(fire2ID);
        else anim.SetTrigger(fire3ID);
    }

    IEnumerator DashAtPlayer()
    {
        if (!player) yield break;

        if (anim) anim.SetTrigger(flyID);
        yield return new WaitForSeconds(0.2f);

        Vector3 target = player.position;
        float t = 0f;
        while (Vector3.Distance(transform.position, target) > dashStopDistance && t < 1.5f)
        {
            Vector3 dir = (target - transform.position).normalized;
            transform.position += dir * dashSpeed * Time.deltaTime;
            t += Time.deltaTime;
            yield return null;
        }
        if (anim) anim.SetTrigger(landID);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            var hp = other.collider.GetComponentInParent<Health>();
            if (hp) hp.TakeDamage(contactDamage);
        }
    }

    public void PlayDeath()
    {
        if (anim) anim.SetTrigger(dieForwardID);
        Halt();
    }
}
