using UnityEngine;
using TMPro;

public class ScoreScreen : MonoBehaviour
{
    private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text accuracyText;
    private string score;
    private string accuracy;

    public void Final(BrokerEvent<ScoreEvents.Final> inEvent)
    {
        score = "Score: " + inEvent.Payload.Score.ToString();
        accuracy = "Accuracy: " + inEvent.Payload.Accuracy.ToString() + "%";
        scoreText.SetText(score);
        accuracyText.SetText(accuracy);
        gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);
        accuracyText.gameObject.SetActive(true);

    }



    private void OnEnable()
    {
        eventBroker.Subscribe<ScoreEvents.Final>(Final);
    }
    private void OnDisable()
    {
        eventBroker.Unsubscribe<ScoreEvents.Final>(Final);
    }

}
