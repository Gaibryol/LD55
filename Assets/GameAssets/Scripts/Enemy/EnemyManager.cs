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

		eventBroker.Publish(this, new SongEvents.GetSongData(Constants.Songs.Song.TestSong1, (data) => song1Queues = data));
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

		StartCoroutine(PlaySongAudio(inEvent.Payload.Song));
	}

	private void HitNoteHandler(BrokerEvent<SongEvents.HitNote> inEvent)
	{
		Enemy enemy = inEvent.Payload.Enemy.GetComponent<Enemy>();
		double timeSinceSpawn = AudioSettings.dspTime - enemy.SpawnTime;

		// Calculate difference and factor in player input calibrated latency
		double difference = timeSinceSpawn - Constants.Songs.TimeToSweetSpot;
		difference = (difference > 0) ? difference : difference * -1;

		if (difference <= Constants.Songs.PerfectThreshold)
		{
			// Perfect
			Debug.Log("Perfect");
			eventBroker.Publish(this, new ScoreEvents.PerfectHit());
		}
		else if (difference <= Constants.Songs.OkThreshold)
		{
			// OK
			Debug.Log("OK");
			eventBroker.Publish(this, new ScoreEvents.OkayHit());
		}
		else if (difference <= Constants.Songs.BadThreshold)
		{
			// Bad
			Debug.Log("Bad");
			eventBroker.Publish(this, new ScoreEvents.BadHit());
		}
	}

	private IEnumerator PlaySongAudio(Constants.Songs.Song song)
	{
		// Add delay and player visual calibrated latency
		float delay = Constants.Songs.SongStartDelay + Constants.Songs.TimeToSweetSpot;
		yield return new WaitForSeconds(delay);

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

		//Debug.Log("Up: " + upData.Count + " / Down: " + downData.Count + " / Left: " + leftData.Count + " / Right: " + rightData.Count);

		int notesCounter = 0;
		while (notesCounter != totalNotes)
		{
			// Find the next note
			float nextUpNote = upData.Count > 0 ? upData.Peek() : -1;
			float nextDownNote = downData.Count > 0 ? downData.Peek() : -1;
			float nextLeftNote = leftData.Count > 0 ? leftData.Peek() : -1;
			float nextRightNote = rightData.Count > 0 ? rightData.Peek() : -1;

			//Debug.Log(AudioSettings.dspTime - startTime + ": " + nextUpNote + " / " + nextDownNote + " / " + nextLeftNote + " / " + nextRightNote);

			if (nextUpNote >= 0 && AudioSettings.dspTime - startTime >= nextUpNote)
			{
				GameObject enemy = GetEnemy();
				enemy.transform.position = upSpawnPosition.position;
				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(upSweetSpot.position + Constants.Game.UpExtraDistance, AudioSettings.dspTime);

				upData.Dequeue();
				totalNotes += 1;
			}
			if (nextDownNote >= 0 && AudioSettings.dspTime - startTime >= nextDownNote)
			{
				GameObject enemy = GetEnemy();
				enemy.transform.position = downSpawnPosition.position;
				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(downSweetSpot.position + Constants.Game.DownExtraDistance, AudioSettings.dspTime);

				downData.Dequeue();
				totalNotes += 1;
			}
			if (nextLeftNote >= 0 && AudioSettings.dspTime - startTime >= nextLeftNote)
			{
				GameObject enemy = GetEnemy();
				enemy.transform.position = leftSpawnPosition.position;
				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(leftSweetSpot.position + Constants.Game.LeftExtraDistance, AudioSettings.dspTime);

				leftData.Dequeue();
				totalNotes += 1;
			}
			if (nextRightNote >= 0 && AudioSettings.dspTime - startTime >= nextRightNote)
			{
				GameObject enemy = GetEnemy();
				enemy.transform.position = rightSpawnPosition.position;
				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(rightSweetSpot.position + Constants.Game.RightExtraDistance, AudioSettings.dspTime);

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
