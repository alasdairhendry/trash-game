using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityCallbacks : MonoBehaviour
{
    [SerializeField] private Citizen citizen;

    private void OnBecameVisible ()
    {
        Debug.Log ( "Visible" );
        citizen.IsCulled = false;
    }

    private void OnBecameInvisible ()
    {
        Debug.Log ( "Invisible" );
        citizen.IsCulled = true;
    }
}
