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

	//Text Showing Score & Combo
	[SerializeField] private TMP_Text scoreText;
	[SerializeField] private TMP_Text comboText;
	[SerializeField] private TMP_Text multiplierText;

    private void Start()
    {
		combo = 0;
		score = 0;
		multiplier = 1f;
		CheckCombo();
		AddScore(Constants.Game.PerfectHit);
    }

    private void PerfectHit(BrokerEvent<ScoreEvents.PerfectHit> inEvent)
	{
		combo += 1;
		perfectHit += 1;
		CheckCombo();
		AddScore(Constants.Game.PerfectHit);
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
		combo = 0;
		CheckCombo();
	}

	private void AddScore(int Amount)
    {
		score += Mathf.CeilToInt(multiplier * Amount);
		scoreText.SetText(score.ToString());
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

		comboText.SetText(combo.ToString());
		multiplierText.SetText(multiplier.ToString() + "x");
	}
	private float CalculateAccuracy()
    {
		float accuracy;
		accuracy = ((300f * perfectHit) + (200f * okayHit) + (100f * badHit)) / (300f * (perfectHit + okayHit + badHit + miss));
		return accuracy;
    }

	//Event Listener When Song Start
	public void StartSong()
    {
		perfectHit = 0;
		okayHit = 0;
		badHit = 0;
		miss = 0;

		combo = 0;
		score = 0;
		multiplier = 1f;

		scoreText.SetText(score.ToString());
		comboText.SetText(combo.ToString());
		multiplierText.SetText(multiplier.ToString() + "x");

		scoreText.gameObject.SetActive(true);
		comboText.gameObject.SetActive(true);
		multiplierText.gameObject.SetActive(true);
	}

	//Event Listener When Song Ends
	public void EndSong()
    {
		scoreText.gameObject.SetActive(false);
		comboText.gameObject.SetActive(false);
		multiplierText.gameObject.SetActive(false);
		eventBroker.Publish(this, new ScoreEvents.Final(score, CalculateAccuracy()));

	}

    private void OnEnable()
    {
		eventBroker.Subscribe<ScoreEvents.PerfectHit>(PerfectHit);
		eventBroker.Subscribe<ScoreEvents.OkayHit>(OkayHit);
		eventBroker.Subscribe<ScoreEvents.BadHit>(BadHit);
		eventBroker.Subscribe<ScoreEvents.Miss>(Miss);
		//Need To Listen To End Of Song To Give Final Score;
		//Need To Listen To How Many Notes The Song Has;

	}
    private void OnDisable()
    {
		eventBroker.Unsubscribe<ScoreEvents.PerfectHit>(PerfectHit);
		eventBroker.Unsubscribe<ScoreEvents.OkayHit>(OkayHit);
		eventBroker.Unsubscribe<ScoreEvents.BadHit>(BadHit);
		eventBroker.Unsubscribe<ScoreEvents.Miss>(Miss);
		//Need To Listen To End Of Song To Give Final Score;
		//Need To Listen To How Many Notes The Song Has;

	}
}
