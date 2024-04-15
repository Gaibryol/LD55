using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[SerializeField, Header("Songs")] private MidiConverter midiConverter;
	[SerializeField] private TextAsset song1;
	[SerializeField] private TextAsset song2;

	[SerializeField, Header("UI")] private GameObject mainMenuPanel;
	[SerializeField] private Button song1Button;
	[SerializeField] private Button song2Button;

	private List<Queue<float>> song1Queues;
	private List<Queue<float>> song2Queues;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	private void Awake()
	{
		midiConverter.JsonConvert(song1, out Queue<float> song1Left, out Queue<float> song1Up, out Queue<float> song1Right, out Queue<float> song1Down);
		song1Queues = new List<Queue<float>>() { song1Up, song1Down, song1Left, song1Right };

		midiConverter.JsonConvert(song2, out Queue<float> song2Left, out Queue<float> song2Up, out Queue<float> song2Right, out Queue<float> song2Down);
		song2Queues = new List<Queue<float>>() { song2Up, song2Down, song2Left, song2Right };
	}

	public void SelectSong(Constants.Songs.Song song)
	{
		eventBroker.Publish(this, new AudioEvents.GetSongLength(song.ToString(), (length) => { StartCoroutine(OnSongEnd(length)); }));
		eventBroker.Publish(this, new SongEvents.PlaySong(song));
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
				inEvent.Payload.ProcessData.DynamicInvoke(song1Queues);
				break;

			case Constants.Songs.Song.Song2:
				inEvent.Payload.ProcessData.DynamicInvoke(song2Queues);
				break;
		}
	}

	private void TotalNotesHandler(BrokerEvent<ScoreEvents.TotalNotes> inEvent)
	{
		int total = 0;

		switch (inEvent.Payload.Song)
		{
			case Constants.Songs.Song.Song1:
				total += song1Queues[0].Count + song1Queues[1].Count + song1Queues[2].Count + song1Queues[3].Count;
				break;
			case Constants.Songs.Song.Song2:
				total += song2Queues[0].Count + song2Queues[1].Count + song2Queues[2].Count + song2Queues[3].Count;
				break;
		}

		inEvent.Payload.ProcessData.DynamicInvoke(total);
	}

	private void OnEnable()
	{
		eventBroker.Subscribe<SongEvents.GetSongData>(GetSongDataHandler);
		eventBroker.Subscribe<ScoreEvents.TotalNotes>(TotalNotesHandler);

		song1Button.onClick.AddListener(() => SelectSong(Constants.Songs.Song.Song1));
		song2Button.onClick.AddListener(() => SelectSong(Constants.Songs.Song.Song2));
	}

	private void OnDisable()
	{
		eventBroker.Unsubscribe<SongEvents.GetSongData>(GetSongDataHandler);
		eventBroker.Unsubscribe<ScoreEvents.TotalNotes>(TotalNotesHandler);

		song1Button.onClick.RemoveListener(() => SelectSong(Constants.Songs.Song.Song1));
		song2Button.onClick.RemoveListener(() => SelectSong(Constants.Songs.Song.Song2));
	}
}
