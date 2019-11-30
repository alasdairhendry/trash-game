using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vice : MonoBehaviour
{
    [SerializeField] private Transform binPlaceholder;
    [SerializeField] private Transform target;
    [SerializeField] private Animator anim;
    [SerializeField] private TruckLights hazardLights;
    private Vector3 posOffset = new Vector3 ();

    private bool isVicing = false;

    private List<Transform> binsInTrigger = new List<Transform> ();    

    private void Update ()
    {
        if (Input.GetKeyDown ( KeyCode.E ))
        {
            if(binsInTrigger.Count > 0)
            {
                DoVice ( binsInTrigger[0] );
            }
            else
            {
                Debug.Log ( "no bins" );
            }
        }  
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag ( "bin" ))
        {
            if (!binsInTrigger.Contains ( other.transform )) binsInTrigger.Add ( other.transform );
        }
    }

    private void OnTriggerExit (Collider other)
    {
        if (other.CompareTag ( "bin" ))
        {
            if (binsInTrigger.Contains ( other.transform )) binsInTrigger.Remove ( other.transform );
        }
    }

    private void FixedUpdate ()
    {
        if (isVicing)
        {
            if(target != null)
            {
                target.GetComponent<Rigidbody> ().MovePosition ( binPlaceholder.TransformPoint( posOffset ) );
            }
        }
    }

    public void DoVice (Transform target)
    {
        if (isVicing) return;
        hazardLights.Blink ();
        this.target = target;
        isVicing = true;

        Rigidbody rbBase = this.target.GetComponent<Rigidbody> ();
        Rigidbody rbLid = this.target.GetComponentsInChildren<Rigidbody> ()[1];

        if (rbBase)
        {
            rbBase.transform.SetParent ( binPlaceholder );
            posOffset = rbBase.transform.localPosition;            

            rbBase.useGravity = false;
            rbBase.detectCollisions = false;
        }

        if (rbLid)
        {

            rbLid.useGravity = false;
            rbLid.detectCollisions = false;
        }

        anim.SetBool ( "active", true );
    }

    public void Stop ()
    {
        float backForce = 5;
        float upForce = 2;

        Rigidbody rbBase = this.target.GetComponent<Rigidbody> ();
        Rigidbody rbLid = this.target.GetComponentsInChildren<Rigidbody> ()[1];

        if (rbBase)
        {
            rbBase.transform.SetParent ( null );
            rbBase.useGravity = true;
            rbBase.detectCollisions = true;
        }

        if (rbLid)
        {
            rbLid.useGravity = true;
            rbLid.detectCollisions = true;
        }

        rbBase.AddForce ( -transform.forward * backForce, ForceMode.VelocityChange );
        rbBase.AddForce ( Vector3.up * upForce, ForceMode.VelocityChange );
        target.SetParent ( null );

        target = null;
        hazardLights.TurnOff ();
        isVicing = false;
    }
}
