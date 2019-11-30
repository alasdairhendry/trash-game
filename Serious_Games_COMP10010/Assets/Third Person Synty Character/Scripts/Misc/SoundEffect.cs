using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundEffect
{
    
    public static void Play(AudioClip clip)
    {
        GameObject go = new GameObject ();
        AudioSource src = go.AddComponent<AudioSource> ();

        src.clip = clip;
        src.Play ();
    }

}
