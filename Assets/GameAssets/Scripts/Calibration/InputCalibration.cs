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
    [SerializeField] private TMP_Text delayText;
    private PlayerInputs inputs;
    [SerializeField] private List<float> inputTimes = new List<float>();
    [SerializeField] Animator anim;
    void Start()
    {
        UpdateDelayText();
    }

    public void ResetCalibration()
    {
        inputTimes.Clear();
        Calibration.Instance.UpdateInputOffset(0);
        UpdateDelayText();

    }

    private void Update()
    {
        float beat = Calibration.Instance.calibrationPositionInBeats % 1;
    }

    public void HandleKeyPress(InputAction.CallbackContext context)
    {
        anim.SetTrigger("Tap");
        float beat = Calibration.Instance.calibrationPositionInBeats % 1;
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
        if (inputTimes.Count > 20)
        {
            inputTimes.RemoveAt(0);
        }
        Calibration.Instance.UpdateInputOffset(inputTimes.Average());

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
