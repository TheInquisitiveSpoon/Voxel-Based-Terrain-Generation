//  SliderUpdater.cs - Updates slider text.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//  CLASS:
public class SliderUpdater : MonoBehaviour
{
    //  REFERENCES:
    public Slider SliderElement;
    public Text TextElement;

    //  FUNCTIONS:
    //  Updates slider text to value.
    public void UpdateText()
    {
        TextElement.text = "Render dist: " + SliderElement.value;
    }
}
