using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Rigidbody))]
public class Area : MonoBehaviour
{
    [SerializeField] private string areaName;
    private List<Collider> areaColliders = new List<Collider> ();

    [SerializeField] private Color debugGizmosColour = new Color ( 1.0f, 1.0f, 1.0f, 0.5f );

    public string AreaName { get => areaName; }

    private void OnTriggerEnter (Collider other)
    {
        AreaCollider area = other.GetComponent<AreaCollider> ();

        if (area == null) return;

        if(area.currentArea == null || area.currentArea != this)
        {
            area.ChangeArea ( this );
        }
    }

    private void OnValidate ()
    {
        areaColliders = GetComponentsInChildren<Collider> ().ToList ();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos ()
    {
        Bounds bounds = new Bounds ();

        if(areaColliders.Count > 0)
        {
            bounds.center = areaColliders[0].bounds.center + new Vector3(0.0f, 10.0f, 0.0f);
            bounds.size = areaColliders[0].bounds.size;
        }

        for (int i = 0; i < areaColliders.Count; i++)
        {
            Gizmos.color = debugGizmosColour;
            Gizmos.DrawCube ( areaColliders[i].bounds.center, areaColliders[i].bounds.size );
        }

        var centeredStyle = new GUIStyle ( GUI.skin.GetStyle ( "Label" ) );
        centeredStyle.alignment = TextAnchor.UpperCenter;
        centeredStyle.fontSize = 18;
        Handles.Label ( bounds.center, AreaName, centeredStyle );
    }
#endif
}
