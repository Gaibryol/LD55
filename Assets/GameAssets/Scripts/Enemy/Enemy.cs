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

    private void Awake()
    {
		rbody = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		spawned = false;
    }

	public void Initialize(Vector3 _target, double _spawnTime)
	{
		target = _target;
		SpawnTime = _spawnTime;
		spawned = true;
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
			transform.position = Vector2.MoveTowards(transform.position, target, 1f * Time.deltaTime);

			if (transform.position == target)
			{
				spawned = false;
				gameObject.SetActive(false);
			}
		}
	}
}
