using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EditorExtensions 
{
    public static GUIStyle TabToolbarActiveStyle
    {
        get
        {
            GUIStyle style = new GUIStyle ( EditorStyles.toolbarButton );
            style.active = style.normal;
            style.fontStyle = FontStyle.Bold;

            return style;
        }
    }

    public static GUIStyle TabToolbarInactiveStyle
    {
        get
        {
            GUIStyle style = new GUIStyle ( EditorStyles.toolbarButton );
            style.normal.background = GUIStyleBackground ( 0.7f );
            style.active.background = GUIStyleBackground ( 0.6f );

            return style;
        }
    }

    public static Texture2D GUIStyleBackground(float colourModifier)
    {
        Color colour = new Color ( colourModifier, colourModifier, colourModifier, 1 );
        Color[] c = new Color[1 * 1];

        for (int i = 0; i < c.Length; i++)
        {
            c[i] = colour;
        }

        Texture2D tex = new Texture2D ( 1, 1 );
        tex.SetPixels ( c );
        tex.Apply ();
        return tex;
    }

    public static void Horizontal (System.Action action)
    {
        if (action == null) return;

        EditorGUILayout.BeginHorizontal ();
        action ();
        EditorGUILayout.EndHorizontal ();
    }

    public static void HorizontalBox (System.Action action)
    {
        if (action == null) return;

        EditorGUILayout.BeginHorizontal ( "Box" );
        action ();
        EditorGUILayout.EndHorizontal ();
    }

    public static void HorizontalLine (float saturation = 0.5f)
    {
        GUIStyle horizontalLine;
        horizontalLine = new GUIStyle ();
        horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
        horizontalLine.margin = new RectOffset ( 0, 0, 4, 4 );
        horizontalLine.fixedHeight = 1;

        var c = GUI.color;
        GUI.color = new Color ( saturation, saturation, saturation, 1.0f );
        GUILayout.Box ( GUIContent.none, horizontalLine );
        GUI.color = c;
    }

    public static void HorizontalLineSpaced (float saturation = 0.5f)
    {
        GUIStyle horizontalLine;
        horizontalLine = new GUIStyle ();
        horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
        horizontalLine.margin = new RectOffset ( 0, 0, 4, 4 );
        horizontalLine.fixedHeight = 1;

        var c = GUI.color;
        GUI.color = new Color ( saturation, saturation, saturation, 1.0f );
        EditorGUILayout.Space ();
        GUILayout.Box ( GUIContent.none, horizontalLine );
        EditorGUILayout.Space ();
        GUI.color = c;
    }

    public static void HorizontalToolbar (System.Action action)
    {
        if (action == null) return;

        EditorGUILayout.BeginHorizontal ( EditorStyles.toolbar );
        action ();
        EditorGUILayout.EndHorizontal ();
    }

    public static void Vertical (System.Action action)
    {
        if (action == null) return;

        EditorGUILayout.BeginVertical ();
        action ();
        EditorGUILayout.EndVertical ();
    }

    public static void VerticalBox (System.Action action)
    {
        if (action == null) return;

        EditorGUILayout.BeginVertical ( "Box" );
        action ();
        EditorGUILayout.EndVertical ();
    }

    public static void VerticalScroll (System.Action action, ref Vector2 scrollPos)
    {
        if (action == null) return;

        scrollPos = EditorGUILayout.BeginScrollView ( scrollPos, false, false );
        EditorGUILayout.BeginVertical ();
        action ();
        EditorGUILayout.EndVertical ();
        EditorGUILayout.EndScrollView ();
    }

    public static void Box (System.Action action, ref Vector2 scrollPos)
    {
        if (action == null) return;

        scrollPos = EditorGUILayout.BeginScrollView ( scrollPos );
        action ();
        EditorGUILayout.EndScrollView ();
    }
}
