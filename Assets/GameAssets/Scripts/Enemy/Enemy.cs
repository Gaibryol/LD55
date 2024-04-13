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

    private void Awake()
    {
		rbody = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		spawned = false;
    }

	public void Initialize(Constants.Game.Directions direction, Vector3 _target)
	{
		//switch (direction)
		//{
		//	case Constants.Game.Directions.Up:
		//		anim.SetBool(Constants.Enemy.Animations.Up.Moving, true);
		//		break;

		//	case Constants.Game.Directions.Down:
		//		anim.SetBool(Constants.Enemy.Animations.Down.Moving, true);
		//		break;

		//	case Constants.Game.Directions.Left:
		//		anim.SetBool(Constants.Enemy.Animations.Left.Moving, true);
		//		break;

		//	case Constants.Game.Directions.Right:
		//		anim.SetBool(Constants.Enemy.Animations.Right.Moving, true);
		//		break;
		//}

		target = _target;
		spawned = true;
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
