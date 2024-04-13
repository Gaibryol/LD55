using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibration : MonoBehaviour
{
    public static Calibration Instance;

    [SerializeField] private AudioSource audioSource;

    [SerializeField] public float bpm;
    [SerializeField] public float secPerBeat;
    [SerializeField] public float calibrationPosition;
    [SerializeField] public float calibrationPositionInBeats;
    [SerializeField] private float firstBeatOffset = 0;
    [SerializeField] private float dspCalibrationTime;

    [SerializeField] private float beatsPerLoop;

    [SerializeField] private int completedLoops = 0;
    [SerializeField] private float loopPositionInBeats;

    [SerializeField] public float loopPositionInAnalog;

    public static float visualCalibrationOffset = 0f;
    public static float inputCalibrationOffset = 0f;

    public float currentBeat;
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        secPerBeat = 60f / bpm;

        Invoke("StartTrack", 2f);
    }

    private void StartTrack()
    {
        dspCalibrationTime = (float)AudioSettings.dspTime;

        audioSource.Play();
    }

    void FixedUpdate()
    {
        if (!audioSource.isPlaying) return;


        calibrationPosition = (float)(AudioSettings.dspTime - dspCalibrationTime - firstBeatOffset + visualCalibrationOffset + inputCalibrationOffset);

        calibrationPositionInBeats = calibrationPosition / secPerBeat;
        currentBeat = calibrationPositionInBeats % 4;

        if (calibrationPositionInBeats >= (completedLoops + 1) * beatsPerLoop)
        {
            completedLoops++;
        }
        loopPositionInBeats = calibrationPositionInBeats - completedLoops * beatsPerLoop;
        loopPositionInAnalog = loopPositionInBeats / beatsPerLoop;
    }
}
