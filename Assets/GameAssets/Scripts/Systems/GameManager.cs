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

	[SerializeField, Header("UI")] private GameObject mainMenuPanel;
	[SerializeField] private Button song1NormalButton;
	[SerializeField] private Button song1HardButton;
	[SerializeField] private Button song2NormalButton;
	[SerializeField] private Button song2HardButton;

	private List<Queue<float>> song1NormalQueues;
	private List<Queue<float>> song1HardQueues;
	private List<Queue<float>> song2NormalQueues;
	private List<Queue<float>> song2HardQueues;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	private void Awake()
	{
		midiConverter.JsonConvert(song1Normal, out Queue<float> song1NormalLeft, out Queue<float> song1NormalUp, out Queue<float> song1NormalRight, out Queue<float> song1NormalDown);
		song1NormalQueues = new List<Queue<float>>() { song1NormalUp, song1NormalDown, song1NormalLeft, song1NormalRight };

		midiConverter.JsonConvert(song1Hard, out Queue<float> song1DiffLeft, out Queue<float> song1DiffUp, out Queue<float> song1DiffRight, out Queue<float> song1DiffDown);
		song1HardQueues = new List<Queue<float>>() { song1DiffUp, song1DiffDown, song1DiffLeft, song1DiffRight };

		midiConverter.JsonConvert(song2Normal, out Queue<float> song2NormalLeft, out Queue<float> song2NormalUp, out Queue<float> song2NormalRight, out Queue<float> song2NormalDown);
		song2NormalQueues = new List<Queue<float>>() { song2NormalUp, song2NormalDown, song2NormalLeft, song2NormalRight };

		midiConverter.JsonConvert(song2Hard, out Queue<float> song2DifficultLeft, out Queue<float> song2DifficultUp, out Queue<float> song2DifficultRight, out Queue<float> song2DifficultDown);
		song2HardQueues = new List<Queue<float>>() { song2DifficultUp, song2DifficultDown, song2DifficultLeft, song2DifficultRight };
	}

	public void SelectSong(Constants.Songs.Song song, Constants.Songs.Difficulties difficulty)
	{
		eventBroker.Publish(this, new AudioEvents.GetSongLength(song.ToString(), (length) => { StartCoroutine(OnSongEnd(length)); }));
		eventBroker.Publish(this, new SongEvents.PlaySong(song, difficulty));
		mainMenuPanel.SetActive(false);
	}

	private IEnumerator OnSongEnd(float length)
	{
		yield return new WaitForSeconds(length);

		// Song ended
		eventBroker.Publish(this, new SongEvents.SongEnded());
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
		}

		inEvent.Payload.ProcessData.DynamicInvoke(total);
	}

	private void OnEnable()
	{
		eventBroker.Subscribe<SongEvents.GetSongData>(GetSongDataHandler);
		eventBroker.Subscribe<ScoreEvents.TotalNotes>(TotalNotesHandler);

		song1NormalButton.onClick.AddListener(() => SelectSong(Constants.Songs.Song.Song1, Constants.Songs.Difficulties.Normal));
		song1HardButton.onClick.AddListener(() => SelectSong(Constants.Songs.Song.Song1, Constants.Songs.Difficulties.Hard));
		song2NormalButton.onClick.AddListener(() => SelectSong(Constants.Songs.Song.Song2, Constants.Songs.Difficulties.Normal));
		song2HardButton.onClick.AddListener(() => SelectSong(Constants.Songs.Song.Song2, Constants.Songs.Difficulties.Hard));
	}

	private void OnDisable()
	{
		eventBroker.Unsubscribe<SongEvents.GetSongData>(GetSongDataHandler);
		eventBroker.Unsubscribe<ScoreEvents.TotalNotes>(TotalNotesHandler);

		song1NormalButton.onClick.RemoveListener(() => SelectSong(Constants.Songs.Song.Song1, Constants.Songs.Difficulties.Normal));
		song1HardButton.onClick.RemoveListener(() => SelectSong(Constants.Songs.Song.Song1, Constants.Songs.Difficulties.Hard));
		song2NormalButton.onClick.RemoveListener(() => SelectSong(Constants.Songs.Song.Song2, Constants.Songs.Difficulties.Normal));
		song2HardButton.onClick.RemoveListener(() => SelectSong(Constants.Songs.Song.Song2, Constants.Songs.Difficulties.Hard));
	}
}
