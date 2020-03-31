using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class SnapTo
{
    [MenuItem ( "World Builder/Snap/Snap Rotate &r", priority = -110 )]
    public static void SnapRotate ()
    {
        GameObject[] ts = Selection.gameObjects;

        for (int i = 0; i < ts.Length; i++)
        {
            Transform t = ts[i].transform;
            Vector3 pivot = t.position;

            if (t.GetComponentInChildren<Collider> ())
                pivot = t.GetComponentInChildren<Collider> ().bounds.center;

            Undo.RecordObject ( t, "Snap Rotate - " + t.name );

            t.RotateAround ( pivot, Vector3.up, 90.0f );
        }
    }

    [MenuItem ( "World Builder/Snap/Snap To Grid 5 &s", priority = -110 )]
    public static void SnapPositionFive ()
    {
        GameObject[] t = Selection.gameObjects;

        if (t.Length <= 0) return;

        for (int i = 0; i < t.Length; i++)
        {
            Undo.RecordObject ( t[i].transform, "Snap To Grid 5 - " + t[i].name );
            t[i].transform.position = SnapPosition ( t[i].transform.position, 5f );
        }
    }

    [MenuItem ( "World Builder/Snap/Snap To Grid 5 With 3 Y &#s", priority = -110 )]
    public static void SnapPositionFiveWithThreeY ()
    {
        GameObject[] t = Selection.gameObjects;

        if (t.Length <= 0) return;

        for (int i = 0; i < t.Length; i++)
        {
            Undo.RecordObject ( t[i].transform, "Snap To Grid 5 with 3 Y - " + t[i].name );
            t[i].transform.position = SnapPosition ( t[i].transform.position, 5f, 3.0f, 5.0f );
        }
    }

    [MenuItem ( "World Builder/Utilities/Clear Parent", priority = -100 )]
    public static void ClearParent ()
    {
        GameObject[] t = Selection.gameObjects;

        if (t.Length <= 0) return;

        for (int i = 0; i < t.Length; i++)
        {
            Undo.RecordObject ( t[i].transform, "Clear Parent - " + t[i].name );
            t[i].transform.SetParent ( null );
        }
    }

    public static Vector3 SnapPosition (Vector3 input, float factor = 1f)
    {
        if (factor <= 0f)
            throw new UnityException ( "factor argument must be above 0" );

        float x = Mathf.Round ( input.x / factor ) * factor;
        float y = 0.0f;
        float z = Mathf.Round ( input.z / factor ) * factor;

        return new Vector3 ( x, y, z );
    }

    public static Vector3 SnapPosition (Vector3 input, float xFactor = 1f, float yFactor = 1f, float zFactor = 1f)
    {
        if (xFactor <= 0f)
            throw new UnityException ( "factor argument must be above 0" );

        if (yFactor <= 0f)
            throw new UnityException ( "factor argument must be above 0" );

        if (zFactor <= 0f)
            throw new UnityException ( "factor argument must be above 0" );

        float x = Mathf.Round ( input.x / xFactor ) * xFactor;
        float y = Mathf.Round ( input.y / yFactor ) * yFactor;
        float z = Mathf.Round ( input.z / zFactor ) * zFactor;

        return new Vector3 ( x, y, z );
    }
}
