using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class EditorCollapseAll
{
    [MenuItem ( "World Builder/Utilities/Collapse GameObjects #e", priority = -100 )]
    public static void CollapseAll ()
    {
        Collapse ();
    }

    public static void SendHotkey ()
    {
        var homeKey = new Event { keyCode = KeyCode.E, type = EventType.KeyUp, shift = true };
        EditorWindow.focusedWindow.SendEvent ( homeKey );
    }

    private static void Collapse ()
    {
        var hierarchy = GetFocusedWindow ( "Hierarchy" );

        var homeKey = new Event { keyCode = KeyCode.Home, type = EventType.KeyDown };
        hierarchy.SendEvent ( homeKey );

        var collapseKey = new Event { keyCode = KeyCode.LeftArrow, type = EventType.KeyDown, alt = true };
        hierarchy.SendEvent ( collapseKey );

        var openKey = new Event { keyCode = KeyCode.RightArrow, type = EventType.KeyDown };
        hierarchy.SendEvent ( openKey );
    }    

    public static EditorWindow GetFocusedWindow (string window)
    {
        FocusOnWindow ( window );
        return EditorWindow.focusedWindow;
    }

    public static void FocusOnWindow (string window)
    {
        EditorApplication.ExecuteMenuItem ( "Window/General/" + window );
    }

  
}