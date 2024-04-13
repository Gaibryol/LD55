using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualCalibration : MonoBehaviour
{
    [SerializeField] private Image[] activeImages;
    [SerializeField] private Slider calibrationSlider;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Calibration.visualCalibrationOffset = calibrationSlider.value / 1000f;
        for (int i = 0; i < activeImages.Length; i++)
        {
            activeImages[i].enabled = (i) == (Mathf.FloorToInt(Calibration.Instance.calibrationPositionInBeats) % 4);
            
        }
    }
}
