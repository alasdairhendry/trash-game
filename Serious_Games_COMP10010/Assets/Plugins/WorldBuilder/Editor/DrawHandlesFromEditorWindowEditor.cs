using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor ( typeof ( DrawHandlesFromEditorWindow ) )]
public class DrawHandlesFromEditorWindowEditor : Editor
{
    static DrawHandlesFromEditorWindow targ;

    private void OnEnable ()
    {
        if(targ == null)
        {
            targ = GameObject.FindObjectOfType<DrawHandlesFromEditorWindow> ();
        }

        if (targ != null)
            DestroyImmediate ( targ.gameObject );
    }

    [DrawGizmo(GizmoType.Active)]
    static void RenderCustomGizmo (Transform objectTransform, GizmoType gizmoType)
    {
        if (targ == null)
        {
            targ = GameObject.FindObjectOfType<DrawHandlesFromEditorWindow> ();
            return;
        }

        for (int i = 0; i < targ.cubes.Count; i++)
        {
            Gizmos.DrawCube ( targ.cubes[i], Vector3.one * targ.cubeSize );
        }
    }
}
