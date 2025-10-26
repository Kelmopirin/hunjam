using UnityEngine;
using UnityEngine.UI;

public class SensitivitySlider : MonoBehaviour
{
    public PlayerController controller;
    public Slider slider;

    void Start()
    {
        slider.minValue = 1f;
        slider.maxValue = 300f;
        slider.value = controller.player.MouseSensitivity;

        slider.onValueChanged.AddListener(value =>
        {
            controller.player.MouseSensitivity = value;
        });
    }
}

