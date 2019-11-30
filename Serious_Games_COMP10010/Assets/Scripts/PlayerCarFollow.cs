using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarFollow : MonoBehaviour
{
    [SerializeField] private PlayerCar pc;
    [SerializeField] private Transform target;
    //[SerializeField] private bool follow;
    [SerializeField] private float targetFollowDistance;
    [SerializeField] private float targetFollowMinAngle;

    [SerializeField] private float distance;
    [SerializeField] private float angle;

   private bool isFollowing = false;

    public void SetIsFollowing(bool state)
    {
        isFollowing = state;
    }

    private void Update ()
    {
        //if (isFollowing)
        //{
        //    if (!follow)
        //    {
        //        // turn off
        //        pc.ReceiveInput ( new Vector2 ( 0.0f, 0.0f ), false );
        //        isFollowing = false;
        //        pc.SetIsFollowing ( isFollowing );
        //    }
        //}
        //else
        //{
        //    if (follow)
        //    {
        //        // turn on
        //        pc.ReceiveInput ( new Vector2 ( 0.0f, 0.0f ), false );
        //        isFollowing = true;
        //        pc.SetIsFollowing ( isFollowing );
        //    }
        //}        

        if (target)
            distance = Vector3.Distance ( target.position, transform.position );

        DoFollow ();
    }

    private void DoFollow ()
    {
        if (!isFollowing) return;

        Vector3 dir = (new Vector2 ( target.position.x, target.position.z ) - new Vector2 ( transform.position.x, transform.position.z )).normalized;
        angle = Vector3.Angle ( new Vector2 ( transform.forward.x, transform.forward.z ), dir );

        Vector3 targetInverse = transform.InverseTransformPoint ( target.position );

        float horizontalDirectionalModifier = (targetInverse.x < 0) ? -1.0f : 1.0f;
        float verticalDirectionalModifier = (targetInverse.z < 0) ? -1.0f : 1.0f;
        float angleNormalisedTarget = Mathf.InverseLerp ( 0.0f, 180.0f, Mathf.Abs ( angle ) );

        float horizontal = (distance < targetFollowDistance) ? 0.0f : angleNormalisedTarget * horizontalDirectionalModifier * verticalDirectionalModifier;
        float vertical = (distance < targetFollowDistance) ? 0.0f : 1.0f * verticalDirectionalModifier;

        pc.ReceiveInput ( new Vector2 ( horizontal, vertical ), distance < targetFollowDistance );
    }
}
