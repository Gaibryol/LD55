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

	private float playerVisualLatency;
	private float playerInputLatency;

	private float song1BPM = 132f;
	private float beatsPerSec;
	private float songPosition;
	private float beatsPosition;
	private float totalBeats;

	private bool playing = false;

	[SerializeField] private AudioClip song1;

	// Start is called before the first frame update
	private void Start()
    {
		startTime = 0d;
		playerVisualLatency = 0f;
		playerInputLatency = 0f;

		beatsPerSec = song1BPM / 60f;

		totalBeats = song1.length * beatsPerSec;

		enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
		foreach (GameObject enemy in enemies)
		{
			enemy.SetActive(false);
		}

		eventBroker.Publish(this, new SongEvents.GetSongData(Constants.Songs.Song.Song1, (data) => song1Queues = data));
	}

	private void Update()
	{
		songPosition = (float)(AudioSettings.dspTime - startTime);
		beatsPosition = songPosition * beatsPerSec;
	}

	private void FixedUpdate()
	{
		songPosition = (float)(AudioSettings.dspTime - startTime);
		beatsPosition = songPosition * beatsPerSec;

		if (playing)
		{
			// Find the next note
			float nextUpNote = song1Queues[0].Count > 0 ? song1Queues[0].Peek() : -1;
			float nextDownNote = song1Queues[1].Count > 0 ? song1Queues[1].Peek() : -1;
			float nextLeftNote = song1Queues[2].Count > 0 ? song1Queues[2].Peek() : -1;
			float nextRightNote = song1Queues[3].Count > 0 ? song1Queues[3].Peek() : -1;

			float nextUpBeat = (nextUpNote / song1.length) * totalBeats;
			float nextDownBeat = (nextDownNote / song1.length) * totalBeats;
			float nextLeftBeat = (nextLeftNote / song1.length) * totalBeats;
			float nextRightBeat = (nextRightNote / song1.length) * totalBeats;

			//Debug.Log(AudioSettings.dspTime - startTime + ": " + nextUpNote + " / " + nextDownNote + " / " + nextLeftNote + " / " + nextRightNote);
			if (beatsPosition >= nextUpBeat - 2.2f)
			{
				GameObject enemy = GetEnemy();
				float distance = beatsPerSec * (songPosition - nextUpNote + 2.2f / beatsPerSec);

				enemy.transform.position = new Vector3(upSpawnPosition.position.x, upSpawnPosition.position.y - distance, upSpawnPosition.position.z);
				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(upSweetSpot.position, beatsPosition, beatsPerSec);

				song1Queues[0].Dequeue();
			}
			if (beatsPosition >= nextDownBeat - 2.2f)
			{
				GameObject enemy = GetEnemy();
				float distance = beatsPerSec * (songPosition - nextDownNote + 2.2f / beatsPerSec);

				enemy.transform.position = new Vector3(downSpawnPosition.position.x, downSpawnPosition.position.y + distance, downSpawnPosition.position.z);
				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(downSweetSpot.position, beatsPosition, beatsPerSec);

				song1Queues[1].Dequeue();
			}
			if (beatsPosition >= nextLeftBeat - 2.2f)
			{
				GameObject enemy = GetEnemy();
				float distance = beatsPerSec * (songPosition - nextLeftNote + 2.2f / beatsPerSec);

				enemy.transform.position = new Vector3(leftSpawnPosition.position.x + distance, leftSpawnPosition.position.y, leftSpawnPosition.position.z);
				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(leftSweetSpot.position, beatsPosition, beatsPerSec);

				song1Queues[2].Dequeue();
			}
			if (beatsPosition >= nextRightBeat - 2.2f)
			{
				GameObject enemy = GetEnemy();
				float distance = beatsPerSec * (songPosition - nextRightNote + 2.2f / beatsPerSec);

				enemy.transform.position = new Vector3(rightSpawnPosition.position.x - distance, rightSpawnPosition.position.y, rightSpawnPosition.position.z);
				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(rightSweetSpot.position, beatsPosition, beatsPerSec);

				song1Queues[3].Dequeue();
			}
		}
	}

	private void PlaySongHandler(BrokerEvent<SongEvents.PlaySong> inEvent)
	{
		playerVisualLatency = PlayerPrefs.GetFloat(Constants.Game.PlayerVisualLatency);
		playerInputLatency = PlayerPrefs.GetFloat(Constants.Game.PlayerInputLatency);

		startTime = AudioSettings.dspTime;

		switch (inEvent.Payload.Song)
		{
			case Constants.Songs.Song.Song1:
				//StartCoroutine(ParseSongData(song1Queues[0], song1Queues[1], song1Queues[2], song1Queues[3]));
				break;
		}

		StartCoroutine(PlaySongAudio(inEvent.Payload.Song));
		startTime = AudioSettings.dspTime;
		playing = true;
	}

	private void HitNoteHandler(BrokerEvent<SongEvents.HitNote> inEvent)
	{
		Enemy enemy = inEvent.Payload.Enemy.GetComponent<Enemy>();
		double beatsSinceSpawn = beatsPosition - enemy.SpawnTime;

		// Calculate difference and factor in player input latency
		double difference = beatsSinceSpawn - 3 - playerInputLatency;
		difference = (difference > 0) ? difference : difference * -1;

		if (difference <= Constants.Songs.PerfectThreshold)
		{
			// Perfect
			eventBroker.Publish(this, new ScoreEvents.PerfectHit());
		}
		else if (difference <= Constants.Songs.OkThreshold)
		{
			// OK
			eventBroker.Publish(this, new ScoreEvents.OkayHit());
		}
		else if (difference <= Constants.Songs.BadThreshold)
		{
			// Bad
			eventBroker.Publish(this, new ScoreEvents.BadHit());
		}
	}

	private IEnumerator PlaySongAudio(Constants.Songs.Song song)
	{
		// Add delay and player visual calibrated latency
		yield return new WaitForSeconds(playerVisualLatency);

		switch (song)
		{
			case Constants.Songs.Song.Song1:
				// Play song
				eventBroker.Publish(this, new AudioEvents.PlayMusic(Constants.Audio.Music.Song1));
				break;
		}

		yield return null;
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
