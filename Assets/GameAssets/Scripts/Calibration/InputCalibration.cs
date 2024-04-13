using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEditor;
using TMPro;

public class InputCalibration : MonoBehaviour
{
    [SerializeField] private GameObject inputHitIndicatorPrefab;
    [SerializeField] private CalibrationSyncedAnimation inputTracker;
    [SerializeField] private TMP_Text delayText;
    [SerializeField] private Image beatGraphic;
    private PlayerInputs inputs;
    [SerializeField] private List<float> inputTimes = new List<float>();

    void Start()
    {
        UpdateDelayText();

    }

    public void ResetCalibration()
    {
        inputTimes.Clear();
        Calibration.inputCalibrationOffset = 0;
        UpdateDelayText();

    }

    private void Update()
    {
        float beat = Calibration.Instance.calibrationPositionInBeats % 1;
        beatGraphic.enabled = beat < .1f || beat > .9f;
    }

    public void HandleKeyPress(InputAction.CallbackContext context)
    {
        float beat = Calibration.Instance.calibrationPositionInBeats % 1;
        Debug.Log(beat);
        float timeFromBeat = 0f;
        if (beat < .5f) // late or super early
        {
            timeFromBeat = beat;
        } else if (beat >= .5f) // early or super late
        {
            timeFromBeat = 1f - beat;
        }
        timeFromBeat *= Calibration.Instance.secPerBeat;

        inputTimes.Add(timeFromBeat);
        if (inputTimes.Count > 50)
        {
            inputTimes.RemoveAt(0);
        }
        Calibration.inputCalibrationOffset = inputTimes.Average();
        UpdateDelayText();
    }

    private void UpdateDelayText()
    {
        delayText.text = string.Format("{0:0.##}", Mathf.Abs(Calibration.inputCalibrationOffset * 1000)) + "ms";

    }

    private void OnEnable()
    {
        if (inputs == null)
        {
            inputs = new PlayerInputs();
        }
        inputs.Player.Enable();
        inputs.Player.Up.performed += HandleKeyPress;
        inputs.Player.Left.performed += HandleKeyPress;
        inputs.Player.Right.performed += HandleKeyPress;
        inputs.Player.Down.performed += HandleKeyPress;

    }

    private void OnDisable()
    {
        inputs.Player.Disable();
        inputs.Player.Up.performed -= HandleKeyPress;
        inputs.Player.Left.performed -= HandleKeyPress;
        inputs.Player.Right.performed -= HandleKeyPress;
        inputs.Player.Down.performed -= HandleKeyPress;
    }
}
