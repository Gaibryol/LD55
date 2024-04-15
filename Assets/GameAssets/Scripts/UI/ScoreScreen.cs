using UnityEngine;
using TMPro;

public class ScoreScreen : MonoBehaviour
{
    private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text accuracyText;
    [SerializeField] private TMP_Text highscoreText;
    [SerializeField] private TMP_Text perfectsText;
    [SerializeField] private TMP_Text okaysText;
    [SerializeField] private TMP_Text badsText;
    [SerializeField] private TMP_Text missesText;
	[SerializeField] private GameObject endScorePanel;

    private string score;
    private string accuracy;

    public void Final(BrokerEvent<ScoreEvents.Final> inEvent)
    {
        score = "Score: " + inEvent.Payload.Score.ToString();
        accuracy = "Accuracy: " + inEvent.Payload.Accuracy.ToString("F2") + "%";


        scoreText.SetText(score);
        accuracyText.SetText(accuracy);

        endScorePanel.SetActive(true);
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
