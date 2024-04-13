using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
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
		Debug.Log("Up");
	}

	public void OnDown(InputAction.CallbackContext context)
	{
		Debug.Log("Down");
	}

	public void OnLeft(InputAction.CallbackContext context)
	{
		Debug.Log("Left");
	}

	public void OnRight(InputAction.CallbackContext context)
	{
		Debug.Log("Right");
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
