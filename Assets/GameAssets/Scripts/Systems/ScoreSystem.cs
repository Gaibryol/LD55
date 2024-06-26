using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	//Score
	private int score;
	//Combo
	private int combo;
	//Multiplier
	private float multiplier;
	private int noteCombo;

	//Keep Track of Hits
	private int perfectHit;
	private int okayHit;
	private int badHit;
	private int miss;
	private int totalNotes;
	private int maxCombo;

	// Currently playing song
	private Constants.Songs.Song currentSong;

	// Difficulty
	private Constants.Songs.Difficulties currentDifficulty;

	//Text Showing Score & Combo
	[SerializeField] private GameObject scoreUI;
	[SerializeField] private TMP_Text scoreText;
	[SerializeField] private TMP_Text comboText;
	[SerializeField] private TMP_Text multiplierText;
	[SerializeField] private TMP_Text accuracyText;

	[SerializeField, Header("Summon")] private SpriteRenderer summonSprite;
	[SerializeField] private List<Sprite> summonItems;
	[SerializeField] Animator fullCircleAnimator;


	public void PlaySong(BrokerEvent<SongEvents.PlaySong> inEvent)
	{
		perfectHit = 0;
		okayHit = 0;
		badHit = 0;
		miss = 0;
		maxCombo = 0;
		noteCombo = 0;

		combo = 0;
		score = 0;
		multiplier = 1f;

		fullCircleAnimator.SetBool("Summon", false);

		currentSong = inEvent.Payload.Song;
		currentDifficulty = inEvent.Payload.Difficulty;
		eventBroker.Publish(this, new ScoreEvents.TotalNotes(currentSong, inEvent.Payload.Difficulty, (data) =>
		{
			totalNotes = data;
		}));

		scoreText.SetText(score.ToString());
		comboText.SetText(noteCombo.ToString() + " Combo");
		multiplierText.SetText(multiplier.ToString() + "X");
		accuracyText.SetText(CalculateAccuracy().ToString("F2") + "%");

		scoreUI.SetActive(true);
	}

	//Event Listener When Song Ends
	public void SongEnded(BrokerEvent<SongEvents.SongEnded> inEvent)
	{
		scoreUI.SetActive(false);

		int highscore;
		bool newRecord = false;
		if (PlayerPrefs.HasKey((Constants.Game.HighscorePP + currentSong + currentDifficulty).ToString()))
		{
			highscore = PlayerPrefs.GetInt((Constants.Game.HighscorePP + currentSong + currentDifficulty).ToString());
			if (score > highscore)
			{
				PlayerPrefs.SetInt((Constants.Game.HighscorePP + currentSong + currentDifficulty).ToString(), score);
				highscore = score;
				newRecord = true;
			}
		}
        else
        {
			PlayerPrefs.SetInt((Constants.Game.HighscorePP + currentSong + currentDifficulty).ToString(), score);
			highscore = score;
			newRecord = true;

        }
		PlayerPrefs.Save();

		string songTitle = "";
		switch (currentSong)
		{
			case Constants.Songs.Song.Song1:
				songTitle = Constants.Songs.Song1.Title;
				break;

			case Constants.Songs.Song.Song2:
				songTitle = Constants.Songs.Song2.Title;
				break;

			case Constants.Songs.Song.Song3:
				songTitle = Constants.Songs.Song3.Title;
				break;
		}

		if (inEvent.Payload.Success)
		{
			float accuracy = CalculateAccuracy();

			if (accuracy >= 90f)
			{
				summonSprite.sprite = summonItems[0];
			}
			else if (accuracy >= 80f)
			{
				summonSprite.sprite = summonItems[1];
			}
			else if (accuracy >= 70f)
			{
				summonSprite.sprite = summonItems[2];
			}
			else if (accuracy >= 60f)
			{
				summonSprite.sprite = summonItems[3];
			}
			else
			{
				summonSprite.sprite = summonItems[4];
			}

			fullCircleAnimator.SetBool("Summon", true);
		}

		if (!inEvent.Payload.Success)
		{
			eventBroker.Publish(this, new ScoreEvents.Final(songTitle, currentDifficulty, currentSong, score, CalculateAccuracy(), highscore, perfectHit, okayHit, badHit, miss, maxCombo, newRecord, perfectHit == totalNotes, null));
		}
		else
		{
			StartCoroutine(ScoreScreenOnDelay(songTitle, highscore, newRecord, summonSprite.sprite));
		}
	}

	private IEnumerator ScoreScreenOnDelay(string songTitle, int highscore, bool newRecord, Sprite summonSprite)
	{
		yield return new WaitForSeconds(8f);
		eventBroker.Publish(this, new ScoreEvents.Final(songTitle, currentDifficulty, currentSong, score, CalculateAccuracy(), highscore, perfectHit, okayHit, badHit, miss, maxCombo, newRecord, perfectHit == totalNotes, summonSprite));
	}

	private void PerfectHit(BrokerEvent<ScoreEvents.PerfectHit> inEvent)
	{
		combo += 1;
		perfectHit += 1;
		noteCombo += 1;
		CheckCombo();
		AddScore(Constants.Game.PerfectHit);
		if (combo == 50)
		{
			eventBroker.Publish(this, new ScoreEvents.Ascended(true));
		}
	}

	private void OkayHit(BrokerEvent<ScoreEvents.OkayHit> inEvent)
	{
		okayHit += 1;
		noteCombo += 1;
		CheckCombo();
		AddScore(Constants.Game.OkayHit);
	}
	private void BadHit(BrokerEvent<ScoreEvents.BadHit> inEvent)
	{
		badHit += 1;
		noteCombo += 1;
		CheckCombo();
		AddScore(Constants.Game.BadHit);
	}

	private void Miss(BrokerEvent<ScoreEvents.Miss> inEvent)
	{
		if (combo >= 50)
		{
			eventBroker.Publish(this, new ScoreEvents.Ascended(false));
		}
		combo = 0;
		miss += 1;
		noteCombo = 0;
		CheckCombo();
	}

	private void AddScore(int Amount)
    {
        score += Mathf.CeilToInt(multiplier * Amount);
		scoreText.SetText(score.ToString());
		accuracyText.SetText(CalculateAccuracy().ToString("F2")+"%");
	}

	private void CheckCombo()
	{
		if (combo < 10)
			multiplier = 1f;
		else if (10 <= combo && combo < 20)
			multiplier = 1.5f;
		else if (20 <= combo && combo < 30)
			multiplier = 2f;
		else if (30 <= combo && combo < 50)
			multiplier = 3f;
		else
			multiplier = 6f;

		comboText.SetText(noteCombo.ToString() + " Combo");
		multiplierText.SetText(multiplier.ToString() + "X");

		if (noteCombo > maxCombo)
		{
			maxCombo = noteCombo;
		}
	}

	private float CalculateAccuracy()
	{
		float accuracy = ((300f * perfectHit) + (200f * okayHit) + (100f * badHit)) / (300f * totalNotes) * 100;
		return accuracy;
	}


	private void OnEnable()
	{
		eventBroker.Subscribe<ScoreEvents.PerfectHit>(PerfectHit);
		eventBroker.Subscribe<ScoreEvents.OkayHit>(OkayHit);
		eventBroker.Subscribe<ScoreEvents.BadHit>(BadHit);
		eventBroker.Subscribe<ScoreEvents.Miss>(Miss);
		eventBroker.Subscribe<SongEvents.PlaySong>(PlaySong);
		eventBroker.Subscribe<SongEvents.SongEnded>(SongEnded);

	}

	private void OnDisable()
	{
		eventBroker.Unsubscribe<ScoreEvents.PerfectHit>(PerfectHit);
		eventBroker.Unsubscribe<ScoreEvents.OkayHit>(OkayHit);
		eventBroker.Unsubscribe<ScoreEvents.BadHit>(BadHit);
		eventBroker.Unsubscribe<ScoreEvents.Miss>(Miss);
		eventBroker.Unsubscribe<SongEvents.PlaySong>(PlaySong);
		eventBroker.Unsubscribe<SongEvents.SongEnded>(SongEnded);
	}
}