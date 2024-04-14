using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
  	[SerializeField, Header("Sweet Spots")] private Transform upSweetSpot;
	[SerializeField] private Transform downSweetSpot;
	[SerializeField] private Transform leftSweetSpot;
	[SerializeField] private Transform rightSweetSpot;

	private PlayerInputs playerInputs;
	private InputAction up;
	private InputAction down;
	private InputAction left;
	private InputAction right;
	private Animator animator;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	private void Awake()
	{
		playerInputs = new PlayerInputs();
		animator = GetComponent<Animator>();
	}

	// Start is called before the first frame update
	private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {

    }

	public void OnUp(InputAction.CallbackContext context)
	{
		// Find note furthest along path
		Collider2D[] hits = Physics2D.OverlapCircleAll(upSweetSpot.position, Constants.Game.PlayerHitRadius);
		if (hits.Length > 0)
		{
			Collider2D furthest = hits[0];
			foreach (Collider2D hit in hits)
			{
				if (hit.transform.position.y < furthest.transform.position.y)
				{
					furthest = hit;
				}
			}

			furthest.GetComponent<Enemy>().Hit();
		}
		animator.SetBool("isPlaying", true);
	}

	public void OnDown(InputAction.CallbackContext context)
	{
		// Find note furthest along path
		
		Collider2D[] hits = Physics2D.OverlapCircleAll(downSweetSpot.position, Constants.Game.PlayerHitRadius);
		if (hits.Length > 0)
		{
			Collider2D furthest = hits[0];
			foreach (Collider2D hit in hits)
			{
				if (hit.transform.position.y > furthest.transform.position.y)
				{
					furthest = hit;
				}
			}

			furthest.GetComponent<Enemy>().Hit();
		}

        animator.SetBool("isPlaying", true);
	}

    public void OnLeft(InputAction.CallbackContext context)
	{
		// Find note furthest along path
		Collider2D[] hits = Physics2D.OverlapCircleAll(leftSweetSpot.position, Constants.Game.PlayerHitRadius);
		if (hits.Length > 0)
		{
			Collider2D furthest = hits[0];
			foreach (Collider2D hit in hits)
			{
				if (hit.transform.position.x > furthest.transform.position.x)
				{
					furthest = hit;
				}
			}

			furthest.GetComponent<Enemy>().Hit();
		}

        animator.SetBool("isPlaying", true);
	}

    public void OnRight(InputAction.CallbackContext context)
	{
		// Find note furthest along path
		Collider2D[] hits = Physics2D.OverlapCircleAll(rightSweetSpot.position, Constants.Game.PlayerHitRadius);
		if (hits.Length > 0)
		{
			Collider2D furthest = hits[0];
			foreach (Collider2D hit in hits)
			{
				if (hit.transform.position.x < furthest.transform.position.x)
				{
					furthest = hit;
				}
			}

			furthest.GetComponent<Enemy>().Hit();
		}

        animator.SetBool("isPlaying", true);
	}

    private void OnEnable()
	{
		up = playerInputs.Player.Up;
		up.performed += OnUp;
		up.Enable();

		down = playerInputs.Player.Down;
		down.performed += OnDown;
		down.Enable();

		left = playerInputs.Player.Left;
		left.performed += OnLeft;
		left.Enable();

		right = playerInputs.Player.Right;
		right.performed += OnRight;
		right.Enable();
	}

	private void OnDisable()
	{
		up.performed -= OnUp;
		up.Disable();

		down.performed -= OnDown;
		down.Disable();

		left.performed -= OnLeft;
		left.Disable();

		right.performed -= OnRight;
		right.Disable();
	}
}
