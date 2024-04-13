using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibration : MonoBehaviour
{
    public static Calibration Instance;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip calibrationClip;

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
        UpdateVisualOffset(PlayerPrefs.GetFloat(Constants.Game.PlayerVisualLatency, 0f));
        UpdateInputOffset(PlayerPrefs.GetFloat(Constants.Game.PlayerInputLatency, 0f));

    }

    public void StartCalibration()
    {
        audioSource.clip = calibrationClip;
        dspCalibrationTime = (float)AudioSettings.dspTime;
        audioSource.Play();
    }

    public void StopCalibration()
    {
        audioSource.Stop();
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

    public void UpdateVisualOffset(float offset)
    {
        visualCalibrationOffset = offset;
        PlayerPrefs.SetFloat(Constants.Game.PlayerVisualLatency, offset);
		PlayerPrefs.Save();
    }

    public void UpdateInputOffset(float offset)
    {
        inputCalibrationOffset = offset;
        PlayerPrefs.SetFloat(Constants.Game.PlayerInputLatency, offset);
		PlayerPrefs.Save();
    }
}
