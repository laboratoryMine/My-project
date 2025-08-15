using UnityEngine;

public class BossFightDirector_Animator : MonoBehaviour
{
    public enum FightState { PreFight, Phase1_Race, Phase2_Combat, Won, Lost }

    [Header("Refs")]
    public CPUCountdown countdown;
    public BossAIWithAnimator bossAI;
    public Health bossHealth;
    public Animator bossAnimator;
    public Transform player;

    [Header("UI Groups")]
    public GameObject phase1UI;
    public GameObject bossHealthUI;
    //public GameObject winPanel;
    //public GameObject losePanel;

    [Header("Misc")]
    public string playerTag = "Player";
    public bool pauseOnEnd = true;

    FightState state = FightState.PreFight;
    int idleID = Animator.StringToHash("idle");

    void Start()
    {
        if (bossAnimator) bossAnimator.SetTrigger(idleID);

        phase1UI?.SetActive(false);
        bossHealthUI?.SetActive(false);
        //winPanel?.SetActive(false);
        //losePanel?.SetActive(false);

        if (countdown)
        {
            countdown.onTimeExpired.AddListener(OnTimeExpired);
            countdown.StartTimer(); // begin Phase 1
        }
        if (bossHealth) bossHealth.onDied.AddListener(OnBossDied);

        state = FightState.Phase1_Race;
        phase1UI?.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (state == FightState.Phase1_Race && other.CompareTag(playerTag))
            StartPhase2();
    }

    void StartPhase2()
    {
        state = FightState.Phase2_Combat;
        phase1UI?.SetActive(false);
        countdown?.StopTimer();
        bossHealthUI?.SetActive(true);

        bossAI.enabled = true;
        bossAI.Begin();
    }

    void OnTimeExpired()
    {
        if (state == FightState.Phase1_Race) Lose();
    }

    void OnBossDied()
    {
        if (state != FightState.Phase2_Combat) return;
        bossAI.PlayDeath();
        Win();
    }

    void Win()
    {
        state = FightState.Won;
        bossAI.Halt();
        bossAI.enabled = false;
        //winPanel?.SetActive(true);
        if (pauseOnEnd) Time.timeScale = 0f;
    }

    void Lose()
    {
        state = FightState.Lost;
        bossAI.Halt();
        bossAI.enabled = false;
        //losePanel?.SetActive(true);
        if (pauseOnEnd) Time.timeScale = 0f;
    }
}
