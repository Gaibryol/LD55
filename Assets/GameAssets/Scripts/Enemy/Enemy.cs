using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	private Rigidbody2D rbody;
	private Animator anim;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	private bool spawned;
	private Vector3 target;
	public double SpawnTime;

	private float BPS;

	private Vector3 direction;

    private void Awake()
    {
		rbody = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		spawned = false;
    }

	public void Initialize(Vector3 _target, double _spawnTime, float bps)
	{
		target = _target;
		SpawnTime = _spawnTime;
		spawned = true;
		BPS = bps;

		direction = (target - transform.position).normalized;
	}

	public void Hit()
	{
		// Stop movement and send logic for scoring
		spawned = false;
		eventBroker.Publish(this, new SongEvents.HitNote(gameObject));

		// Play animation
		// anim.SetBool("Hit", true);
		gameObject.SetActive(false);
	}

	private void Update()
	{
		if (spawned)
		{
			transform.position = transform.position + direction * BPS * Time.deltaTime;

			if (direction.x < 0)
			{
				// Right
				if (transform.position.x <= target.x)
				{
					spawned = false;
					eventBroker.Publish(this, new ScoreEvents.Miss());
					gameObject.SetActive(false);
				}
			}
			else if (direction.x > 0)
			{
				// Left
				if (transform.position.x >= target.x)
				{
					spawned = false;
					eventBroker.Publish(this, new ScoreEvents.Miss());
					gameObject.SetActive(false);
				}
			}
			else if (direction.y < 0)
			{
				// Up
				if (transform.position.y <= target.y)
				{
					spawned = false;
					eventBroker.Publish(this, new ScoreEvents.Miss());
					gameObject.SetActive(false);
				}

			}
			else if (direction.y > 0)
			{
				// Down
				if (transform.position.y >= target.y)
				{
					spawned = false;
					eventBroker.Publish(this, new ScoreEvents.Miss());
					gameObject.SetActive(false);
				}
			}
		}
	}
}
