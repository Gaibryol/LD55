using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[SerializeField, Header("Songs")] private MidiConverter midiConverter;
	[SerializeField] private TextAsset song1;

	[SerializeField, Header("UI")] private GameObject mainMenuPanel;
	[SerializeField] private Button song1Button;

	private List<Queue<float>> song1Queues;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	private void Awake()
	{
		midiConverter.JsonConvert(song1, out Queue<float> song1Left, out Queue<float> song1Up, out Queue<float> song1Right, out Queue<float> song1Down);
		song1Queues = new List<Queue<float>>() { song1Up, song1Down, song1Left, song1Right };
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
		}

		inEvent.Payload.ProcessData.DynamicInvoke(total);
	}

	private void OnEnable()
	{
		eventBroker.Subscribe<SongEvents.GetSongData>(GetSongDataHandler);
		eventBroker.Subscribe<ScoreEvents.TotalNotes>(TotalNotesHandler);

		song1Button.onClick.AddListener(() => SelectSong(Constants.Songs.Song.Song1));
	}

	private void OnDisable()
	{
		eventBroker.Unsubscribe<SongEvents.GetSongData>(GetSongDataHandler);
		eventBroker.Unsubscribe<ScoreEvents.TotalNotes>(TotalNotesHandler);

		song1Button.onClick.RemoveListener(() => SelectSong(Constants.Songs.Song.Song1));
	}
}
