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
		startTime = AudioSettings.dspTime;

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

	private IEnumerator PlaySongAudio(Constants.Songs.Song song)
	{
		// Add TimeToPlayer delay
		yield return new WaitForSeconds(Constants.Songs.TimeToPlayer);

		switch (song)
		{
			case Constants.Songs.Song.TestSong1:
				// Play song
				break;

			case Constants.Songs.Song.TestSong2:
				// Play song
				break;

			case Constants.Songs.Song.TestSong3:
				// Play song
				break;
		}
	}

	private IEnumerator ParseSongData(Queue<float> upData, Queue<float> downData, Queue<float> leftData, Queue<float> rightData)
	{
		startTime = AudioSettings.dspTime;
		int totalNotes = upData.Count + downData.Count + leftData.Count + rightData.Count;
		int notesCounter = 0;
		while (notesCounter != totalNotes)
		{
			// Find the next note
			float nextUpNote = upData.Peek();
			float nextDownNote = downData.Peek();
			float nextLeftNote = leftData.Peek();
			float nextRightNote = rightData.Peek();

			//Debug.Log(AudioSettings.dspTime - startTime + ": " + nextUpNote + " / " + nextDownNote + " / " + nextLeftNote + " / " + nextRightNote);

			if (AudioSettings.dspTime - startTime >= nextUpNote)
			{
				GameObject enemy = GetEnemy();
				enemy.transform.position = upSpawnPosition.position;
				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Spawned = true;

				upData.Dequeue();
				totalNotes += 1;
			}
			if (AudioSettings.dspTime - startTime >= nextDownNote)
			{
				GameObject enemy = GetEnemy();
				enemy.transform.position = downSpawnPosition.position;
				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Spawned = true;

				downData.Dequeue();
				totalNotes += 1;
			}
			if (AudioSettings.dspTime - startTime >= nextLeftNote)
			{
				GameObject enemy = GetEnemy();
				enemy.transform.position = leftSpawnPosition.position;
				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Spawned = true;

				leftData.Dequeue();
				totalNotes += 1;
			}
			if (AudioSettings.dspTime - startTime >= nextRightNote)
			{
				GameObject enemy = GetEnemy();
				enemy.transform.position = rightSpawnPosition.position;
				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Spawned = true;

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
	}

	private void OnDisable()
	{
		eventBroker.Unsubscribe<SongEvents.PlaySong>(PlaySongHandler);
	}
}
