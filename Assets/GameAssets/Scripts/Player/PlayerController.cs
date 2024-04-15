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

	[SerializeField,Header("Animators")] Animator upperCircleAnimator;
	[SerializeField] Animator lowerCircleAnimator;
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
		animator.enabled = false;
		upperCircleAnimator.enabled = false;
		lowerCircleAnimator.enabled = false;
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

	private void Ascended(BrokerEvent<ScoreEvents.Ascended> inEvent)
	{
		bool ascended = inEvent.Payload.Ascend;
		if (ascended)
		{
			animator.SetBool("Ascended",true);
			upperCircleAnimator.SetBool("Ascended", true);
			lowerCircleAnimator.SetBool("Ascended", true);
		}
		else if (!ascended)
		{
			animator.SetBool("Ascended", false);
			upperCircleAnimator.SetBool("Ascended", false);
			lowerCircleAnimator.SetBool("Ascended", false);
		}
		else
		{
			Debug.Log("Ascended Variable Is Null");
		}
	}

	private void PlaySong(BrokerEvent<SongEvents.PlaySong> inEvent)
	{
		animator.enabled = true;
		upperCircleAnimator.enabled = true;
		lowerCircleAnimator.enabled = true;
	}
	private void SongEnded(BrokerEvent<SongEvents.SongEnded> inEvent)
	{
		animator.enabled = false;
		upperCircleAnimator.enabled = false;
		lowerCircleAnimator.enabled = false;
		animator.SetBool("Ascended", false);
		upperCircleAnimator.SetBool("Ascended", false);
		lowerCircleAnimator.SetBool("Ascended", false);
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

		eventBroker.Subscribe<ScoreEvents.Ascended>(Ascended);
		eventBroker.Subscribe<SongEvents.PlaySong>(PlaySong);
		eventBroker.Subscribe<SongEvents.SongEnded>(SongEnded);
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

		eventBroker.Unsubscribe<ScoreEvents.Ascended>(Ascended);
		eventBroker.Subscribe<SongEvents.PlaySong>(PlaySong);
		eventBroker.Subscribe<SongEvents.SongEnded>(SongEnded);
	}
}
