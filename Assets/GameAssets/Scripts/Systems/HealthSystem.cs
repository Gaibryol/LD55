using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();
    [SerializeField] private GameObject healthBar;
    [SerializeField] private Slider slider;
    private float currentHealth  = 100f;
    private float lerpSpeed = 0.25f;
    private float time;
    private bool lostGame;

    public void PlaySong(BrokerEvent<SongEvents.PlaySong> inEvent)
    {
        lostGame = false;
        currentHealth = slider.maxValue;
        slider.value = slider.maxValue;
        healthBar.SetActive(true);
    }
    public void SongEnded(BrokerEvent<SongEvents.SongEnded> inEvent)
    {
        healthBar.SetActive(false);
    }
    public void PerfectHit(BrokerEvent<ScoreEvents.PerfectHit> inEvent)
    {
        currentHealth += Constants.Game.PerfectHitHeal;
        if (currentHealth > 100f)
            currentHealth = 100f;
        time = 0;
    }
    public void Miss(BrokerEvent<ScoreEvents.Miss> inEvent)
    {
        currentHealth += Constants.Game.MissDamage;
        if (currentHealth < 0f)
            currentHealth = 0f;
        time = 0;
    }

    private void Update()
    {
        AnitmateHealthBar();
        if (currentHealth == 0 && !lostGame)
        {
            Debug.Log("Lost Game");
            lostGame = true;
        }
    }
    private void AnitmateHealthBar()
    {
        float targetHealth = currentHealth;
        float startHealth = slider.value;
        time += Time.deltaTime * lerpSpeed;
        slider.value = Mathf.Lerp(startHealth, targetHealth, time);
    }

    private void OnEnable()
    {
        eventBroker.Subscribe<SongEvents.PlaySong>(PlaySong);
        eventBroker.Subscribe<SongEvents.SongEnded>(SongEnded);
        eventBroker.Subscribe<ScoreEvents.PerfectHit>(PerfectHit);
        eventBroker.Subscribe<ScoreEvents.Miss>(Miss);
    }
    private void OnDisable()
    {
        eventBroker.Unsubscribe<SongEvents.PlaySong>(PlaySong);
        eventBroker.Unsubscribe<SongEvents.SongEnded>(SongEnded);
        eventBroker.Unsubscribe<ScoreEvents.PerfectHit>(PerfectHit);
        eventBroker.Unsubscribe<ScoreEvents.Miss>(Miss);
    }
}
