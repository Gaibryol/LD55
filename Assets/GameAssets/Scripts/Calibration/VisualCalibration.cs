using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VisualCalibration : MonoBehaviour
{
    [SerializeField] private Image[] activeImages;
    [SerializeField] private Slider calibrationSlider;
    [SerializeField] private TMP_Text valueText;
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
    void Update()
    {
        for (int i = 0; i < activeImages.Length; i++)
        {
            activeImages[i].enabled = (i) == (Mathf.FloorToInt(Calibration.Instance.calibrationPositionInBeats) % 4);
            
        }
    }
}
