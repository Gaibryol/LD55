using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static ScoreEvents;

public class PlayerController : MonoBehaviour
{
  	[SerializeField, Header("Sweet Spots")] private Transform upSweetSpot;
	[SerializeField] private Transform downSweetSpot;
	[SerializeField] private Transform leftSweetSpot;
	[SerializeField] private Transform rightSweetSpot;

	private Coroutine upCoroutine;
	private Coroutine downCoroutine;
    private Coroutine rightCoroutine;
    private Coroutine leftCoroutine;

    private PlayerInputs playerInputs;
	private InputAction up;
	private InputAction down;
	private InputAction left;
	private InputAction right;

	private SpriteRenderer spriteRenderer;

	[SerializeField,Header("Animators")] Animator upperCircleAnimator;
	[SerializeField] Animator lowerCircleAnimator;
	[SerializeField] Animator fullCircleAnimator;
	[SerializeField] private List<Sprite> summonItems;
	[SerializeField] private SpriteRenderer summonItemSprite;
	private Animator animator;

	[SerializeField, Header("Background")] private SpriteRenderer defaultBackground;
	[SerializeField] private SpriteRenderer ascendBackground;


	private Coroutine defaultCoroutine;
	private Coroutine ascendCoroutine;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	private void Awake()
	{
		playerInputs = new PlayerInputs();
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	// Start is called before the first frame update
	private void Start()
    {
		animator.enabled = false;
		upperCircleAnimator.enabled = false;
		lowerCircleAnimator.enabled = false;

		defaultCoroutine = null;
		ascendCoroutine = null;
	}

    // Update is called once per frame
    private void Update()
    {

    }

	private IEnumerator HideSweetSpot(SpriteRenderer sweetSpot)
	{
		yield return new WaitForSeconds(0.1f);
		sweetSpot.enabled = false;
	}

	public void OnUp(InputAction.CallbackContext context)
	{
		// Find note furthest along path
		upSweetSpot.GetComponent<SpriteRenderer>().enabled = true;
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
		if (upCoroutine != null)
		{
			StopCoroutine(upCoroutine);
		}
		upCoroutine = StartCoroutine(HideSweetSpot(upSweetSpot.GetComponent<SpriteRenderer>()));
	}

	public void OnDown(InputAction.CallbackContext context)
	{
		// Find note furthest along path
		downSweetSpot.GetComponent<SpriteRenderer>().enabled = true;

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
        if (downCoroutine != null)
        {
            StopCoroutine(downCoroutine);
        }
        downCoroutine = StartCoroutine(HideSweetSpot(downSweetSpot.GetComponent<SpriteRenderer>()));
    }

    public void OnLeft(InputAction.CallbackContext context)
	{
        leftSweetSpot.GetComponent<SpriteRenderer>().enabled = true;

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
        if (leftCoroutine != null)
        {
            StopCoroutine(leftCoroutine);
        }
        leftCoroutine = StartCoroutine(HideSweetSpot(leftSweetSpot.GetComponent<SpriteRenderer>()));
    }

    public void OnRight(InputAction.CallbackContext context)
	{
        rightSweetSpot.GetComponent<SpriteRenderer>().enabled = true;

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
        if (rightCoroutine != null)
        {
            StopCoroutine(rightCoroutine);
        }
        rightCoroutine = StartCoroutine(HideSweetSpot(rightSweetSpot.GetComponent<SpriteRenderer>()));
    }

	private void Ascended(BrokerEvent<ScoreEvents.Ascended> inEvent)
	{
		bool ascended = inEvent.Payload.Ascend;
		if (ascended)
		{
			animator.SetBool("Ascended",true);
			upperCircleAnimator.SetBool("Ascended", true);
			lowerCircleAnimator.SetBool("Ascended", true);

			if (ascendCoroutine != null)
			{
				StopCoroutine(ascendCoroutine);
			}
			if (defaultCoroutine != null)
			{
				StopCoroutine(defaultCoroutine);
			}

			// Lerp to default
			ascendCoroutine = StartCoroutine(LerpToAscend());
		}
		else if (!ascended)
		{
			animator.SetBool("Ascended", false);
			upperCircleAnimator.SetBool("Ascended", false);
			lowerCircleAnimator.SetBool("Ascended", false);

			if (ascendCoroutine != null)
			{
				StopCoroutine(ascendCoroutine);
			}
			if (defaultCoroutine != null)
			{
				StopCoroutine(defaultCoroutine);
			}

			// Lerp to background
			StartCoroutine(LerpToDefault());
		}
		else
		{
			Debug.Log("Ascended Variable Is Null");
		}
	}

	private IEnumerator LerpToDefault()
	{
		while (defaultBackground.color.a < 1)
		{
			defaultBackground.color = new Color(defaultBackground.color.r, defaultBackground.color.g, defaultBackground.color.b, defaultBackground.color.a + Time.deltaTime/2f);
			ascendBackground.color = new Color(ascendBackground.color.r, ascendBackground.color.g, ascendBackground.color.b, ascendBackground.color.a - Time.deltaTime/2f);
			yield return null;
		}
	}

	private IEnumerator LerpToAscend()
	{
		while (ascendBackground.color.a < 1)
		{
			defaultBackground.color = new Color(defaultBackground.color.r, defaultBackground.color.g, defaultBackground.color.b, defaultBackground.color.a - Time.deltaTime/2f);
			ascendBackground.color = new Color(ascendBackground.color.r, ascendBackground.color.g, ascendBackground.color.b, ascendBackground.color.a + Time.deltaTime/2f);
			yield return null;
		}
	}

	private void PlaySong(BrokerEvent<SongEvents.PlaySong> inEvent)
	{
		fullCircleAnimator.SetBool("Summon", false);
		spriteRenderer.enabled = true;

		animator.enabled = true;
		upperCircleAnimator.enabled = true;
		lowerCircleAnimator.enabled = true;

		defaultBackground.color = new Color(defaultBackground.color.r, defaultBackground.color.g, defaultBackground.color.b, 1f);
		ascendBackground.color = new Color(ascendBackground.color.r, ascendBackground.color.g, ascendBackground.color.b, 0f);
	}
	private void SongEnded(BrokerEvent<SongEvents.SongEnded> inEvent)
	{
		//spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);

		if (inEvent.Payload.Success)
		{
			summonItemSprite.sprite = summonItems[UnityEngine.Random.Range(0, summonItems.Count)];
            fullCircleAnimator.SetBool("Summon", true);

        }

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
		eventBroker.Unsubscribe<SongEvents.PlaySong>(PlaySong);
		eventBroker.Unsubscribe<SongEvents.SongEnded>(SongEnded);

    }
}
