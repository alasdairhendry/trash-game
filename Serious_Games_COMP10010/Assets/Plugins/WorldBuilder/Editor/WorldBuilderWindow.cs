using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static EditorExtensions;

public class WorldBuilderWindow : EditorWindow
{
    static EditorWindow window;
    static PrefabReplacer pr;
    private DrawHandlesFromEditorWindow drawHandlesFromEditorWindow;

    private Vector2 scrollPos = new Vector2 ();

    private GameObject previouslyUsedPrefab = null;
    private GameObject prefabToAddToReplaceList = null;

    public enum Tab { Replace, Place, Array, QuickParent }
    private Tab currentTab = Tab.Replace;

    private ArrayTabData arrayTabData = new ArrayTabData () { amount = 1, spacing = 5, target = null, space = Space.World, direction = new Vector3 ( 1.0f, 0.0f, 0.0f ), addToTargetParent = true };
    private static QuickParentData quickParentData = null;

    private GameObject prefabToPlace = null;

    [MenuItem ( "Tools/Open World Builder", priority = 100 )]
    private static void Open ()
    {
        CreateWindow ();
    }

    private static void CreateWindow ()
    {
        if (window == null)
        {
            window = EditorWindow.GetWindow<WorldBuilderWindow> ();
            window.titleContent = new GUIContent ( "World Builder" );
            window.Show ();
        }      
    }

    private static void VerifyQuickParentData ()
    {
        string assetPath = "Assets/Plugins/WorldBuilder/Data/QuickParentData.asset";

        if (quickParentData == null)
        {
            quickParentData = AssetDatabase.LoadAssetAtPath<QuickParentData> ( assetPath );

            if (quickParentData == null)
            {
                quickParentData = ScriptableObject.CreateInstance<QuickParentData> ();
                quickParentData.categories.Add ( "Default" );
                AssetDatabase.CreateAsset ( quickParentData, assetPath );
                AssetDatabase.SaveAssets ();
            }
        }
    }

    private void OnFocus ()
    {
        if (pr == null)
            pr = FindObjectOfType<PrefabReplacer> ();

        VerifyQuickParentData ();
    }

    private void OnDisable ()
    {
        if (drawHandlesFromEditorWindow != null) DestroyImmediate ( drawHandlesFromEditorWindow.gameObject );
    }

    private void OnDestroy ()
    {        
        if (drawHandlesFromEditorWindow != null) DestroyImmediate ( drawHandlesFromEditorWindow.gameObject );
    }

    private void OnGUI ()
    {
        EditorGUILayout.Space ();

        HorizontalToolbar ( () =>
        {          
            for (int i = 0; i < Enum.GetValues ( typeof ( Tab ) ).Length; i++)
            {
                string tabString = ((Tab)i).ToString ();                

                if(i == (int)currentTab)
                {
                    if (GUILayout.Button ( tabString, TabToolbarActiveStyle ))
                    {
                        currentTab = (Tab)i;
                    }
                }
                else
                {
                    if (GUILayout.Button ( tabString, TabToolbarInactiveStyle ))
                    {
                        currentTab = (Tab)i;
                    }
                }
            }



            if (GUILayout.Button ( "Query", EditorStyles.toolbarDropDown ))
            {
                GenericMenu menu = new GenericMenu ();
                menu.AddItem ( new GUIContent ( "Create Query" ), false, () => { SelectTopLayerObjects (false); } );
                menu.AddItem ( new GUIContent ( "Query From Selection" ), false, () => { SelectTopLayerObjects (true); } );
                menu.ShowAsContext ();
            }
        } );

        HorizontalLineSpaced ();

        if(currentTab!= Tab.Array)
        {
            if (drawHandlesFromEditorWindow != null) DestroyImmediate ( drawHandlesFromEditorWindow.gameObject );
        }

        switch (currentTab)
        {
            case Tab.Replace:
                ReplaceGUI ();
                break;
            case Tab.Array:
                ArrayGUI ();
                break;
            case Tab.QuickParent:
                QuickParentGUI ();
                break;
            case Tab.Place:
                Place ();
                break;
        }

    }

