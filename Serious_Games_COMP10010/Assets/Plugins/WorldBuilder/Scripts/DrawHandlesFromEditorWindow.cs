using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawHandlesFromEditorWindow : MonoBehaviour
{
    public List<List<Vector3>> lines { get; set; } = new List<List<Vector3>> ();
    public List<Vector3> cubes { get; set; } = new List<Vector3> ();

    public float cubeSize { get; set; } = 0.1f;
}
