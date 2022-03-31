//  SeedSlider.cs - Syncs the UI seed text, slider and input text to the correct values.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//  CLASS:
public class SeedSlider : MonoBehaviour
{
    //  REFERENCES:
    public Text         TextElement;
    public Slider       SliderElement;
    public InputField   TextInput;

    //  FUNCTIONS:
    //  Parses text in the text input to a seed value, then updates the slider and seed text.
    public void EnterTextSeed()
    {
        int value = int.Parse(TextInput.text);
        value = value > 100000 ? 0 : value;
        SliderElement.value = value;
        TextElement.text = "Seed: " + value;
    }

    //  Updates the seed text to the value of the slider.
    public void UpdateText()
    {
        TextElement.text = "Seed: " + SliderElement.value;
    }
}