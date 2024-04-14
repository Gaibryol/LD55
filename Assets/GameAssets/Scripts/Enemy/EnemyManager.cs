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

	private bool test = false;

	[SerializeField] private AudioClip song1;

	// Start is called before the first frame update
	private void Start()
    {
		startTime = 0d;
		playerVisualLatency = 0f;
		playerInputLatency = 0f;

		beatsPerSec = song1BPM / 60f;

		totalBeats = song1.length * beatsPerSec;
		//Debug.Log("total beats: " + totalBeats);
		//Debug.Log("len: " + song1.length);
		//Debug.Log("beats per sec: " + beatsPerSec);
		//Debug.Log("BPM: " + song1BPM);

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
		eventBroker.Publish(this, new AudioEvents.PlayMusic(Constants.Audio.Music.Song1));
		startTime = AudioSettings.dspTime;
		test = true;
	}

	private void HitNoteHandler(BrokerEvent<SongEvents.HitNote> inEvent)
	{
		Enemy enemy = inEvent.Payload.Enemy.GetComponent<Enemy>();
		double timeSinceSpawn = AudioSettings.dspTime - enemy.SpawnTime;

		// Calculate difference and factor in player input latency
		double difference = timeSinceSpawn - Constants.Songs.TimeToSweetSpot - playerInputLatency;
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
		//float delay = Constants.Songs.SongStartDelay + Constants.Songs.TimeToSweetSpot - playerVisualLatency;
		//yield return new WaitForSeconds(delay);

		//switch (song)
		//{
		//	case Constants.Songs.Song.Song1:
		//		// Play song
		//		eventBroker.Publish(this, new AudioEvents.PlayMusic(Constants.Audio.Music.Song1));
		//		break;
		//}

		yield return null;
	}

	private void FixedUpdate()
	{
		songPosition = (float)(AudioSettings.dspTime - startTime);
		beatsPosition = songPosition * beatsPerSec;

		if (test)
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
			if (beatsPosition >= nextUpBeat - 3)
			{
				GameObject enemy = GetEnemy();
				// b = 
				float distance = beatsPerSec * (songPosition - nextUpNote + 3/beatsPerSec);
				Debug.Log("distance: " + distance);
				enemy.transform.position = new Vector3(upSpawnPosition.position.x, upSpawnPosition.position.y - distance, upSpawnPosition.position.z);
				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(upSweetSpot.position + Constants.Game.UpExtraDistance, AudioSettings.dspTime, beatsPerSec);

				song1Queues[0].Dequeue();
			}
			if (beatsPosition >= nextDownBeat - 3)
			{
				GameObject enemy = GetEnemy();
				float distance = beatsPerSec * (songPosition - nextDownNote + 3 / beatsPerSec);

                enemy.transform.position = new Vector3(downSpawnPosition.position.x, downSpawnPosition.position.y + distance, downSpawnPosition.position.z);
				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(downSweetSpot.position + Constants.Game.DownExtraDistance, AudioSettings.dspTime, beatsPerSec);

				song1Queues[1].Dequeue();
			}
			if (beatsPosition >= nextLeftBeat - 3)
			{
				GameObject enemy = GetEnemy();
				float distance = beatsPerSec * (songPosition - nextLeftNote + 3 / beatsPerSec);

                enemy.transform.position = new Vector3(leftSpawnPosition.position.x + distance, leftSpawnPosition.position.y, leftSpawnPosition.position.z);
				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(leftSweetSpot.position + Constants.Game.LeftExtraDistance, AudioSettings.dspTime, beatsPerSec);

				song1Queues[2].Dequeue();
			}
			if (beatsPosition >= nextRightBeat - 3)
			{
				GameObject enemy = GetEnemy();
				float distance = beatsPerSec * (songPosition - nextRightNote + 3/beatsPerSec);

				enemy.transform.position = new Vector3(rightSpawnPosition.position.x - distance, rightSpawnPosition.position.y, rightSpawnPosition.position.z);
				enemy.SetActive(true);
				enemy.GetComponent<Enemy>().Initialize(rightSweetSpot.position + Constants.Game.RightExtraDistance, AudioSettings.dspTime, beatsPerSec);

				song1Queues[3].Dequeue();
			}
		}
		
		//if (beatsPosition <= nextDownBeat)
		//{
		//	GameObject enemy = GetEnemy();
		//	float distance = beatsPerSec * (beatsPosition - nextDownBeat);
		//	enemy.transform.position = new Vector3(downSpawnPosition.position.x, downSpawnPosition.position.y + distance, downSpawnPosition.position.z);
		//	enemy.SetActive(true);
		//	enemy.GetComponent<Enemy>().Initialize(downSweetSpot.position + Constants.Game.DownExtraDistance, AudioSettings.dspTime, beatsPerSec);

		//	downData.Dequeue();
		//	totalNotes += 1;
		//}
		//if (beatsPosition <= nextLeftBeat)
		//{
		//	GameObject enemy = GetEnemy();
		//	float distance = beatsPerSec * (beatsPosition - nextLeftBeat);
		//	enemy.transform.position = new Vector3(leftSpawnPosition.position.x + distance, leftSpawnPosition.position.y, leftSpawnPosition.position.z);
		//	enemy.SetActive(true);
		//	enemy.GetComponent<Enemy>().Initialize(leftSweetSpot.position + Constants.Game.LeftExtraDistance, AudioSettings.dspTime, beatsPerSec);

		//	leftData.Dequeue();
		//	totalNotes += 1;
		//}
		//if (beatsPosition <= nextRightBeat)
		//{
		//	GameObject enemy = GetEnemy();
		//	float distance = beatsPerSec * (beatsPosition - nextRightBeat);
		//	enemy.transform.position = new Vector3(rightSpawnPosition.position.x + distance, rightSpawnPosition.position.y, rightSpawnPosition.position.z);
		//	enemy.SetActive(true);
		//	enemy.GetComponent<Enemy>().Initialize(rightSweetSpot.position + Constants.Game.RightExtraDistance, AudioSettings.dspTime, beatsPerSec);

		//	rightData.Dequeue();
		//	totalNotes += 1;
		//}
	}

	//private IEnumerator ParseSongData(Queue<float> upData, Queue<float> downData, Queue<float> leftData, Queue<float> rightData)
	//{
	//	//yield return new WaitForSeconds(Constants.Songs.SongStartDelay);

	//	//startTime = AudioSettings.dspTime;

	//	eventBroker.Publish(this, new AudioEvents.PlayMusic(Constants.Audio.Music.Song1));

	//	startTime = AudioSettings.dspTime;
	//	songPosition = (float)(AudioSettings.dspTime - startTime);
	//	beatsPosition = songPosition / beatsPerSec;



	//	//Debug.Log("Up: " + upData.Count + " / Down: " + downData.Count + " / Left: " + leftData.Count + " / Right: " + rightData.Count);

		
	//}

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
