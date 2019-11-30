using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClipping : MonoBehaviour
{
    Camera cam;
    [SerializeField] private float x = 120;
    float[] distances = new float[32];

    private void Start ()
    {
        cam = GetComponent<Camera> ();

        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = 0;
        }

        distances[11] = x;

        cam.layerCullSpherical = true;
        cam.layerCullDistances = distances;
    }

    private void OnValidate ()
    {
        if (cam == null) return;
        if (Application.isPlaying)
        {
            distances[11] = x;
            cam.layerCullDistances = distances;
        }
    }
}
