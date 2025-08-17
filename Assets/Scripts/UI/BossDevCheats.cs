using UnityEngine;

public class BossDevCheats : MonoBehaviour
{
    public Health boss;
    public BossFightDirector_Animator director;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) boss?.TakeDamage(50);
        if (Input.GetKeyDown(KeyCode.L)) boss?.TakeDamage(99999);
        if (Input.GetKeyDown(KeyCode.N)) director?.SendMessage("StartPhase2", SendMessageOptions.DontRequireReceiver);
        if (Input.GetKeyDown(KeyCode.P)) Time.timeScale = Time.timeScale < 0.5f ? 1f : 0f;
    }
}
