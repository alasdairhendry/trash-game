using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Third Person Synty Character/New Transform Data") )]
public class TransformData : ScriptableObject
{
    public Vector3 position = new Vector3 ();
    public Vector3 eulerAngles = new Vector3 ();
    public Vector3 localScale = new Vector3 ();

    public void SetLocal(Transform target)
    {
        target.localPosition = position;
        target.localEulerAngles = eulerAngles;
        target.localScale = localScale;
    }

}
