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

	private List<Queue<float>> song1NormalQueues;
	private List<Queue<float>> song1HardQueues;
	private List<Queue<float>> song2NormalQueues;
	private List<Queue<float>> song2HardQueues;
	private List<Queue<float>> song3NormalQueues;
	private List<Queue<float>> song3HardQueues;
	private double startTime;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	private float playerVisualLatency;
	private float playerInputLatency;

	private float song1BPM = 132f;
	private float song1Length;

	private float song2BPM = 120f;
	private float song2Length;

	private float song3BPM = 169f;
	private float song3Length;

	private float bpm;
	private float beatsPerSec;
	private float songPosition;
	private float beatsPosition;
	private float totalBeats;

	private bool playing = false;

	private Queue<float> songUpQueue;
	private Queue<float> songDownQueue;
	private Queue<float> songLeftQueue;
	private Queue<float> songRightQueue;

	// Start is called before the first frame update
	private void Start()
    {
		startTime = 0d;
		playerVisualLatency = 0f;
		playerInputLatency = 0f;

		enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
		foreach (GameObject enemy in enemies)
		{
			enemy.SetActive(false);
		}

		eventBroker.Publish(this, new SongEvents.GetSongData(Constants.Songs.Song.Song1, Constants.Songs.Difficulties.Normal, (data) => song1NormalQueues = data));
		eventBroker.Publish(this, new SongEvents.GetSongData(Constants.Songs.Song.Song1, Constants.Songs.Difficulties.Hard, (data) => song1HardQueues = data));
		eventBroker.Publish(this, new AudioEvents.GetSongLength(Constants.Songs.Song.Song1.ToString(), (data) => song1Length = data));

		eventBroker.Publish(this, new SongEvents.GetSongData(Constants.Songs.Song.Song2, Constants.Songs.Difficulties.Normal, (data) => song2NormalQueues = data));
		eventBroker.Publish(this, new SongEvents.GetSongData(Constants.Songs.Song.Song2, Constants.Songs.Difficulties.Hard, (data) => song2HardQueues = data));
		eventBroker.Publish(this, new AudioEvents.GetSongLength(Constants.Songs.Song.Song2.ToString(), (data) => song2Length = data));

		eventBroker.Publish(this, new SongEvents.GetSongData(Constants.Songs.Song.Song3, Constants.Songs.Difficulties.Normal, (data) => song3NormalQueues = data));
		eventBroker.Publish(this, new SongEvents.GetSongData(Constants.Songs.Song.Song3, Constants.Songs.Difficulties.Hard, (data) => song3HardQueues = data));
		eventBroker.Publish(this, new AudioEvents.GetSongLength(Constants.Songs.Song.Song3.ToString(), (data) => song3Length = data));
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

			float maxSpeed = Constants.Game.MetersAwayToSweetSpot / ((60 / bpm) * 8f);

			float lookaheadBeats = 8f;
			if (hasUpNote && beatsPosition >= nextUpBeat - lookaheadBeats)
			{
				GameObject enemy = GetEnemy();
				float distance = beatsPerSec * (songPosition - nextUpNote + lookaheadBeats / beatsPerSec);

				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(upSweetSpot.position, beatsPosition, maxSpeed, splineNorth, distance, Constants.Game.Directions.Up);

				songUpQueue.Dequeue();
			}
			if (hasDownNote && beatsPosition >= nextDownBeat - lookaheadBeats)
			{
				GameObject enemy = GetEnemy();
				float distance = beatsPerSec * (songPosition - nextDownNote + lookaheadBeats / beatsPerSec);

				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(downSweetSpot.position, beatsPosition, maxSpeed, splineSouth, distance, Constants.Game.Directions.Down);

				songDownQueue.Dequeue();
			}
			if (hasLeftNote && beatsPosition >= nextLeftBeat - lookaheadBeats)
			{
				GameObject enemy = GetEnemy();
				float distance = beatsPerSec * (songPosition - nextLeftNote + lookaheadBeats / beatsPerSec);

				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(leftSweetSpot.position, beatsPosition, maxSpeed, splineWest, distance, Constants.Game.Directions.Left);

				songLeftQueue.Dequeue();
			}
			if (hasRightNote && beatsPosition >= nextRightBeat - lookaheadBeats)
			{
				GameObject enemy = GetEnemy();
				float distance = beatsPerSec * (songPosition - nextRightNote + lookaheadBeats / beatsPerSec);

				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(rightSweetSpot.position, beatsPosition, maxSpeed, splineEast, distance, Constants.Game.Directions.Right);

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

				if (inEvent.Payload.Difficulty == Constants.Songs.Difficulties.Normal)
				{
					songUpQueue = new Queue<float>(song1NormalQueues[0]);
					songDownQueue = new Queue<float>(song1NormalQueues[1]);
					songLeftQueue = new Queue<float>(song1NormalQueues[2]);
					songRightQueue = new Queue<float>(song1NormalQueues[3]);
				}
				else if (inEvent.Payload.Difficulty == Constants.Songs.Difficulties.Hard)
				{
					songUpQueue = new Queue<float>(song1HardQueues[0]);
					songDownQueue = new Queue<float>(song1HardQueues[1]);
					songLeftQueue = new Queue<float>(song1HardQueues[2]);
					songRightQueue = new Queue<float>(song1HardQueues[3]);
				}

				beatsPerSec = song1BPM / 60f;
				totalBeats = song1Length * beatsPerSec;
				bpm = song1BPM;
				break;

			case Constants.Songs.Song.Song2:
				if (inEvent.Payload.Difficulty == Constants.Songs.Difficulties.Normal)
				{
					songUpQueue = new Queue<float>(song2NormalQueues[0]);
					songDownQueue = new Queue<float>(song2NormalQueues[1]);
					songLeftQueue = new Queue<float>(song2NormalQueues[2]);
					songRightQueue = new Queue<float>(song2NormalQueues[3]);
				}
				else if (inEvent.Payload.Difficulty == Constants.Songs.Difficulties.Hard)
				{
					songUpQueue = new Queue<float>(song2HardQueues[0]);
					songDownQueue = new Queue<float>(song2HardQueues[1]);
					songLeftQueue = new Queue<float>(song2HardQueues[2]);
					songRightQueue = new Queue<float>(song2HardQueues[3]);
				}

				beatsPerSec = song2BPM / 60f;
				totalBeats = song2Length * beatsPerSec;
				bpm = song1BPM;
				break;

			case Constants.Songs.Song.Song3:
				if (inEvent.Payload.Difficulty == Constants.Songs.Difficulties.Normal)
				{
					songUpQueue = new Queue<float>(song3NormalQueues[0]);
					songDownQueue = new Queue<float>(song3NormalQueues[1]);
					songLeftQueue = new Queue<float>(song3NormalQueues[2]);
					songRightQueue = new Queue<float>(song3NormalQueues[3]);
				}
				else if (inEvent.Payload.Difficulty == Constants.Songs.Difficulties.Hard)
				{
					songUpQueue = new Queue<float>(song3HardQueues[0]);
					songDownQueue = new Queue<float>(song3HardQueues[1]);
					songLeftQueue = new Queue<float>(song3HardQueues[2]);
					songRightQueue = new Queue<float>(song3HardQueues[3]);
				}

				beatsPerSec = song3BPM / 60f;
				totalBeats = song3Length * beatsPerSec;
				bpm = song1BPM;
				break;
		}

		StartCoroutine(PlaySongAudio(inEvent.Payload.Song));
		startTime = AudioSettings.dspTime;
		playing = true;
	}

	private void HitNoteHandler(BrokerEvent<SongEvents.HitNote> inEvent)
	{
		Vector3 notePosition = inEvent.Payload.Enemy.transform.position;
		float difference = 0f;

		switch (inEvent.Payload.Direction)
		{
			case Constants.Game.Directions.Up:
				difference = Vector3.Distance(notePosition, upSweetSpot.position);
				break;

			case Constants.Game.Directions.Down:
				difference = Vector3.Distance(notePosition, downSweetSpot.position);
				break;

			case Constants.Game.Directions.Left:
				difference = Vector3.Distance(notePosition, leftSweetSpot.position);
				break;

			case Constants.Game.Directions.Right:
				difference = Vector3.Distance(notePosition, rightSweetSpot.position);
				break;
		}

		difference -= playerInputLatency;
		difference = difference > 0 ? difference : difference * -1;

		if (difference <= Constants.Songs.PerfectThreshold + playerInputLatency)
		{
			// Perfect
			inEvent.Payload.Enemy.GetComponent<Enemy>().PlayHitAnimation("Perfect");
			eventBroker.Publish(this, new ScoreEvents.PerfectHit());
		}
		else if (difference <= Constants.Songs.OkThreshold + playerInputLatency)
		{
            // OK
            inEvent.Payload.Enemy.GetComponent<Enemy>().PlayHitAnimation("Okay");
            eventBroker.Publish(this, new ScoreEvents.OkayHit());
		}
		else
		{
            // Bad
            inEvent.Payload.Enemy.GetComponent<Enemy>().PlayHitAnimation("Bad");
            eventBroker.Publish(this, new ScoreEvents.BadHit());
		}
	}

	private void SongEndedHandler(BrokerEvent<SongEvents.SongEnded> inEvent)
	{
		playing = false;

		foreach (GameObject enemy in enemies)
		{
			enemy.SetActive(false);
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

			case Constants.Songs.Song.Song2:
				eventBroker.Publish(this, new AudioEvents.PlayMusic(Constants.Audio.Music.Song2));
				break;

			case Constants.Songs.Song.Song3:
				eventBroker.Publish(this, new AudioEvents.PlayMusic(Constants.Audio.Music.Song3));
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
