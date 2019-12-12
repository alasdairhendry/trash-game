using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickSystem : MonoBehaviour
{
    public static TickSystem instance;

    private void Awake ()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy ( this.gameObject );
            return;
        }
    }

    public System.Action tick;
    public static System.Action Tick
    {
        get
        {
            return TickSystem.instance.tick;
        }
        set
        {
            TickSystem.instance.tick = value;
        }
    }

    private float timePerTick = 0.025f;
    private float currentTimeCounter;
    private int currentTick = 0;

    private void Update ()
    {
        currentTimeCounter += Time.deltaTime;

        if (currentTimeCounter >= timePerTick)
        {
            currentTimeCounter = 0.0f;
            currentTick++;

            tick?.Invoke ( );
        }
    }

    public static bool Equals (int t)
    {
        return (instance.currentTick % t == 0);
    }
}