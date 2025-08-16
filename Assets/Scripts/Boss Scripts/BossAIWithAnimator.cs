//using System.Collections;
//using UnityEngine;

//public class BossAIWithAnimator : MonoBehaviour
//{
//    [Header("Links")]
//    public Animator anim;
//    public Transform player;
//    public Transform shootOrigin;
//    public GameObject bulletPrefab;

//    [Header("Tuning")]
//    public float attackCooldown = 1.4f;
//    public float bulletSpeed = 14f;
//    public int burstShots = 6;
//    public float burstInterval = 0.18f;
//    public float dashSpeed = 18f;
//    public float dashStopDistance = 1.8f;
//    public int contactDamage = 15;

//    Coroutine loop;
//    int fireIndex = -1;

//    // Animator trigger hashes (rename strings if your controller uses different names)
//    int idleID = Animator.StringToHash("idle");
//    int walkID = Animator.StringToHash("walk");
//    int flyID = Animator.StringToHash("fly");
//    int landID = Animator.StringToHash("land");
//    int forwardStanceID = Animator.StringToHash("forward_stance");
//    int stopFiringID = Animator.StringToHash("stop_firing");
//    int fire1ID = Animator.StringToHash("fire1");
//    int fire2ID = Animator.StringToHash("fire2");
//    int fire3ID = Animator.StringToHash("fire3");
//    int dieForwardID = Animator.StringToHash("die_forward_stance");

//    void Reset() { anim = GetComponent<Animator>(); }

//    public void Begin()
//    {
//        if (loop != null) StopCoroutine(loop);
//        if (anim) anim.SetTrigger(forwardStanceID);
//        loop = StartCoroutine(AttackLoop());
//    }

//    public void Halt()
//    {
//        if (loop != null) StopCoroutine(loop);
//        loop = null;
//        if (anim) anim.SetTrigger(stopFiringID);
//    }

//    IEnumerator AttackLoop()
//    {
//        yield return new WaitForSeconds(0.8f);
//        while (true)
//        {
//            yield return ProjectileBurst();
//            yield return new WaitForSeconds(attackCooldown);
//            yield return DashAtPlayer();
//            yield return new WaitForSeconds(attackCooldown);
//        }
//    }

//    IEnumerator ProjectileBurst()
//    {
//        if (!player || !bulletPrefab || !shootOrigin) yield break;
//        yield return new WaitForSeconds(0.25f);

//        for (int i = 0; i < burstShots; i++)
//        {
//            TriggerNextFire();
//            var b = Instantiate(bulletPrefab, shootOrigin.position, Quaternion.identity);
//            if (b.TryGetComponent<Rigidbody>(out var rb))
//            {
//                Vector3 dir = (player.position - shootOrigin.position).normalized;
//                rb.linearVelocity = dir * bulletSpeed;
//            }
//            yield return new WaitForSeconds(burstInterval);
//        }
//        if (anim) anim.SetTrigger(stopFiringID);
//    }

//    void TriggerNextFire()
//    {
//        if (!anim) return;
//        fireIndex = (fireIndex + 1) % 3;
//        if (fireIndex == 0) anim.SetTrigger(fire1ID);
//        else if (fireIndex == 1) anim.SetTrigger(fire2ID);
//        else anim.SetTrigger(fire3ID);
//    }

//    IEnumerator DashAtPlayer()
//    {
//        if (!player) yield break;

//        if (anim) anim.SetTrigger(flyID);
//        yield return new WaitForSeconds(0.2f);

//        Vector3 target = player.position;
//        float t = 0f;
//        while (Vector3.Distance(transform.position, target) > dashStopDistance && t < 1.5f)
//        {
//            Vector3 dir = (target - transform.position).normalized;
//            transform.position += dir * dashSpeed * Time.deltaTime;
//            t += Time.deltaTime;
//            yield return null;
//        }
//        if (anim) anim.SetTrigger(landID);
//    }

//    void OnCollisionEnter(Collision other)
//    {
//        if (other.collider.CompareTag("Player"))
//        {
//            var hp = other.collider.GetComponentInParent<Health>();
//            if (hp) hp.TakeDamage(contactDamage);
//        }
//    }

//    public void PlayDeath()
//    {
//        if (anim) anim.SetTrigger(dieForwardID);
//        Halt();
//    }
//}


using System.Collections;
using System.Linq;
using UnityEngine;

public class BossAI_WithSockets : MonoBehaviour
{
    [Header("Links")]
    public Animator anim;
    public Transform player;
    [Tooltip("All muzzle sockets on the robot (left/right/eye). +Z points forward")]
    public Transform[] muzzles;
    public GameObject bulletPrefab;

    [Header("Attacks")]
    public float attackCooldown = 1.3f;
    public int burstShots = 6;
    public float burstInterval = 0.16f;
    public float bulletSpeed = 16f;
    public bool alternateMuzzles = true;       // L,R,L,R…
    public bool chooseClosestMuzzle = false;   // or pick the closest to the player

    [Header("Aim")]
    public bool useAimLead = true;             // predict player motion if it has a Rigidbody
    public float verticalAimOffset = 1.1f;     // aim at chest/head height
    public float turnSpeedDeg = 360f;          // boss rotates to face target when firing

    [Header("Dash")]
    public float dashSpeed = 18f;
    public float dashStopDistance = 1.8f;
    public int contactDamage = 15;

