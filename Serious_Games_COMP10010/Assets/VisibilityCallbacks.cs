using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityCallbacks : MonoBehaviour
{
    [SerializeField] private Citizen citizen;

    private void OnBecameVisible ()
    {
        citizen.SetIsCulled ( false );
    }

    private void OnBecameInvisible ()
    {
        citizen.SetIsCulled ( true );
    }
}
