using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Third Person Synty Character/Recoil Data" )]
public class RecoilData : ScriptableObject
{
    public float time = 5.0f;
    [Space]
    public AnimationCurve positionalCurve = null;
    public Vector3 positionalDirection = new Vector3 ();
    [Space]
    public AnimationCurve rotationalCurve = null;
    public Vector3 rotationalDirection = new Vector3 ();
    [Space]
    public AnimationCurve cameraXCurve = null;
    public float cameraXAmount = 0.0f;
    public Vector2 rangeXModifier = new Vector2 ();
    [Space]
    public AnimationCurve cameraYCurve = null;
    public float cameraYAmount = 0.0f;
    public Vector2 rangeYModifier = new Vector2 ();
    [Space]
    public float visualMultiplier = 1.0f;
    public float cameraMultiplier = 1.0f;
}
