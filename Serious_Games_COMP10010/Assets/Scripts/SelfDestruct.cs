using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] private float lifeTime = 1.0f;
    [SerializeField] private bool playOnAwake = true;
    private bool isPlaying = false;
    private bool hasDestructed = false;
    public System.Action onDestruct;

    private void Awake ()
    {
        if (playOnAwake)
            isPlaying = true;
    }

    public void Play ()
    {
        isPlaying = true;
    }

    public void Pause ()
    {
        isPlaying = false;
    }

    private void Update ()
    {
        if (isPlaying)
        {
            lifeTime -= Time.deltaTime;

            if(lifeTime <= 0.0f)
            {
                DestructNow ();
            }
        }        
    }

    public void DestructNow ()
    {
        if (hasDestructed) return;
        hasDestructed = true;

        onDestruct?.Invoke ();
        Destroy ( this.gameObject );
    }
}
