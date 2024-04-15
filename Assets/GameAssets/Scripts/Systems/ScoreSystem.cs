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

	//Keep Track of Hits
	private int perfectHit;
	private int okayHit;
	private int badHit;
	private int miss;
	private int totalNotes;

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


	public void PlaySong(BrokerEvent<SongEvents.PlaySong> inEvent)
	{
		perfectHit = 0;
		okayHit = 0;
		badHit = 0;
		miss = 0;

		combo = 0;
		score = 0;
		multiplier = 1f;

		currentSong = inEvent.Payload.Song;
		currentDifficulty = inEvent.Payload.Difficulty;
		eventBroker.Publish(this, new ScoreEvents.TotalNotes(currentSong, inEvent.Payload.Difficulty, (data) =>
		{
			totalNotes = data;
		}));

		scoreText.SetText(score.ToString());
		comboText.SetText(combo.ToString() + " Combo");
		multiplierText.SetText(multiplier.ToString() + "X");
		accuracyText.SetText(CalculateAccuracy().ToString("F2") + "%");

		scoreUI.SetActive(true);
	}

	//Event Listener When Song Ends
	public void SongEnded(BrokerEvent<SongEvents.SongEnded> inEvent)
	{
		scoreUI.SetActive(false);
		int highscore;
		if (PlayerPrefs.HasKey((Constants.Game.HighscorePP + currentSong + currentDifficulty).ToString()))
		{
			highscore = PlayerPrefs.GetInt((Constants.Game.HighscorePP + currentSong + currentDifficulty).ToString());
			if (score > highscore)
			{
				PlayerPrefs.SetInt((Constants.Game.HighscorePP + currentSong + currentDifficulty).ToString(), score);
				highscore = score;
			}
		}
        else
        {
			PlayerPrefs.SetInt((Constants.Game.HighscorePP + currentSong + currentDifficulty).ToString(), score);
			highscore = score;

        }
		eventBroker.Publish(this, new ScoreEvents.Final(score, CalculateAccuracy(), highscore, perfectHit, okayHit,badHit, miss));
	}

	private void PerfectHit(BrokerEvent<ScoreEvents.PerfectHit> inEvent)
	{
		combo += 1;
		perfectHit += 1;
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
		CheckCombo();
		AddScore(Constants.Game.OkayHit);
	}
	private void BadHit(BrokerEvent<ScoreEvents.BadHit> inEvent)
	{
		badHit += 1;
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

		comboText.SetText(combo.ToString() + " Combo!");
		multiplierText.SetText(multiplier.ToString() + "X");
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