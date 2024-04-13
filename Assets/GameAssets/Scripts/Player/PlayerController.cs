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

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	private void Awake()
	{
		playerInputs = new PlayerInputs();
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
		Collider2D hit = Physics2D.OverlapCircle(upSweetSpot.position, Constants.Game.PlayerHitRadius);
		if (hit != null)
		{
			hit.gameObject.GetComponent<Enemy>().Hit();
		}
	}

	public void OnDown(InputAction.CallbackContext context)
	{
		Collider2D hit = Physics2D.OverlapCircle(downSweetSpot.position, Constants.Game.PlayerHitRadius);
		if (hit != null)
		{
			hit.gameObject.GetComponent<Enemy>().Hit();
		}
	}

	public void OnLeft(InputAction.CallbackContext context)
	{
		Collider2D hit = Physics2D.OverlapCircle(leftSweetSpot.position, Constants.Game.PlayerHitRadius);
		if (hit != null)
		{
			hit.gameObject.GetComponent<Enemy>().Hit();
		}
	}

	public void OnRight(InputAction.CallbackContext context)
	{
		Collider2D hit = Physics2D.OverlapCircle(rightSweetSpot.position, Constants.Game.PlayerHitRadius);
		if (hit != null)
		{
			hit.gameObject.GetComponent<Enemy>().Hit();
		}
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
