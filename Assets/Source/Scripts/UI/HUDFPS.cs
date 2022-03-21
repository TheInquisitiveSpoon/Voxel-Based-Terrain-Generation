using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDFPS : MonoBehaviour
{
    public Text FPSDisplay;
    public float UpdateTimer = 0.5f;
    public float AccumulatedFPS;
    public float CurrentTimer;
    public int Frames;

    // Start is called before the first frame update
    void Start()
    {
        CurrentTimer = UpdateTimer;
    }

    // Update is called once per frame
    void Update()
    {
        CurrentTimer -= Time.deltaTime;
        AccumulatedFPS += Time.timeScale / Time.deltaTime;
        Frames++;

        if (CurrentTimer <= 0.0f)
        {
            float fps = AccumulatedFPS / Frames;
            string output = System.String.Format("{0:F2} FPS", fps);
            FPSDisplay.text = output;

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

            CurrentTimer = UpdateTimer;
            AccumulatedFPS = 0.0f;
            Frames = 0;
        }
    }
}