    // === Animator trigger names (strings) ===
    const string TRIG_IDLE = "idle";
    const string TRIG_FLY = "fly";
    const string TRIG_LAND = "land";
    const string TRIG_STANCE = "forward_stance";
    const string TRIG_STOP = "stop_firing";
    const string TRIG_FIRE1 = "fire1";
    const string TRIG_FIRE2 = "fire2";
    const string TRIG_FIRE3 = "fire3";
    const string TRIG_DIE = "die_forward_stance";

    Coroutine loop;
    int fireIndex = -1;
    int muzzleIndex = -1;
    Rigidbody playerRB;
    Collider[] bossColliders;

    void Awake()
    {
        if (!anim) anim = GetComponent<Animator>();
        playerRB = player ? player.GetComponent<Rigidbody>() : null;
        bossColliders = GetComponentsInChildren<Collider>(true);
    }

    public void Begin()
    {
        if (loop != null) StopCoroutine(loop);
        anim?.SetTrigger(TRIG_STANCE);
        loop = StartCoroutine(AttackLoop());
    }

    public void Halt()
    {
        if (loop != null) StopCoroutine(loop);
        loop = null;
        anim?.SetTrigger(TRIG_STOP);
    }

    IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(0.6f); // intro beat
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
        if (muzzles == null || muzzles.Length == 0 || !bulletPrefab || !player) yield break;

        // tiny wind-up + face target
        yield return new WaitForSeconds(0.18f);

        for (int i = 0; i < burstShots; i++)
        {
            Transform muzzle = SelectMuzzle();
            Vector3 aimPoint = ComputeAimPoint(muzzle.position);
            RotateTowards(aimPoint);

            // fire anim (cycles 1→2→3)
            fireIndex = (fireIndex + 1) % 3;
            if (fireIndex == 0) anim?.SetTrigger(TRIG_FIRE1);
            else if (fireIndex == 1) anim?.SetTrigger(TRIG_FIRE2);
            else anim?.SetTrigger(TRIG_FIRE3);

            SpawnBullet(muzzle, aimPoint);
            yield return new WaitForSeconds(burstInterval);
        }
        anim?.SetTrigger(TRIG_STOP);
    }

    IEnumerator DashAtPlayer()
    {
        if (!player) yield break;

        anim?.SetTrigger(TRIG_FLY);
        yield return new WaitForSeconds(0.2f);

        Vector3 target = PlayerCenter();
        float t = 0f;
        while (Vector3.Distance(transform.position, target) > dashStopDistance && t < 1.5f)
        {
            Vector3 dir = (target - transform.position).normalized;
            transform.position += dir * dashSpeed * Time.deltaTime;
            t += Time.deltaTime;
            yield return null;
        }
        anim?.SetTrigger(TRIG_LAND);
    }

    void RotateTowards(Vector3 worldPoint)
    {
        Vector3 to = (worldPoint - transform.position); to.y = 0f;
        if (to.sqrMagnitude < 0.0001f) return;
        Quaternion target = Quaternion.LookRotation(to.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeedDeg * Time.deltaTime);
    }

    Transform SelectMuzzle()
    {
        if (chooseClosestMuzzle)
            return muzzles.OrderBy(m => (m.position - PlayerCenter()).sqrMagnitude).First();

        if (alternateMuzzles)
        {
            muzzleIndex = (muzzleIndex + 1) % muzzles.Length;
            return muzzles[muzzleIndex];
        }
        return muzzles[0];
    }

    void SpawnBullet(Transform muzzle, Vector3 aimPoint)
    {
        Vector3 dir = (aimPoint - muzzle.position).normalized;
        var go = Instantiate(bulletPrefab, muzzle.position, Quaternion.LookRotation(dir, Vector3.up));
        if (go.TryGetComponent<Rigidbody>(out var rb))
            rb.linearVelocity = dir * bulletSpeed;

        // prevent instant self-hit
        if (go.TryGetComponent<Collider>(out var bc))
            foreach (var myCol in bossColliders) Physics.IgnoreCollision(bc, myCol, true);
    }

    Vector3 ComputeAimPoint(Vector3 muzzlePos)
    {
        Vector3 targetPos = PlayerCenter();
        if (!(useAimLead && playerRB)) return targetPos;

        Vector3 v = playerRB.linearVelocity;
        Vector3 toTarget = targetPos - muzzlePos;
        float a = Vector3.Dot(v, v) - bulletSpeed * bulletSpeed;
        float b = 2f * Vector3.Dot(v, toTarget);
        float c = Vector3.Dot(toTarget, toTarget);
        float t;
        if (Mathf.Abs(a) < 0.0001f) t = -c / Mathf.Max(b, 0.0001f);
        else
        {
            float d = b * b - 4f * a * c;
            if (d < 0f) return targetPos;
            float sqrt = Mathf.Sqrt(d);
            float t1 = (-b + sqrt) / (2f * a);
            float t2 = (-b - sqrt) / (2f * a);
            t = Mathf.Min(t1, t2);
        }
        if (t < 0.02f || t > 3f) return targetPos; // clamp bad solutions
        return targetPos + v * t;
    }

    Vector3 PlayerCenter()
    {
        if (!player) return Vector3.zero;
        var col = player.GetComponentInChildren<Collider>();
        Vector3 p = col ? col.bounds.center : player.position;
        return p + Vector3.up * verticalAimOffset;
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
        anim?.SetTrigger(TRIG_DIE);
        Halt();
    }
}