    private void Place ()
    {
        if(prefabToPlace == null)
        {
            if (Selection.objects.Length >= 1)
            {
                if (PrefabUtility.GetPrefabAssetType ( Selection.objects[0] ) != PrefabAssetType.NotAPrefab)
                {
                    prefabToPlace = Selection.objects[0] as GameObject;
                }
            }
        }
        //else
        //{
        //    //if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null)
        //    //{
        //        RaycastHit hit;
        //    //Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay ( Event.current.mousePosition );
        //    Ray ray = HandleUtility.GUIPointToWorldRay ( Event.current.mousePosition );

        //        if (Physics.Raycast ( ray, out hit ))
        //        {
        //            Debug.Log ( hit.collider.gameObject.name );
        //        }

        //        //SceneView.lastActiveSceneView.Repaint ();
        //        Repaint ();
        //    //}
        //}

        //EditorGUILayout.LabelField ( "Prefab", (prefabToPlace) ? prefabToPlace.name : "null" );
    }

    private void QuickParentGUI ()
    {
        EditorGUI.BeginChangeCheck ();
        quickParentData.editMode = EditorGUILayout.Toggle ( "Edit Mode", quickParentData.editMode );
        if (EditorGUI.EndChangeCheck ())
        {
            if (quickParentData.datas.Count > 0)
                quickParentData.datas = quickParentData.datas.OrderBy ( x => x.category ).ToList ();
        }

        List<QuickParentData.Data> eligibleData = quickParentData.datas.Where ( x => x.scene == SceneManager.GetActiveScene ().name ).ToList ();

        if (eligibleData.Count > 0)
        {
            VerticalScroll ( () =>
              {
                  string category = eligibleData[0].category;

                  for (int i = 0; i < eligibleData.Count; i++)
                  {
                      if(category != eligibleData[i].category || i == 0)
                      {
                          HorizontalLine ();
                          category = quickParentData.datas[i].category;
                          EditorGUILayout.LabelField ( category, EditorStyles.boldLabel );
                      }

                      if (eligibleData[i] == null) continue;
                      if (eligibleData[i].transform == null) continue;

                      Horizontal ( () =>
                      {
                          if (!quickParentData.editMode)
                          {
                              if (GUILayout.Button ( eligibleData[i].customName ))
                              {
                                  GameObject[] g = Selection.gameObjects;

                                  bool isAsset = false;

                                  for (int x = 0; x < g.Length; x++)
                                  {
                                      if (AssetDatabase.Contains ( g[x] ))
                                      {
                                          isAsset = true;
                                          break;
                                      }
                                  }

                                  if (isAsset)
                                  {
                                      return;
                                  }

                                  for (int x = 0; x < g.Length; x++)
                                  {
                                      Undo.SetTransformParent ( g[x].transform, eligibleData[i].transform, "Quick Parent to " + eligibleData[i].transform.name );
                                      g[x].transform.SetParent ( eligibleData[i].transform );
                                  }

                                  return;
                              }
                          }
                          else
                          {
                              eligibleData[i].customName = EditorGUILayout.TextField ( eligibleData[i].customName );
                          }

                          if (quickParentData.editMode)
                          {
                              if (EditorGUILayout.DropdownButton ( new GUIContent ( eligibleData[i].category ), FocusType.Passive ))
                              {
                                  GenericMenu menu = new GenericMenu ();

                                  for (int y = 0; y < quickParentData.categories.Count; y++)
                                  {
                                      int categoryIndex = y;
                                      int dataIndex = i;


                                      menu.AddItem 
                                      (
                                          new GUIContent ( quickParentData.categories[y] ),

                                          eligibleData[i].category == quickParentData.categories[y],

                                          () =>
                                          {
                                              eligibleData[dataIndex].category = quickParentData.categories[categoryIndex];
                                          } 
                                      );
                                  }

                                  menu.ShowAsContext ();
                              }
                              
                              if (GUILayout.Button ( "Ping", GUILayout.MaxWidth ( 64 ) ))
                              {
                                  EditorGUIUtility.PingObject ( eligibleData[i].transform );
                              }

                              if (GUILayout.Button ( "X", GUILayout.MaxWidth ( 32 ) ))
                              {
                                  quickParentData.datas.RemoveAt ( i );
                              }
                          }
                      } );
                  }
              }, ref scrollPos );
        }
        EditorGUILayout.Separator ();
        EditorGUILayout.Separator ();
        GUILayout.FlexibleSpace ();

        if (quickParentData.editMode)
        {
            HorizontalLine ();

            EditorGUILayout.LabelField ( "Categories", EditorStyles.boldLabel );

            for (int i = 0; i < quickParentData.categories.Count; i++)
            {
                Horizontal ( () =>
                 {
                     EditorGUILayout.LabelField ( (i+1).ToString("0") + ". " + quickParentData.categories[i].ToString () );

                     if (quickParentData.categories[i] != "Default")
                     {
                         if (GUILayout.Button ( "X", GUILayout.MaxWidth ( 32 ) ))
                         {
                             quickParentData.categories.RemoveAt ( i );

                             for (int y = 0; y < eligibleData.Count; y++)
                             {
                                 if (!quickParentData.categories.Contains( eligibleData[y].category ))
                                 {
                                     eligibleData[y].category = "Default";
                                 }
                             }
                         }
                     }
                 } );
            }

            Horizontal ( () =>
            {
                quickParentData.categoryToAdd = EditorGUILayout.TextField ( "Add Category", quickParentData.categoryToAdd );

                if (GUILayout.Button ( "Add", GUILayout.MaxWidth ( 64 ) ))
                {
                    if (!quickParentData.categories.Contains ( quickParentData.categoryToAdd ) && !string.IsNullOrEmpty ( quickParentData.categoryToAdd ))
                    {
                        quickParentData.categories.Add ( quickParentData.categoryToAdd );
                        quickParentData.categoryToAdd = "";
                    }
                }
            } );
        }

        HorizontalLine ();

        Transform transformToAdd = null;
        transformToAdd = EditorGUILayout.ObjectField ( "Add", transformToAdd, typeof ( Transform ), true ) as Transform;

        if (transformToAdd != null)
        {
            if (quickParentData.datas.FirstOrDefault ( x => x.GUID == transformToAdd.GetInstanceID () ) == null)
            {
                quickParentData.datas.Add ( new QuickParentData.Data ( transformToAdd, "Default", transformToAdd.name, transformToAdd.gameObject.scene ) );
            }
        }

        if(GUILayout.Button("Add Selected" ))
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                if (quickParentData.datas.FirstOrDefault ( x => x.transform == Selection.gameObjects[i].transform ) == null)
                {
                    quickParentData.datas.Add ( new QuickParentData.Data ( Selection.gameObjects[i].transform, "Default", Selection.gameObjects[i].transform.name, Selection.gameObjects[i].transform.gameObject.scene ) );
                }
            }
        }
    }

    private void ArrayGUI ()
    {
        if (Selection.gameObjects.Length > 1)
        {
            EditorGUILayout.HelpBox ( "Selection has multiple GameObjects. Please select one GameObject to continue", MessageType.Error, true );
            if (drawHandlesFromEditorWindow != null) DestroyImmediate ( drawHandlesFromEditorWindow.gameObject );
            Repaint ();
            return;
        }

        if (Selection.gameObjects.Length <= 0)
        {
            EditorGUILayout.HelpBox ( "Selection has no GameObjects. Please select a GameObject to continue", MessageType.Error, true );
            if (drawHandlesFromEditorWindow != null) DestroyImmediate ( drawHandlesFromEditorWindow.gameObject );
            Repaint ();
            return;
        }

        GameObject go = Selection.gameObjects[0];

        if (AssetDatabase.Contains ( go ))
        {
            EditorGUILayout.HelpBox ( "Selected GameObject is a Project Asset. Please select a Scene GameObject to continue.", MessageType.Error, true );
            if (drawHandlesFromEditorWindow != null) DestroyImmediate ( drawHandlesFromEditorWindow.gameObject );
            Repaint ();
            return;
        }

        if (drawHandlesFromEditorWindow == null)
        {
            GameObject _dhfew = new GameObject ( "Draw Handles From Editor Window" );
            _dhfew.transform.SetAsLastSibling ();
            drawHandlesFromEditorWindow = _dhfew.AddComponent<DrawHandlesFromEditorWindow> ();
        }

        if(go == null)
        {
            if (drawHandlesFromEditorWindow != null) DestroyImmediate ( drawHandlesFromEditorWindow.gameObject );
            Repaint ();
            return;
        }

        arrayTabData.target = EditorGUILayout.ObjectField ( "Target Object", go, typeof ( GameObject ), true ) as GameObject;
        string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot ( arrayTabData.target );
        EditorGUILayout.LabelField ( prefabPath, EditorStyles.wordWrappedLabel );

        if (!string.IsNullOrEmpty ( prefabPath ))
        {
            EditorGUILayout.Space ();
            EditorGUILayout.LabelField ( "Target is a prefab", EditorStyles.boldLabel );
        }

        HorizontalLineSpaced ();

        Vertical ( () =>
        {
            scrollPos = EditorGUILayout.BeginScrollView ( scrollPos );

            GUIStyle header = new GUIStyle ( EditorStyles.boldLabel );
            header.normal.background = GUIStyleBackground ( 0.7f );

            EditorGUILayout.LabelField ( "Array Data", header );
            EditorGUILayout.Space ();

            arrayTabData.spacing = EditorGUILayout.FloatField ( "Spacing", arrayTabData.spacing );
            if (arrayTabData.spacing < 0.0f) arrayTabData.spacing = 0.0f;

            arrayTabData.amount = EditorGUILayout.IntField ( "Amount", arrayTabData.amount );
            if (arrayTabData.amount < 1) arrayTabData.amount = 1;

            arrayTabData.addToTargetParent = EditorGUILayout.Toggle ( "Retain Parent", arrayTabData.addToTargetParent );

            arrayTabData.space = (Space)EditorGUILayout.EnumPopup ( "Space", arrayTabData.space );

            EditorGUILayout.Space ();

            arrayTabData.direction = EditorGUILayout.Vector3Field ( "Direction", arrayTabData.direction );

            EditorGUILayout.Vector3Field ( "Normalised Direction", arrayTabData.direction.normalized );

            UpdateSceneHandles ();

            EditorGUILayout.EndScrollView ();

            GUILayout.FlexibleSpace ();

            HorizontalLineSpaced ();

            Horizontal ( () =>
            {
                if (GUILayout.Button ( "Invert Direction" ))
                {
                    arrayTabData.direction = arrayTabData.direction * -1.0f;
                }

                if (GUILayout.Button ( "Cycle Directions" ))
                {
                    if (arrayTabData.direction == Vector3.left)
                        arrayTabData.direction = Vector3.forward;
                    else if (arrayTabData.direction == Vector3.forward)
                        arrayTabData.direction = Vector3.right;
                    else if (arrayTabData.direction == Vector3.right)
                        arrayTabData.direction = Vector3.back;
                    else if (arrayTabData.direction == Vector3.back)
                        arrayTabData.direction = Vector3.left;
                    else
                        arrayTabData.direction = Vector3.left;
                }
            } );

            Horizontal ( () =>
            {
                if (GUILayout.Button ( "Create Array - 1" ))
                {
                    CreateArray ( -1 );
                }

                if (GUILayout.Button ( "Create Array + 1" ))
                {
                    CreateArray ( 1 );
                }
            } );

            if (GUILayout.Button ( "Create Array" ))
            {
                CreateArray ( 0 );
            }

            HorizontalLineSpaced ();

        } );

        Repaint ();
    }

    private void CreateArray (int toAdd)
    {
        if (arrayTabData.amount + toAdd <= 0)
            toAdd = 0;

        GameObject target = arrayTabData.target;
        GameObject prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource ( arrayTabData.target );
        GameObject[] newSelection = new GameObject[arrayTabData.amount + 1 + toAdd];
        newSelection[0] = target;

        for (int i = 0; i < arrayTabData.amount + toAdd; i++)
        {
            GameObject newArrayObject = null;

            if (prefabRoot == null)
            {
                newArrayObject = Instantiate ( target );
                Undo.RegisterCreatedObjectUndo ( newArrayObject, "Create Array" );
            }
            else
            {
                newArrayObject = PrefabUtility.InstantiatePrefab ( prefabRoot ) as GameObject;
                Undo.RegisterCreatedObjectUndo ( newArrayObject, "Create Array" );
            }

            if (arrayTabData.addToTargetParent && target.transform.parent != null)
            {
                newArrayObject.transform.SetParent ( target.transform.parent );
            }

            Vector3 newPosition = target.transform.position;

            switch (arrayTabData.space)
            {
                case Space.World:
                    newPosition += arrayTabData.getDirectionNormalised * (arrayTabData.spacing * (i + 1));
                    break;
                case Space.Self:
                    newPosition += arrayTabData.target.transform.TransformDirection ( arrayTabData.getDirectionNormalised ) * (arrayTabData.spacing * (i + 1));
                    break;
            }

            newArrayObject.transform.position = newPosition;
            newArrayObject.transform.rotation = target.transform.rotation;
            newArrayObject.transform.localScale = target.transform.localScale;
            newSelection[i + 1] = newArrayObject;
        }

        Selection.objects = newSelection;
    }

    private void UpdateSceneHandles ()
    {
        drawHandlesFromEditorWindow.cubes = new List<Vector3> ();
        drawHandlesFromEditorWindow.cubeSize = 0.2f;

        for (int i = 0; i < arrayTabData.amount; i++)
        {
            Vector3 newPosition = arrayTabData.target.transform.position;

            switch (arrayTabData.space)
            {
                case Space.World:
                    newPosition += arrayTabData.getDirectionNormalised * (arrayTabData.spacing * (i + 1));
                    break;
                case Space.Self:
                    newPosition += arrayTabData.target.transform.TransformDirection ( arrayTabData.getDirectionNormalised ) * (arrayTabData.spacing * (i + 1));
                    break;
            }

            drawHandlesFromEditorWindow.cubes.Add ( newPosition );
        }
    }

    private void OnSceneGUI ()
    {

    }

    private void ReplaceGUI ()
    {
        EditorGUILayout.BeginVertical ( "Box" );

        EditorGUILayout.BeginHorizontal ();
        prefabToAddToReplaceList = EditorGUILayout.ObjectField ( "Add", prefabToAddToReplaceList, typeof ( GameObject ), false ) as GameObject;
        if (prefabToAddToReplaceList != null)
        {
            if (pr.Prefabs.Contains ( prefabToAddToReplaceList ))
            {
                prefabToAddToReplaceList = null;
            }
            else
            {
                pr.Prefabs.Add ( prefabToAddToReplaceList );
                prefabToAddToReplaceList = null;
            }
        }
        EditorGUILayout.EndHorizontal ();

        pr.view = (PrefabReplacer.View)EditorGUILayout.EnumPopup ( "View", pr.view );

        EditorGUILayout.EndVertical ();

        EditorGUILayout.Space ();
        EditorGUILayout.Space ();

        if (pr == null)
        {
            GUILayout.Label ( "No Prefab Replacer In Scene" );
            if (GUILayout.Button ( "Create Prefab Replacer" ))
            {
                GameObject go = new GameObject ( "Prefab Replacer" );
                go.transform.position = Vector3.zero;
                go.transform.eulerAngles = Vector3.zero;
                go.transform.localScale = Vector3.one;
                pr = go.AddComponent<PrefabReplacer> ();
                EditorGUIUtility.PingObject ( go );
            }

            return;
        }

        if (Selection.gameObjects.ToList ().Contains ( pr.gameObject ))
        {
            EditorGUILayout.LabelField ( "Prefab Replacer Is Selected. You cannot replace this object.", EditorStyles.boldLabel );
            return;
        }

        switch (pr.view)
        {
            case PrefabReplacer.View.List:
                DoListView ();

                break;
            case PrefabReplacer.View.Grid:
                DoGridView ();

                break;
            case PrefabReplacer.View.Details:
                DoDetailsView ();
                break;
        }
    }

    private void DoDetailsView ()
    {
        GameObject[] activeObjects = Selection.gameObjects;
        List<GameObject> newSelection = new List<GameObject> ();

        EditorGUILayout.LabelField ( activeObjects.Length + " selected objects" );
        EditorGUILayout.Space ();


        bool shownCity = false;
        bool shownTown = false;

        List<GameObject> sorted = pr.Prefabs.OrderBy ( x => AssetDatabase.GetAssetPath ( x ).Contains ( "City" ) ).ToList ();

        if(previouslyUsedPrefab != null)
        {
            Horizontal ( () =>
            {
                GUILayout.Label ( "Last Used", EditorStyles.boldLabel );

                bool clickedButton = false;

                if (GUILayout.Button ( previouslyUsedPrefab.name ))
                {
                    clickedButton = true;
                    ReplaceObjects ( activeObjects, previouslyUsedPrefab, ref newSelection );
                }

                if (clickedButton)
                    Selection.objects = newSelection.ToArray ();
            } );
           
        }

        HorizontalLineSpaced ();

        scrollPos = EditorGUILayout.BeginScrollView ( scrollPos, false, false );

        for (int i = 0; i < sorted.Count; i++)
        {
            GameObject prefab = sorted[i];

            if (prefab == null) continue;

            if (!shownCity)
            {
                if (AssetDatabase.GetAssetPath ( prefab ).Contains ( "City" ))
                {
                    HorizontalLineSpaced ();
                    EditorGUILayout.LabelField ( "City Pack", EditorStyles.boldLabel );
                    EditorGUILayout.Space ();
                    shownCity = true;
                }
            }

            if (!shownTown)
            {
                if (AssetDatabase.GetAssetPath ( prefab ).Contains ( "Town" ))
                {
                    EditorGUILayout.LabelField ( "Town Pack", EditorStyles.boldLabel );
                    EditorGUILayout.Space ();
                    shownTown = true;
                }
            }

            bool clickedButton = false;

            if (GUILayout.Button ( prefab.name ))
            {
                clickedButton = true;

                ReplaceObjects ( activeObjects, prefab, ref newSelection );                
            }

            if (clickedButton)
            {
                previouslyUsedPrefab = prefab;
                Selection.objects = newSelection.ToArray ();
            }
        }
        EditorGUILayout.EndScrollView ();
    }

    private void ReplaceObjects(GameObject[] activeObjects, GameObject prefab, ref List<GameObject> newSelection)
    {
        for (int x = 0; x < activeObjects.Length; x++)
        {
            GameObject go = PrefabUtility.InstantiatePrefab ( prefab ) as GameObject;

            Undo.RegisterCreatedObjectUndo ( go, "Replace " + activeObjects[x].name + " 1" );

            go.transform.position = activeObjects[x].transform.position;
            go.transform.eulerAngles = activeObjects[x].transform.eulerAngles;
            go.transform.localScale = activeObjects[x].transform.localScale;

            newSelection.Add ( go );

            Undo.DestroyObjectImmediate ( activeObjects[x] );
            DestroyImmediate ( activeObjects[x] );
        }
    }

    private void DoListView ()
    {
        GameObject[] activeObjects = Selection.gameObjects;
        List<GameObject> newSelection = new List<GameObject> ();

        EditorGUILayout.LabelField ( activeObjects.Length + " selected objects" );
        EditorGUILayout.Space ();

        scrollPos = EditorGUILayout.BeginScrollView ( scrollPos, false, false );

        for (int i = 0; i < pr.Prefabs.Count; i++)
        {
            GameObject prefab = pr.Prefabs[i];

            if (prefab == null) continue;


            EditorGUILayout.BeginVertical ( "Box" );
            EditorGUILayout.BeginHorizontal ();
            GUILayout.Label ( AssetPreview.GetAssetPreview ( prefab ), GUILayout.MaxWidth ( 86 ), GUILayout.MaxHeight ( 86 ) );

            EditorGUILayout.BeginVertical ();
            EditorGUILayout.LabelField ( prefab.name, EditorStyles.boldLabel );
            EditorGUILayout.LabelField ( AssetDatabase.GetAssetPath ( prefab ), EditorStyles.wordWrappedLabel );
            EditorGUILayout.EndVertical ();

            EditorGUILayout.EndHorizontal ();

            bool clickedButton = false;

            if (GUILayout.Button ( "Replace" ))
            {
                clickedButton = true;

                for (int x = 0; x < activeObjects.Length; x++)
                {
                    GameObject go = PrefabUtility.InstantiatePrefab ( prefab ) as GameObject;

                    Undo.RegisterCreatedObjectUndo ( go, "Replace " + activeObjects[x].name + " 1" );

                    go.transform.position = activeObjects[x].transform.position;
                    go.transform.eulerAngles = activeObjects[x].transform.eulerAngles;
                    go.transform.localScale = activeObjects[x].transform.localScale;

                    newSelection.Add ( go );

                    Undo.DestroyObjectImmediate ( activeObjects[x] );
                    DestroyImmediate ( activeObjects[x] );
                }
            }

            if (clickedButton)
                Selection.objects = newSelection.ToArray ();

            EditorGUILayout.EndVertical ();

        }
        EditorGUILayout.EndScrollView ();
    }

    private void DoGridView ()
    {
        GameObject[] activeObjects = Selection.gameObjects;
        List<GameObject> newSelection = new List<GameObject> ();

        EditorGUILayout.LabelField ( activeObjects.Length + " selected objects" );
        EditorGUILayout.Space ();

        scrollPos = EditorGUILayout.BeginScrollView ( scrollPos, false, false );

        float maxWidth = 86 + 32;

        for (int i = 0; i < pr.Prefabs.Count; i++)
        {
            if (i % 3 == 0 || i == 0)
                EditorGUILayout.BeginHorizontal ();

            GameObject prefab = pr.Prefabs[i];

            if (prefab == null) continue;

            EditorGUILayout.BeginVertical ( "Box" );
            EditorGUILayout.LabelField ( prefab.name, EditorStyles.boldLabel, GUILayout.MaxWidth ( maxWidth ) );
            GUILayout.Label ( AssetPreview.GetAssetPreview ( prefab ), GUILayout.MaxWidth ( 86 + 32 ), GUILayout.MaxHeight ( 86 + 32 ) );

            bool clickedButton = false;

            if (GUILayout.Button ( "Replace", GUILayout.MaxWidth ( maxWidth ) ))
            {
                clickedButton = true;

                for (int x = 0; x < activeObjects.Length; x++)
                {
                    GameObject go = PrefabUtility.InstantiatePrefab ( prefab ) as GameObject;

                    Undo.RegisterCreatedObjectUndo ( go, "Replace " + activeObjects[x].name + " 1" );

                    go.transform.position = activeObjects[x].transform.position;
                    go.transform.eulerAngles = activeObjects[x].transform.eulerAngles;
                    go.transform.localScale = activeObjects[x].transform.localScale;

                    newSelection.Add ( go );

                    Undo.DestroyObjectImmediate ( activeObjects[x] );
                    DestroyImmediate ( activeObjects[x] );
                }
            }

            if (clickedButton)
                Selection.objects = newSelection.ToArray ();

            EditorGUILayout.EndVertical ();

            GUILayout.FlexibleSpace ();

            if (i % 3 == 2 || i == pr.Prefabs.Count - 1)
                EditorGUILayout.EndHorizontal ();
        }
        EditorGUILayout.EndScrollView ();
    }

    public static void SelectTopLayerObjects (bool useSelected)
    {
        if (useSelected)
        {
            if (Selection.objects.Length > 0)
            {
                GameObject[] gos = SceneManager.GetActiveScene ().GetRootGameObjects ();
                string name = Selection.objects[0].name.ToLower();
                if (name.ToCharArray ()[name.Length - 1] == ')')
                    name = name.Remove ( name.Length - 4 );

                List<GameObject> filtered = gos.Where ( x => x.name.ToLower ().Contains ( name ) ).ToList ();

                if (filtered.Count <= 0)
                    EditorUtility.DisplayDialog ( "No Objects", "No gameobjects with that name have been found.", "Ok" );
                else
                    Selection.objects = filtered.ToArray ();
            }

            return;
        }

        PopupWindow.Show ( "Select All", "Root scene objects containing or matching your query will be selected.",
            (s) =>
            {
                GameObject[] gos = SceneManager.GetActiveScene ().GetRootGameObjects ();
                List<GameObject> filtered = gos.Where ( x => x.name.ToLower ().Contains ( s ) ).ToList ();

                if (filtered.Count <= 0)
                    EditorUtility.DisplayDialog ( "No Objects", "No gameobjects with that name have been found.", "Ok" );
                else
                    Selection.objects = filtered.ToArray ();
            },

            "Query",

            "Select",

            (s) =>
            {
                return true;
            },

            "Cancel" );
    }

    [MenuItem("Tools/Scene Counter", priority = 100)]
    public static void GetSceneData ()
    {
        List<GameObject> a = GetAllObjectsInScene ();
        List<GameObject> m = GetAllObjectsInScene ().Where ( x => x.GetComponent<MeshFilter> () != null ).ToList ();

        int vertices = 0;

        for (int i = 0; i < m.Count; i++)
        {
            if (m[i].GetComponent<MeshFilter> () == null) continue;
            if (m[i].GetComponent<MeshFilter> ().sharedMesh == null) continue;
            vertices += m[i].GetComponent<MeshFilter> ().sharedMesh.vertexCount;
        }

        Debug.Log ( Selection.gameObjects.Length.ToString ( "n0" ) + " selected GameObjects." );
        Debug.Log ( a.Count.ToString ( "n0" ) + " gameobjects in the scene." );
        Debug.Log ( m.Count.ToString ( "n0" ) + " meshes in the scene." );
        Debug.Log ( vertices.ToString ( "n0" ) + " vertices in the scene." );
        
    }

    static List<GameObject> GetAllObjectsInScene ()
    {
        List<GameObject> objectsInScene = new List<GameObject> ();

        foreach (GameObject go in Resources.FindObjectsOfTypeAll ( typeof ( GameObject ) ) as GameObject[])
        {
            if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave)
                continue;

            if (AssetDatabase.Contains ( go )) continue;

            objectsInScene.Add ( go );
        }

        return objectsInScene;
    }

    private struct ArrayTabData
    {
        public GameObject target;
        public float spacing;
        public int amount;
        public Space space;
        public Vector3 direction;
        public Vector3 getDirectionNormalised { get { return direction.normalized; } }
        public bool addToTargetParent;
    }

    public class PopupWindow : EditorWindow
    {
        private PopupWindow window;
        private new string title;
        private string message;
        private string confirm;
        private string cancel;
        private string input;
        private string query;

        private System.Action<string> onConfirm;
        private System.Func<string, bool> validation;

        public static void Show (string title, string message, System.Action<string> onConfirm, string query = "Query", string confirm = "Ok", System.Func<string, bool> validation = null, string cancel = "Cancel")
        {
            PopupWindow window = GetWindow<PopupWindow> ( true, title );
            window.position = new Rect ( Screen.currentResolution.width / 2 - (250 / 2), Screen.currentResolution.height / 2 - (150 / 2), 250, 150 );
            window.ShowPopup ();

            window.window = window;
            window.title = title;
            window.message = message;
            window.confirm = confirm;
            window.cancel = cancel;
            window.onConfirm = onConfirm;
            window.validation = validation;
            window.query = query;
        }

        private void OnGUI ()
        {
            EditorGUILayout.BeginVertical ();

            EditorGUILayout.LabelField ( message, EditorStyles.wordWrappedLabel );

            EditorGUILayout.Space ();

            EditorGUILayout.BeginHorizontal ();
            GUILayout.Label ( query );
            input = EditorGUILayout.TextField ( input );
            EditorGUILayout.EndHorizontal ();

            GUILayout.FlexibleSpace ();

            EditorGUILayout.BeginHorizontal ();
            if (GUILayout.Button ( cancel ))
            {
                window.Close ();
            }
            if (GUILayout.Button ( confirm ))
            {
                if (validation != null)
                {
                    if (!validation ( input ))
                    {
                        EditorUtility.DisplayDialog ( "Incorrect Validation", "Your response could not be validated", "Sorry" );
                        return;
                    }
                }

                onConfirm?.Invoke ( input );
                window.Close ();
            }
            EditorGUILayout.EndHorizontal ();

            EditorGUILayout.EndVertical ();
        }
    }
}
