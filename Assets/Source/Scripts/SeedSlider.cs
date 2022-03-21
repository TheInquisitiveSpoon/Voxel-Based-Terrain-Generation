using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedSlider : MonoBehaviour
{
    public Text TextElement;
    public Slider SliderElement;

    public void UpdateText()
    {
        TextElement.text = "Seed: " + SliderElement.value;
    }
}