using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreScreen : MonoBehaviour
{
    private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	[SerializeField] private TMP_Text titleText;
	[SerializeField] private TMP_Text difficultyText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text accuracyText;
    [SerializeField] private TMP_Text highscoreText;
    [SerializeField] private TMP_Text perfectsText;
    [SerializeField] private TMP_Text okaysText;
    [SerializeField] private TMP_Text badsText;
    [SerializeField] private TMP_Text missesText;
	[SerializeField] private TMP_Text maxComboText;
	[SerializeField] private GameObject endScorePanel;
	[SerializeField] private GameObject newRecord;
	[SerializeField] private GameObject allPerfect;
	[SerializeField] private Button playAgainButton;

	[SerializeField] private GameManager gameManager;

	private Constants.Songs.Song lastSong;
	private Constants.Songs.Difficulties lastDifficulty;

    public void Final(BrokerEvent<ScoreEvents.Final> inEvent)
    {
		titleText.text = inEvent.Payload.Title;
		difficultyText.text = inEvent.Payload.Difficulty.ToString();
		scoreText.text = inEvent.Payload.Score.ToString();
		accuracyText.text = inEvent.Payload.Accuracy.ToString("F2") + "%";
		highscoreText.text = inEvent.Payload.Highscore.ToString();
		maxComboText.text = inEvent.Payload.MaxCombo.ToString();

		perfectsText.text = inEvent.Payload.PerfectHit.ToString();
		okaysText.text = inEvent.Payload.OkayHit.ToString();
		badsText.text = inEvent.Payload.BadHit.ToString();
		missesText.text = inEvent.Payload.Misses.ToString();

		newRecord.SetActive(inEvent.Payload.NewRecord);
		allPerfect.SetActive(inEvent.Payload.AllPerfect);

		newRecord.SetActive(inEvent.Payload.NewRecord);
		allPerfect.SetActive(inEvent.Payload.AllPerfect);

		lastSong = inEvent.Payload.Song;
		lastDifficulty = inEvent.Payload.Difficulty;

        endScorePanel.SetActive(true);
    }

	private void PlayAgain()
	{
		// Play same song
		gameManager.SelectSong(lastSong, lastDifficulty);
		endScorePanel.SetActive(false);
	}

    private void OnEnable()
    {
        eventBroker.Subscribe<ScoreEvents.Final>(Final);

		playAgainButton.onClick.AddListener(PlayAgain);
	}

    private void OnDisable()
    {
        eventBroker.Unsubscribe<ScoreEvents.Final>(Final);

		playAgainButton.onClick.RemoveListener(PlayAgain);
	}

}
