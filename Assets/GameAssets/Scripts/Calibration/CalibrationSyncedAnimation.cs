using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class CalibrationSyncedAnimation : MonoBehaviour
{
    [SerializeField] public Vector3 startLocation;
    public float velocity;
    [SerializeField] private float halfDistance = 500f;
    [SerializeField] private float halfPeriod = 2f;
    private void Start()
    {
        startLocation = transform.position;
        velocity = halfDistance / halfPeriod;
    }
    void Update()
    {
        transform.position = startLocation + new Vector3(Triangle(-500, 500, halfPeriod * 2, 0,Time.time), 0, 0);
    }

    public float GetPosition(float t)
    {
        return halfDistance * Mathf.Sin(Mathf.PI * t / (halfPeriod * 2));
    }

    private float Triangle(float minLevel, float maxLevel, float period, float phase, float t)
    {
        float pos = Mathf.Repeat(t - phase, period) / period;

        if (pos < .5f)
        {
            return Mathf.Lerp(minLevel, maxLevel, pos * 2f);
        }
        else
        {
            return Mathf.Lerp(maxLevel, minLevel, (pos - .5f) * 2f);
        }
    }
}
