using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OnUp()
	{
		Debug.Log("Up");
	}

	public void OnDown()
	{
		Debug.Log("Down");
	}

	public void OnLeft()
	{
		Debug.Log("Left");
	}

	public void OnRight()
	{
		Debug.Log("Right");
	}	
}
