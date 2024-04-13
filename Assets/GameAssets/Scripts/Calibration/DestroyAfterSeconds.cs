using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField] private float seconds;
    void Start()
    {
        Invoke("HAhaestroyme", seconds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HAhaestroyme()
    {
        Destroy(this.gameObject);
    }
}
