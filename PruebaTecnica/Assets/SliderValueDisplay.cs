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
        if(value % 1 != 0)
            valueText.text = value.ToString("0.00"); 
        else
            valueText.text = value.ToString("0");
    }

    [ContextMenu(nameof(TestUpdateValueText))]
    [ExecuteInEditMode]
    void TestUpdateValueText()
    {
        UpdateValueText(slider.value);
    }
}
