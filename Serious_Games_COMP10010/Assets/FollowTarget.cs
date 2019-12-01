using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    private Transform target;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void LateUpdate ()
    {
        transform.position = target.position + (Vector3.up * 0.1f);
    }
}
