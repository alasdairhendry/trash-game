using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Area : MonoBehaviour
{
    [SerializeField] private string areaName;
    private List<AreaCollider> areaColliders = new List<AreaCollider> ();

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
}
