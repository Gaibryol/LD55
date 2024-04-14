using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.U2D;

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

	[SerializeField, Header("Spline")] private SplineContainer splineNorth;
	[SerializeField] private SplineContainer splineSouth;
	[SerializeField] private SplineContainer splineEast;
	[SerializeField] private SplineContainer splineWest;

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

	private Queue<float> songUpQueue;
	private Queue<float> songDownQueue;
	private Queue<float> songLeftQueue;
	private Queue<float> songRightQueue;

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

		
		Debug.Log(splineEast.Spline.GetLength());
		Debug.Log(splineWest.Spline.GetLength());
		Debug.Log(splineNorth.Spline.GetLength());
		Debug.Log(splineSouth.Spline.GetLength());
	}

	private void FixedUpdate()
	{
		songPosition = (float)(AudioSettings.dspTime - startTime);
		beatsPosition = (songPosition * beatsPerSec) - beatsPerSec;

		if (playing)
		{
			// Find the next note
			bool hasUpNote = songUpQueue.Count > 0;
			bool hasDownNote = songDownQueue.Count > 0;
			bool hasLeftNote = songLeftQueue.Count > 0;
			bool hasRightNote = songRightQueue.Count > 0;

			float nextUpNote = hasUpNote ? songUpQueue.Peek() : -1;
			float nextDownNote = hasDownNote ? songDownQueue.Peek() : -1;
			float nextLeftNote = hasLeftNote ? songLeftQueue.Peek() : -1;
			float nextRightNote = hasRightNote ? songRightQueue.Peek() : -1;

			float nextUpBeat = (nextUpNote * beatsPerSec);
			float nextDownBeat = (nextDownNote * beatsPerSec);
			float nextLeftBeat = (nextLeftNote * beatsPerSec);
			float nextRightBeat = (nextRightNote * beatsPerSec);

			float maxSpeed = Constants.Game.MetersAwayToSweetSpot / ((60 / song1BPM) * 8f);

			float lookaheadBeats = 8f;
			//Debug.Log(AudioSettings.dspTime - startTime + ": " + nextUpNote + " / " + nextDownNote + " / " + nextLeftNote + " / " + nextRightNote);
			if (hasUpNote && beatsPosition >= nextUpBeat - lookaheadBeats)
			{
				GameObject enemy = GetEnemy();
				float distance = beatsPerSec * (songPosition - nextUpNote + lookaheadBeats / beatsPerSec);

				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(upSweetSpot.position, beatsPosition, maxSpeed, splineNorth, distance);

				songUpQueue.Dequeue();
			}
			if (hasDownNote && beatsPosition >= nextDownBeat - lookaheadBeats)
			{
				GameObject enemy = GetEnemy();
				float distance = beatsPerSec * (songPosition - nextDownNote + lookaheadBeats / beatsPerSec);

				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(downSweetSpot.position, beatsPosition, maxSpeed, splineSouth, distance);

				songDownQueue.Dequeue();
			}
			if (hasLeftNote && beatsPosition >= nextLeftBeat - lookaheadBeats)
			{
				GameObject enemy = GetEnemy();
				float distance = beatsPerSec * (songPosition - nextLeftNote + lookaheadBeats / beatsPerSec);

				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(leftSweetSpot.position, beatsPosition, maxSpeed, splineWest, distance);

				songLeftQueue.Dequeue();
			}
			if (hasRightNote && beatsPosition >= nextRightBeat - lookaheadBeats)
			{
				GameObject enemy = GetEnemy();
				float distance = beatsPerSec * (songPosition - nextRightNote + lookaheadBeats / beatsPerSec);

				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(rightSweetSpot.position, beatsPosition, maxSpeed, splineEast, distance);

				songRightQueue.Dequeue();
			}
		}
	}

	private void PlaySongHandler(BrokerEvent<SongEvents.PlaySong> inEvent)
	{
		playerVisualLatency = PlayerPrefs.GetFloat(Constants.Game.PlayerVisualLatency);
		playerInputLatency = PlayerPrefs.GetFloat(Constants.Game.PlayerInputLatency);

		switch (inEvent.Payload.Song)
		{
			case Constants.Songs.Song.Song1:
				songUpQueue = new Queue<float>(song1Queues[0]);
				songDownQueue = new Queue<float>(song1Queues[1]);
				songLeftQueue = new Queue<float>(song1Queues[2]);
				songRightQueue = new Queue<float>(song1Queues[3]);
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

	private void SongEndedHandler(BrokerEvent<SongEvents.SongEnded> inEvent)
	{
		playing = false;
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
				return obj;
			}
		}

		return null;
	}

	private void OnEnable()
	{
		eventBroker.Subscribe<SongEvents.PlaySong>(PlaySongHandler);
		eventBroker.Subscribe<SongEvents.HitNote>(HitNoteHandler);
		eventBroker.Subscribe<SongEvents.SongEnded>(SongEndedHandler);
	}

	private void OnDisable()
	{
		eventBroker.Unsubscribe<SongEvents.PlaySong>(PlaySongHandler);
		eventBroker.Unsubscribe<SongEvents.HitNote>(HitNoteHandler);
		eventBroker.Unsubscribe<SongEvents.SongEnded>(SongEndedHandler);
	}
}
