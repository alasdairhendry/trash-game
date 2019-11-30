using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent ( typeof ( Image ) )]
public class Spritesheet : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] List<Sprite> sprites = new List<Sprite> ();
    [SerializeField] private float fps = 30;
    [SerializeField] private bool playOnAwake = true;

    private float delay;
    private float currentDelay = 0;
    private int currentIndex = 0;

    private bool isPlaying = false;

    private void Awake ()
    {
        if (playOnAwake) isPlaying = true;

        if (sprites.Count > 0)
            image.sprite = sprites[0];

        delay = 1 / fps;
    }

    private void OnValidate ()
    {
        if (image == null)
            image = GetComponent<Image> ();

        if (sprites.Count > 0)
            image.sprite = sprites[0];
    }

    public void Play ()
    {
        if (!isPlaying)
        {
            isPlaying = true;
        }
    }

    public void Stop ()
    {
        if (isPlaying)
        {
            isPlaying = false;
        }
    }

    private void Update ()
    {
        if (!isPlaying) return;
        if (sprites.Count <= 0) return;

        currentDelay += Time.deltaTime;

        if (currentDelay >= delay)
        {
            currentDelay = 0;
            currentIndex++;

            if (currentIndex >= sprites.Count)
                currentIndex = 0;

            image.sprite = sprites[currentIndex];
        }
    }
}
