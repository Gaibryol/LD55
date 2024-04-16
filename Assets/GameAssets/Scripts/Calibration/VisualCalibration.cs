using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VisualCalibration : MonoBehaviour
{
    [SerializeField] private Animator[] activeImages;
    [SerializeField] private Slider calibrationSlider;
    [SerializeField] private TMP_Text valueText;
    private int index = -1;
    void Start()
    {
        calibrationSlider.value = PlayerPrefs.GetFloat(Constants.Game.PlayerVisualLatency, 0f) * 1000f;
        valueText.text = string.Format("{0:0.##}", Calibration.visualCalibrationOffset * 1000) + "ms";
    }

    public void SliderUpdate()
    {
        Calibration.Instance.UpdateVisualOffset(calibrationSlider.value / 1000f);
        valueText.text = string.Format("{0:0.##}", Calibration.visualCalibrationOffset * 1000) + "ms";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Calibration.Instance.calibrationActive) return;

        int nextIndex = (Mathf.RoundToInt(Calibration.Instance.calibrationPositionInBeats + 1) % 4);
        if (nextIndex != index)
        {
            activeImages[nextIndex].SetTrigger("Tap");
            index = nextIndex;
        }
    }

    private void OnEnable()
    {
        index = -1;
    }
}
