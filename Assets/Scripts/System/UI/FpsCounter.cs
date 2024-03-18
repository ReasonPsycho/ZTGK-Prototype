using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
    TextMeshProUGUI text;

    private int averageFps = 0;
    private float timePassed = 0;
    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        CalculateFps();
        if(timePassed == 0)
            text.text = "FPS: " + averageFps;
        timePassed += Time.deltaTime;
        if (timePassed > 3)
        {
            timePassed = 0;
        }
    }


    private void CalculateFps()
    {
        List<float> fpsList = new List<float>();
        float fps = 1.0f / Time.unscaledDeltaTime;
        fpsList.Add(fps);
        if (fpsList.Count > 60)
        {
            fpsList.RemoveAt(0);
        }
        averageFps = (int)fpsList.Average();
    }
}
