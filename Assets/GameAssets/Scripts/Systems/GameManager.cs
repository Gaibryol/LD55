using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[SerializeField, Header("Songs")] private MidiConverter midiConverter;
	[SerializeField] private TextAsset song1Normal;
	[SerializeField] private TextAsset song1Hard;
	[SerializeField] private TextAsset song2Normal;
	[SerializeField] private TextAsset song2Hard;
	[SerializeField] private TextAsset song3Normal;
	[SerializeField] private TextAsset song3Hard;

	[SerializeField, Header("UI")] private GameObject mainMenuPanel;
	[SerializeField] private GameObject summoningCircleUI;

	private List<Queue<float>> song1NormalQueues;
	private List<Queue<float>> song1HardQueues;
	private List<Queue<float>> song2NormalQueues;
	private List<Queue<float>> song2HardQueues;
	private List<Queue<float>> song3NormalQueues;
	private List<Queue<float>> song3HardQueues;

	private bool playing;

	private Coroutine songEndCheck;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	private void Awake()
	{
		playing = false;

		midiConverter.JsonConvert(song1Normal, out Queue<float> song1NormalLeft, out Queue<float> song1NormalUp, out Queue<float> song1NormalRight, out Queue<float> song1NormalDown);
		song1NormalQueues = new List<Queue<float>>() { song1NormalUp, song1NormalDown, song1NormalLeft, song1NormalRight };

		midiConverter.JsonConvert(song1Hard, out Queue<float> song1DiffLeft, out Queue<float> song1DiffUp, out Queue<float> song1DiffRight, out Queue<float> song1DiffDown);
		song1HardQueues = new List<Queue<float>>() { song1DiffUp, song1DiffDown, song1DiffLeft, song1DiffRight };

		midiConverter.JsonConvert(song2Normal, out Queue<float> song2NormalLeft, out Queue<float> song2NormalUp, out Queue<float> song2NormalRight, out Queue<float> song2NormalDown);
		song2NormalQueues = new List<Queue<float>>() { song2NormalUp, song2NormalDown, song2NormalLeft, song2NormalRight };

		midiConverter.JsonConvert(song2Hard, out Queue<float> song2DifficultLeft, out Queue<float> song2DifficultUp, out Queue<float> song2DifficultRight, out Queue<float> song2DifficultDown);
		song2HardQueues = new List<Queue<float>>() { song2DifficultUp, song2DifficultDown, song2DifficultLeft, song2DifficultRight };

		midiConverter.JsonConvert(song3Normal, out Queue<float> song3NormalLeft, out Queue<float> song3NormalUp, out Queue<float> song3NormalRight, out Queue<float> song3NormalDown);
		song3NormalQueues = new List<Queue<float>>() { song3NormalUp, song3NormalDown, song3NormalLeft, song3NormalRight };

		midiConverter.JsonConvert(song3Hard, out Queue<float> song3DifficultLeft, out Queue<float> song3DifficultUp, out Queue<float> song3DifficultRight, out Queue<float> song3DifficultDown);
		song3HardQueues = new List<Queue<float>>() { song3DifficultUp, song3DifficultDown, song3DifficultLeft, song3DifficultRight };
	}

	private void Start()
	{
		eventBroker.Publish(this, new AudioEvents.PlayMusic(Constants.Audio.Music.MainMenuTheme));
	}

	private void Update()
	{
		summoningCircleUI.transform.Rotate(0f, 0f, -Time.deltaTime * 5f);
	}

	public void BackToMainMenu()
	{
		eventBroker.Publish(this, new AudioEvents.PlayMusic(Constants.Audio.Music.MainMenuTheme));
	}

	public void SelectSong(Constants.Songs.Song song, Constants.Songs.Difficulties difficulty)
	{
		eventBroker.Publish(this, new AudioEvents.GetSongLength(song.ToString(), (length) => 
		{
			eventBroker.Publish(this, new SongEvents.PlaySong(song, difficulty));
			mainMenuPanel.SetActive(false);

			playing = true;

			if (songEndCheck != null)
			{
				StopCoroutine(songEndCheck);
			}

			songEndCheck = StartCoroutine(OnSongEnd(length));
		}));
	}

	private void FinalHandler(BrokerEvent<ScoreEvents.Final> inEvent)
	{
		playing = false;
		
		if (songEndCheck != null)
		{
			StopCoroutine(songEndCheck);
		}
	}

	private IEnumerator OnSongEnd(float length)
	{
		float timer = 0f;
		while (playing)
		{
			if (timer >= length)
			{
				playing = false;
			}

			timer += Time.deltaTime;
			yield return null;
		}

		// Song ended
		eventBroker.Publish(this, new SongEvents.SongEnded(true));
	}

	private void GetSongDataHandler(BrokerEvent<SongEvents.GetSongData> inEvent)
	{
		switch (inEvent.Payload.Song)
		{
			case Constants.Songs.Song.Song1:
				if (inEvent.Payload.Difficulty == Constants.Songs.Difficulties.Normal)
				{
					inEvent.Payload.ProcessData.DynamicInvoke(song1NormalQueues);
				}
				else if (inEvent.Payload.Difficulty == Constants.Songs.Difficulties.Hard)
				{
					inEvent.Payload.ProcessData.DynamicInvoke(song1HardQueues);
				}
				break;

			case Constants.Songs.Song.Song2:
				if (inEvent.Payload.Difficulty == Constants.Songs.Difficulties.Normal)
				{
					inEvent.Payload.ProcessData.DynamicInvoke(song2NormalQueues);
				}
				else if (inEvent.Payload.Difficulty == Constants.Songs.Difficulties.Hard)
				{
					inEvent.Payload.ProcessData.DynamicInvoke(song2HardQueues);
				}
				break;

			case Constants.Songs.Song.Song3:
				if (inEvent.Payload.Difficulty == Constants.Songs.Difficulties.Normal)
				{
					inEvent.Payload.ProcessData.DynamicInvoke(song3NormalQueues);
				}
				else if (inEvent.Payload.Difficulty == Constants.Songs.Difficulties.Hard)
				{
					inEvent.Payload.ProcessData.DynamicInvoke(song3HardQueues);
				}
				break;
		}
	}

	private void TotalNotesHandler(BrokerEvent<ScoreEvents.TotalNotes> inEvent)
	{
		int total = 0;

		switch (inEvent.Payload.Song)
		{
			case Constants.Songs.Song.Song1:
				if (inEvent.Payload.Difficulty == Constants.Songs.Difficulties.Normal)
				{
					total += song1NormalQueues[0].Count + song1NormalQueues[1].Count + song1NormalQueues[2].Count + song1NormalQueues[3].Count;
				}
				else if (inEvent.Payload.Difficulty == Constants.Songs.Difficulties.Hard)
				{
					total += song1HardQueues[0].Count + song1HardQueues[1].Count + song1HardQueues[2].Count + song1HardQueues[3].Count;
				}
				break;

			case Constants.Songs.Song.Song2:
				if (inEvent.Payload.Difficulty == Constants.Songs.Difficulties.Normal)
				{
					total += song2NormalQueues[0].Count + song2NormalQueues[1].Count + song2NormalQueues[2].Count + song2NormalQueues[3].Count;
				}
				else if (inEvent.Payload.Difficulty == Constants.Songs.Difficulties.Hard)
				{
					total += song2HardQueues[0].Count + song2HardQueues[1].Count + song2HardQueues[2].Count + song2HardQueues[3].Count;
				}
				break;

			case Constants.Songs.Song.Song3:
				if (inEvent.Payload.Difficulty == Constants.Songs.Difficulties.Normal)
				{
					total += song3NormalQueues[0].Count + song3NormalQueues[1].Count + song3NormalQueues[2].Count + song3NormalQueues[3].Count;
				}
				else if (inEvent.Payload.Difficulty == Constants.Songs.Difficulties.Hard)
				{
					total += song3HardQueues[0].Count + song3HardQueues[1].Count + song3HardQueues[2].Count + song3HardQueues[3].Count;
				}
				break;
		}

		inEvent.Payload.ProcessData.DynamicInvoke(total);
	}

	private void OnEnable()
	{
		eventBroker.Subscribe<SongEvents.GetSongData>(GetSongDataHandler);
		eventBroker.Subscribe<ScoreEvents.TotalNotes>(TotalNotesHandler);
		eventBroker.Subscribe<ScoreEvents.Final>(FinalHandler);
	}

	private void OnDisable()
	{
		eventBroker.Unsubscribe<SongEvents.GetSongData>(GetSongDataHandler);
		eventBroker.Unsubscribe<ScoreEvents.TotalNotes>(TotalNotesHandler);
		eventBroker.Unsubscribe<ScoreEvents.Final>(FinalHandler);
	}
}
