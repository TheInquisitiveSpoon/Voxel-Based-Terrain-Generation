//  HUDFPS.cs - Shows FPS in a UI element
//  Class designed based on the following source:
//  Available at:   https://web.archive.org/web/20201111204658/http://wiki.unity3d.com/index.php?title=FramesPerSecond

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//  CLASS:
public class HUDFPS : MonoBehaviour
{
    //  REFERENCES:
    public Text     FPSDisplay;

    //  VARIABLES:
    public float    UpdateTimer     = 0.5f;
    public float    AccumulatedFPS;
    public float    CurrentTimer;
    public int      Frames;

    // Start is called before the first frame update
    void Start()
    {
        CurrentTimer = UpdateTimer;
    }

    // Update is called once per frame
    void Update()
    {
        //  Reduces current timer by delta time.
        CurrentTimer -= Time.deltaTime;

        //  Adds timescale by delta time to determine accumulated FPS.
        AccumulatedFPS += Time.timeScale / Time.deltaTime;

        //  Adds to frame counter.
        Frames++;

        //  Calculates frames per second and outputs it to text, after timer passed.
        if (CurrentTimer <= 0.0f)
        {
            //  Gets accumulated FPS over time passed.
            float fps = AccumulatedFPS / Frames;

            //  Formats FPS to string.
            string output = System.String.Format("{0:F2} FPS", fps);

            //  Sets text to the output string.
            FPSDisplay.text = output;

            //  Changes FPS text colour based on the value of the FPS.
            if (fps < 15)
            {
                FPSDisplay.color = Color.red;
            }
            else if (fps < 30)
            {
                FPSDisplay.color = Color.yellow;
            }
            else
            {
                FPSDisplay.color = Color.green;
            }

            //  Resets timer, accumulated frames and frame counter.
            CurrentTimer = UpdateTimer;
            AccumulatedFPS = 0.0f;
            Frames = 0;
        }
    }
}