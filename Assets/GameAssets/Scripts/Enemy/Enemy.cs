using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	private Rigidbody2D rbody;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	public bool Spawned;

    private void Awake()
    {
		rbody = GetComponent<Rigidbody2D>();
		Spawned = false;
    }

	private void Update()
	{
		if (Spawned)
		{
			transform.position = Vector2.MoveTowards(transform.position, Vector2.zero, 1f * Time.deltaTime);

			if (transform.position == Vector3.zero)
			{
				Spawned = false;
				gameObject.SetActive(false);
			}
		}
	}
}
