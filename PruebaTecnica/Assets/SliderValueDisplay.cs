using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderValueDisplay : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI valueText; 

    void Start()
    {
        slider.onValueChanged.AddListener(UpdateValueText);
        UpdateValueText(slider.value);
    }

    void UpdateValueText(float value)
    {
        valueText.text = value.ToString("0"); 
    }
}
