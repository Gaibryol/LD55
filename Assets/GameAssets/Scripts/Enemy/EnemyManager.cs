using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	[SerializeField] private List<GameObject> enemies = new List<GameObject>();

	[SerializeField, Header("Spawn Positions")] private Transform upSpawnPosition;
	[SerializeField] private Transform downSpawnPosition;
	[SerializeField] private Transform leftSpawnPosition;
	[SerializeField] private Transform rightSpawnPosition;

	[SerializeField, Header("Sweet Spots")] private Transform upSweetSpot;
	[SerializeField] private Transform downSweetSpot;
	[SerializeField] private Transform leftSweetSpot;
	[SerializeField] private Transform rightSweetSpot;

	[SerializeField, Header("Testing")] private MidiConverter midiConverter;
	[SerializeField] private TextAsset testJson;

	private List<Queue<float>> song1Queues;
	private List<Queue<float>> song2Queues;
	private List<Queue<float>> song3Queues;
	private double startTime;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	// Start is called before the first frame update
	private void Start()
    {
		startTime = 0d;

		enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
		foreach (GameObject enemy in enemies)
		{
			enemy.SetActive(false);
		}

		midiConverter.JsonConvert(testJson, out Queue<float> leftQueue, out Queue<float> upQueue, out Queue<float> rightQueue, out Queue<float> downQueue);
		song1Queues = new List<Queue<float>>() { upQueue, downQueue, leftQueue, rightQueue };

		startTime = AudioSettings.dspTime;
		StartCoroutine(ParseSongData(song1Queues[0], song1Queues[1], song1Queues[2], song1Queues[3]));
	}

	private void PlaySongHandler(BrokerEvent<SongEvents.PlaySong> inEvent)
	{
		switch (inEvent.Payload.Song)
		{
			case Constants.Songs.Song.TestSong1:
				StartCoroutine(ParseSongData(song1Queues[0], song1Queues[1], song1Queues[2], song1Queues[3]));
				break;

			case Constants.Songs.Song.TestSong2:
				StartCoroutine(ParseSongData(song2Queues[0], song2Queues[1], song2Queues[2], song2Queues[3]));
				break;

			case Constants.Songs.Song.TestSong3:
				StartCoroutine(ParseSongData(song3Queues[0], song3Queues[1], song3Queues[2], song3Queues[3]));
				break;
		}
	}

	private void HitNoteHandler(BrokerEvent<SongEvents.HitNote> inEvent)
	{
		float distance = 0;
		switch (inEvent.Payload.Direction)
		{
			case Constants.Game.Directions.Up:
				distance = inEvent.Payload.Enemy.transform.position.y - upSweetSpot.position.y;
				break;

			case Constants.Game.Directions.Down:
				distance = inEvent.Payload.Enemy.transform.position.y - upSweetSpot.position.y;
				break;

			case Constants.Game.Directions.Left:
				distance = inEvent.Payload.Enemy.transform.position.x - upSweetSpot.position.x;
				break;

			case Constants.Game.Directions.Right:
				distance = inEvent.Payload.Enemy.transform.position.x - upSweetSpot.position.x;
				break;
		}
	}

	private IEnumerator PlaySongAudio(Constants.Songs.Song song)
	{
		// Add TimeToPlayer delay
		yield return new WaitForSeconds(Constants.Songs.SongStartDelay);
		yield return new WaitForSeconds(Constants.Songs.TimeToPlayer);

		switch (song)
		{
			case Constants.Songs.Song.TestSong1:
				// Play song
				//eventBroker.Publish(this, new AudioEvents.PlayMusic(Constants.Audio.Music.GameTheme));
				break;

			case Constants.Songs.Song.TestSong2:
				// Play song
				//eventBroker.Publish(this, new AudioEvents.PlayMusic(Constants.Audio.Music.GameTheme));
				break;

			case Constants.Songs.Song.TestSong3:
				// Play song
				//eventBroker.Publish(this, new AudioEvents.PlayMusic(Constants.Audio.Music.GameTheme));
				break;
		}
	}

	private IEnumerator ParseSongData(Queue<float> upData, Queue<float> downData, Queue<float> leftData, Queue<float> rightData)
	{
		yield return new WaitForSeconds(Constants.Songs.SongStartDelay);

		startTime = AudioSettings.dspTime;
		int totalNotes = upData.Count + downData.Count + leftData.Count + rightData.Count;
		int notesCounter = 0;
		while (notesCounter != totalNotes)
		{
			// Find the next note
			float nextUpNote = upData.Count > 0 ? upData.Peek() : -1;
			float nextDownNote = downData.Count > 0 ? downData.Peek() : -1;
			float nextLeftNote = leftData.Count > 0 ? leftData.Peek() : -1;
			float nextRightNote = rightData.Count > 0 ? rightData.Peek() : -1;

			//Debug.Log(AudioSettings.dspTime - startTime + ": " + nextUpNote + " / " + nextDownNote + " / " + nextLeftNote + " / " + nextRightNote);

			if (nextUpNote > 0 && AudioSettings.dspTime - startTime >= nextUpNote)
			{
				GameObject enemy = GetEnemy();
				enemy.transform.position = upSpawnPosition.position;
				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(Constants.Game.Directions.Up, upSweetSpot.position);

				upData.Dequeue();
				totalNotes += 1;
			}
			if (nextDownNote > 0 && AudioSettings.dspTime - startTime >= nextDownNote)
			{
				GameObject enemy = GetEnemy();
				enemy.transform.position = downSpawnPosition.position;
				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(Constants.Game.Directions.Down, downSweetSpot.position);

				downData.Dequeue();
				totalNotes += 1;
			}
			if (nextLeftNote > 0 && AudioSettings.dspTime - startTime >= nextLeftNote)
			{
				GameObject enemy = GetEnemy();
				enemy.transform.position = leftSpawnPosition.position;
				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(Constants.Game.Directions.Left, leftSweetSpot.position);

				leftData.Dequeue();
				totalNotes += 1;
			}
			if (nextRightNote > 0 && AudioSettings.dspTime - startTime >= nextRightNote)
			{
				GameObject enemy = GetEnemy();
				enemy.transform.position = rightSpawnPosition.position;
				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(Constants.Game.Directions.Right, rightSweetSpot.position);

				rightData.Dequeue();
				totalNotes += 1;
			}
			yield return null;
		}
	}

	private GameObject GetEnemy()
	{
		for (int i = 0; i < enemies.Count; i++)
		{
			if (!enemies[i].activeSelf)
			{
				// Assign obj
				GameObject obj = enemies[i];

				// Move obj to back of the list so it should always iterate through non-active enemies first
				enemies.RemoveAt(i);
				enemies.Add(obj);

				return obj;
			}
		}

		return null;
	}

	private void OnEnable()
	{
		eventBroker.Subscribe<SongEvents.PlaySong>(PlaySongHandler);
		eventBroker.Subscribe<SongEvents.HitNote>(HitNoteHandler);
	}

	private void OnDisable()
	{
		eventBroker.Unsubscribe<SongEvents.PlaySong>(PlaySongHandler);
		eventBroker.Unsubscribe<SongEvents.HitNote>(HitNoteHandler);
	}
}
