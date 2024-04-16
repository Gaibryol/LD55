using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SongSelectionUI : MonoBehaviour
{
	[SerializeField] private GameManager gameManager;
	[SerializeField] private GameObject screen;
	[SerializeField] private Button prevButton;
	[SerializeField] private Button nextButton;
	[SerializeField] private Button backButton;
	[SerializeField] private Button normalDifficultyButton;
	[SerializeField] private Button hardDifficultyButton;
	[SerializeField] private TMP_Text songName;
	[SerializeField] private TMP_Text songLength;
	[SerializeField] private TMP_Text normalHighscore;
	[SerializeField] private TMP_Text hardHighscore;

	private int index;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	private void Start()
	{
		index = 0;
	}

	public void EnableSongSelection()
	{
		index = 0;
		UpdateUI();
		screen.SetActive(true);
	}

	private void UpdateUI()
	{
		if (index == 0)
		{
			// Song 2
			songName.text = Constants.Songs.Song2.Title.ToString();
			eventBroker.Publish(this, new AudioEvents.GetSongLength(Constants.Songs.Song.Song2.ToString(), (length) => { songLength.text = TimeSpan.FromSeconds(length).ToString("mm':'ss"); }));

			if (PlayerPrefs.HasKey((Constants.Game.HighscorePP + Constants.Songs.Song.Song2 + Constants.Songs.Difficulties.Normal).ToString()))
			{
				normalHighscore.text = PlayerPrefs.GetInt((Constants.Game.HighscorePP + Constants.Songs.Song.Song2 + Constants.Songs.Difficulties.Normal).ToString()).ToString();
				normalHighscore.gameObject.SetActive(true);
			}
			else
			{
				normalHighscore.gameObject.SetActive(false);
			}

			if (PlayerPrefs.HasKey((Constants.Game.HighscorePP + Constants.Songs.Song.Song2 + Constants.Songs.Difficulties.Hard).ToString()))
			{
				hardHighscore.text = PlayerPrefs.GetInt((Constants.Game.HighscorePP + Constants.Songs.Song.Song2 + Constants.Songs.Difficulties.Hard).ToString()).ToString();
				hardHighscore.gameObject.SetActive(true);
			}
			else
			{
				hardHighscore.gameObject.SetActive(false);
			}

			normalDifficultyButton.onClick.RemoveAllListeners();
			hardDifficultyButton.onClick.RemoveAllListeners();

			normalDifficultyButton.onClick.AddListener(() => gameManager.SelectSong(Constants.Songs.Song.Song2, Constants.Songs.Difficulties.Normal));
			hardDifficultyButton.onClick.AddListener(() => gameManager.SelectSong(Constants.Songs.Song.Song2, Constants.Songs.Difficulties.Hard));

			eventBroker.Publish(this, new AudioEvents.PlayMusic(Constants.Audio.Music.Song2Preview, true));
		}
		else if (index == 1)
		{
			// Song 3
			songName.text = Constants.Songs.Song3.Title.ToString();
			eventBroker.Publish(this, new AudioEvents.GetSongLength(Constants.Songs.Song.Song3.ToString(), (length) => { songLength.text = TimeSpan.FromSeconds(length).ToString("mm':'ss"); }));

			if (PlayerPrefs.HasKey((Constants.Game.HighscorePP + Constants.Songs.Song.Song3 + Constants.Songs.Difficulties.Normal).ToString()))
			{
				normalHighscore.text = PlayerPrefs.GetInt((Constants.Game.HighscorePP + Constants.Songs.Song.Song3 + Constants.Songs.Difficulties.Normal).ToString()).ToString();
				normalHighscore.gameObject.SetActive(true);
			}
			else
			{
				normalHighscore.gameObject.SetActive(false);
			}

			if (PlayerPrefs.HasKey((Constants.Game.HighscorePP + Constants.Songs.Song.Song3 + Constants.Songs.Difficulties.Hard).ToString()))
			{
				hardHighscore.text = PlayerPrefs.GetInt((Constants.Game.HighscorePP + Constants.Songs.Song.Song3 + Constants.Songs.Difficulties.Hard).ToString()).ToString();
				hardHighscore.gameObject.SetActive(true);
			}
			else
			{
				hardHighscore.gameObject.SetActive(false);
			}

			normalDifficultyButton.onClick.RemoveAllListeners();
			hardDifficultyButton.onClick.RemoveAllListeners();

			normalDifficultyButton.onClick.AddListener(() => gameManager.SelectSong(Constants.Songs.Song.Song3, Constants.Songs.Difficulties.Normal));
			hardDifficultyButton.onClick.AddListener(() => gameManager.SelectSong(Constants.Songs.Song.Song3, Constants.Songs.Difficulties.Hard));

			eventBroker.Publish(this, new AudioEvents.PlayMusic(Constants.Audio.Music.Song3Preview, true));
		}
		else
		{
			// Song 1
			songName.text = Constants.Songs.Song1.Title.ToString();
			eventBroker.Publish(this, new AudioEvents.GetSongLength(Constants.Songs.Song.Song1.ToString(), (length) => { songLength.text = TimeSpan.FromSeconds(length).ToString("mm':'ss"); }));

			if (PlayerPrefs.HasKey((Constants.Game.HighscorePP + Constants.Songs.Song.Song1 + Constants.Songs.Difficulties.Normal).ToString()))
			{
				normalHighscore.text = PlayerPrefs.GetInt((Constants.Game.HighscorePP + Constants.Songs.Song.Song1 + Constants.Songs.Difficulties.Normal).ToString()).ToString();
				normalHighscore.gameObject.SetActive(true);
			}
			else
			{
				normalHighscore.gameObject.SetActive(false);
			}

			if (PlayerPrefs.HasKey((Constants.Game.HighscorePP + Constants.Songs.Song.Song1 + Constants.Songs.Difficulties.Hard).ToString()))
			{
				hardHighscore.text = PlayerPrefs.GetInt((Constants.Game.HighscorePP + Constants.Songs.Song.Song1 + Constants.Songs.Difficulties.Hard).ToString()).ToString();
				hardHighscore.gameObject.SetActive(true);
			}
			else
			{
				hardHighscore.gameObject.SetActive(false);
			}

			normalDifficultyButton.onClick.RemoveAllListeners();
			hardDifficultyButton.onClick.RemoveAllListeners();

			normalDifficultyButton.onClick.AddListener(() => gameManager.SelectSong(Constants.Songs.Song.Song1, Constants.Songs.Difficulties.Normal));
			hardDifficultyButton.onClick.AddListener(() => gameManager.SelectSong(Constants.Songs.Song.Song1, Constants.Songs.Difficulties.Hard));

			eventBroker.Publish(this, new AudioEvents.PlayMusic(Constants.Audio.Music.Song1Preview, true));
		}
	}

	private void OnPrevButtonClick()
	{
		index -= 1;
		if (index < 0)
		{
			index = 2;
		}

		UpdateUI();
	}

	private void OnNextButtonClick()
	{
		index += 1;
		if (index > 2)
		{
			index = 0;
		}

		UpdateUI();
	}

	private void OnBackButtonClick()
	{
		// Back to main menu
		screen.SetActive(false);
		eventBroker.Publish(this, new AudioEvents.StopMusic());
	}

	private void OnEnable()
	{
		prevButton.onClick.AddListener(OnPrevButtonClick);
		nextButton.onClick.AddListener(OnNextButtonClick);
		backButton.onClick.AddListener(OnBackButtonClick);
	}

	private void OnDisable()
	{
		prevButton.onClick.RemoveListener(OnPrevButtonClick);
		nextButton.onClick.RemoveListener(OnNextButtonClick);
		backButton.onClick.RemoveListener(OnBackButtonClick);
	}
}
