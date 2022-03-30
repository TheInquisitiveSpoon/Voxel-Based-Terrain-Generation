using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedSlider : MonoBehaviour
{
    public Text TextElement;
    public Slider SliderElement;
    public InputField TextInput;

    public void EnterTextSeed()
    {
        int value = int.Parse(TextInput.text);
        value = value > 100000 ? 0 : value;
        SliderElement.value = value;
        TextElement.text = "Seed: " + value;
    }

    public void UpdateText()
    {
        TextElement.text = "Seed: " + SliderElement.value;
    }
}