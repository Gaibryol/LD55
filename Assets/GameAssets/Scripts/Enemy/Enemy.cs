using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.U2D;

public class Enemy : MonoBehaviour
{
	private Rigidbody2D rbody;
	private Animator anim;
	private SplineContainer spline;
	[SerializeField] private SplineAnimate splineAnimate;

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
		splineAnimate = GetComponent<SplineAnimate>();
		spawned = false;
    }

	public void Initialize(Vector3 _target, double _spawnTime, float lookaheadBeats, SplineContainer pathToFollow, float beatOffset)
	{
		Debug.Log(beatOffset);
		


		target = _target;
		SpawnTime = _spawnTime;
		spawned = true;
		splineAnimate.Container = pathToFollow;
		splineAnimate.Restart(false);
		splineAnimate.MaxSpeed = Constants.Game.BeatsAwayToSweetSpot / lookaheadBeats;
		splineAnimate.StartOffset = beatOffset / pathToFollow.Spline.GetLength();
        splineAnimate.Play();

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

	private void FixedUpdate()
	{
		if (spawned)
		{
            if (splineAnimate.NormalizedTime == 1)
			{
                spawned = false;
                eventBroker.Publish(this, new ScoreEvents.Miss());
                gameObject.SetActive(false);
            }
			return;

            transform.position = transform.position + (direction * 3.4691f * Time.fixedDeltaTime);

			if (direction.x < 0)
			{
				// Right
				if (transform.position.x <= target.x + Constants.Game.RightExtraDistance)
				{
					spawned = false;
					eventBroker.Publish(this, new ScoreEvents.Miss());
					gameObject.SetActive(false);
				}
			}
			else if (direction.x > 0)
			{
				// Left
				if (transform.position.x >= target.x + Constants.Game.LeftExtraDistance)
				{
					spawned = false;
					eventBroker.Publish(this, new ScoreEvents.Miss());
					gameObject.SetActive(false);
				}
			}
			else if (direction.y < 0)
			{
				// Up
				if (transform.position.y <= target.y + Constants.Game.UpExtraDistance)
				{
					spawned = false;
					eventBroker.Publish(this, new ScoreEvents.Miss());
					gameObject.SetActive(false);
				}

			}
			else if (direction.y > 0)
			{
				// Down
				if (transform.position.y >= target.y + Constants.Game.DownExtraDistance)
				{
					spawned = false;
					eventBroker.Publish(this, new ScoreEvents.Miss());
					gameObject.SetActive(false);
				}
			}
		}
	}
}
