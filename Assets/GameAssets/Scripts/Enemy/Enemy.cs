using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.U2D;
using static ScoreEvents;

public class Enemy : MonoBehaviour
{
	private Animator anim;
	private Collider2D coll;
	[SerializeField] private SplineAnimate splineAnimate;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	private bool spawned;
	private Vector3 target;
	public double SpawnTime;

	private float BPS;

	private Constants.Game.Directions Direction;

    private void Awake()
    {
		anim = GetComponent<Animator>();
		coll = GetComponent<Collider2D>();
		splineAnimate = GetComponent<SplineAnimate>();
		spawned = false;
    }

	public void Initialize(Vector3 _target, double _spawnTime, float maxSpeed, SplineContainer pathToFollow, float beatOffset, Constants.Game.Directions direction)
	{
		CancelInvoke();
		target = _target;
		SpawnTime = _spawnTime;
		spawned = true;
		splineAnimate.Container = pathToFollow;
		splineAnimate.Restart(false);
		splineAnimate.MaxSpeed = maxSpeed;
		splineAnimate.NormalizedTime = beatOffset / pathToFollow.Spline.GetLength();
		splineAnimate.Play();
		coll.enabled = true;
		Direction = direction;
		anim.SetInteger("Note", (int)Direction);
	}

	public void Hit()
	{
		// Stop movement and send logic for scoring
		spawned = false;
        splineAnimate.Pause();

        eventBroker.Publish(this, new SongEvents.HitNote(Direction, gameObject));
		coll.enabled = false;
		// Play animation
		// anim.SetBool("Hit", true);
		// gameObject.SetActive(false);
	}

	public void PlayHitAnimation(string name)
	{
		spawned = false;
		anim.SetTrigger(name);
        Invoke("setActiveFalse", .3f);

    }

    private void setActiveFalse()
    {
		anim.ResetTrigger("Miss");
        anim.SetInteger("Note", -1);

        gameObject.SetActive(false);
    }

	private void HandleMiss()
	{
        spawned = false;
		coll.enabled = false;
        eventBroker.Publish(this, new ScoreEvents.Miss(Direction));
        anim.SetTrigger("Miss");
        splineAnimate.Pause();
        Invoke("setActiveFalse", .3f);
    }

    private void FixedUpdate()
	{
		transform.rotation = Quaternion.identity;
		if (spawned)
		{
            if (splineAnimate.NormalizedTime >= .9)
			{
				HandleMiss();
            }
			else if (!splineAnimate.IsPlaying)
			{
				//Debug.Log("I'm not playing!");
				splineAnimate.Play();
			}

			switch (Direction)
			{
				case Constants.Game.Directions.Up:
					if (transform.position.y <= target.y + Constants.Game.UpExtraDistance)
					{
						HandleMiss();
					}
					break;

				case Constants.Game.Directions.Down:
					if (transform.position.y >= target.y + Constants.Game.DownExtraDistance)
					{
                        HandleMiss();
                    }
                    break;

				case Constants.Game.Directions.Left:
					if (transform.position.x >= target.x + Constants.Game.LeftExtraDistance)
					{
                        HandleMiss();
                    }
                    break;

				case Constants.Game.Directions.Right:
					if (transform.position.x <= target.x + Constants.Game.RightExtraDistance)
					{
                        HandleMiss();
                    }
                    break;
			}
		}
	}
}
