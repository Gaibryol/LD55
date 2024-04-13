using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();
    [SerializeField] private GameObject healthBar;
    [SerializeField] private Slider slider;

    public void PlaySong(BrokerEvent<SongEvents.PlaySong> inEvent)
    {
        slider.value = slider.maxValue;
        healthBar.SetActive(true);
    }
    public void SongEnded(BrokerEvent<SongEvents.SongEnded> inEvent)
    {
        healthBar.SetActive(false);
    }
    public void PerfectHit(BrokerEvent<ScoreEvents.PerfectHit> inEvent)
    {
        slider.value += Constants.Game.PerfectHitHeal;
    }
    public void Miss(BrokerEvent<ScoreEvents.Miss> inEvent)
    {
        slider.value += Constants.Game.MissDamage;
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
