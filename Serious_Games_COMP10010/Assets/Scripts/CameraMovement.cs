using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Transform forwardTarget;
    [SerializeField] Transform backwardTarget;
    [SerializeField] private float posDamp;
    [SerializeField] private float rotDamp;

    public bool lookBack = false;

    private void Awake ()
    {
        transform.parent = null;
    }

    private void FixedUpdate ()
    {
        Transform target = lookBack ? backwardTarget : forwardTarget;

        transform.position = Vector3.Slerp ( transform.position, target.position, posDamp * Time.deltaTime );
        transform.rotation = Quaternion.Slerp ( transform.rotation, target.rotation, rotDamp * Time.deltaTime );
    }
}
