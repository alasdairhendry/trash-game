using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
    float deltaTime = 0.0f;
    float totalFps = 0.0f;
    int counter = 0;
    
    void Update ()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI ()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle ();

        Rect rect = new Rect ( 0, 0, w, h * 2 / 100 );
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = new Color ( 0.0f, 0.0f, 0.5f, 1.0f );
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;

        totalFps += fps;
        counter++;
        float totalFPSDisplay = totalFps / (float)counter;

        if (counter > 120)
        {
            totalFps = 0.0f;
            counter = 0;
            totalFPSDisplay = 0.0f;
        }

        string text = fps.ToString ( "0" ) + " fps (" + totalFPSDisplay.ToString ( "0" ) + " avg)";
        GUI.Label ( rect, text, style );
    }
}