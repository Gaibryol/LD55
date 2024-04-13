using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField, Header("Songs")] private MidiConverter midiConverter;
	[SerializeField] private TextAsset song1;

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
			case Constants.Songs.Song.TestSong1:
				inEvent.Payload.ProcessData.DynamicInvoke(song1Queues);
				break;

			case Constants.Songs.Song.TestSong2:
				break;

			case Constants.Songs.Song.TestSong3:
				break; 
		}
	}

	private void OnEnable()
	{
		eventBroker.Subscribe<SongEvents.GetSongData>(GetSongDataHandler);
	}

	private void OnDisable()
	{
		eventBroker.Unsubscribe<SongEvents.GetSongData>(GetSongDataHandler);
	}
}
