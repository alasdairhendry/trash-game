using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckLights : MonoBehaviour
{
    [SerializeField] private MeshRenderer mr;
    [SerializeField] private Material off;
    [SerializeField] private Material on;
    [SerializeField] private float blinkDelay;

    private bool shouldBlink = false;
    private bool isBlinking = false;

    [ContextMenu ( "Turn on")]
    public void TurnOn ()
    {
        if (shouldBlink)
        {
            shouldBlink = false;
            return;
        }

        mr.material = on;
    }

    [ContextMenu("Blink")]
    public void Blink ()
    {
        if (isBlinking) return;
        shouldBlink = true;
        StartCoroutine ( DoBlink () );
    }

    private IEnumerator DoBlink ()
    {
        isBlinking = true;

        float counter = blinkDelay;
        bool isOn = false;

        while (shouldBlink)
        {
            counter -= Time.deltaTime;

            if (counter <= 0.0f)
            {
                counter = blinkDelay;

                if (isOn)
                {
                    mr.material = off;
                    isOn = false;
                }
                else
                {
                    mr.material = on;
                    isOn = true;
                }
            }

            yield return null;
        }

        while (isOn)
        {
            counter -= Time.deltaTime;

            if (counter <= 0.0f)
            {
                mr.material = off;
                isOn = false;
            }

            yield return null;
        }

        isBlinking = false;
    }

    [ContextMenu ( "Turn off" )]
    public void TurnOff ()
    {
        if (shouldBlink)
        {
            shouldBlink = false;
            return;
        }

        mr.material = off;
    }
}
