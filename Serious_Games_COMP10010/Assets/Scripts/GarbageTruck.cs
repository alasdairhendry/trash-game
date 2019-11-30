using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GarbageTruck : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] PlayerCar pc;

    [SerializeField] private Vector3 forward = new Vector3 ();
    [SerializeField] private Vector3 backward = new Vector3 ();
    [SerializeField] private Vector3 velocityNrm = new Vector3 ();

    [SerializeField] TruckLights brakeLights;
    [SerializeField] TruckLights reverseLights;

    [SerializeField] Transform brakeFrontTransform;
    [SerializeField] Transform brakeRearTransform;
    [SerializeField] private float brakeDownwardForce;

    [SerializeField] List<WheelData> wheels = new List<WheelData> ();

    [SerializeField] private float angleSin = 0;
    [SerializeField] private float angleFromForward = 0;
    [SerializeField] private float angleFromBack = 0;

    [SerializeField] CameraMovement cameraMovement;
    [SerializeField] private bool lookBackEnabled;
    [SerializeField] private TextMeshProUGUI speedText;
    private PlayerCar car;

    public bool IsDrifting { get; protected set; }

    public System.Action<GameObject> OnDestroyProp;

    private void Awake ()
    {
        car = GetComponent<PlayerCar> ();
    }

    [System.Serializable]
    public class WheelData
    {
        public RaycastWheelSimple wheel;
        public TrailRenderer trail;
    }

    private void Update ()
    {
        SetDirections ();
        CheckInput ();
        CheckDrifting ();
        CheckCamera ();
        CheckSpeed ();
        CheckAir ();

        speedText.text = (car.GetNormalisedSpeed * 120).ToString ( "0" ) + "mph";
    }

    private void CheckAir ()
    {
        if (transform.position.y > 1)
        {
            int wheelsInAir = 0;

            for (int i = 0; i < wheels.Count; i++)
            {
                if (!wheels[i].wheel.IsGrounded)
                {
                    wheelsInAir++;
                }
            }

            if (wheelsInAir >= 3)
            {
                if (transform.position.y > 4)
                {
                    MultiplierManager.instance.AddProgress ( 0.3f * transform.position.y, "Big-Air" );
                }
                else
                {
                    MultiplierManager.instance.AddProgress ( 0.2f * transform.position.y, "Air-Time" );
                }
            }
        }
    }

    private void CheckSpeed ()
    {
        if (car.GetNormalisedSpeed > 0.60f)
        {
            MultiplierManager.instance.AddProgress ( 0.25f, "High-Speed" );
        }
    }

    private void SetDirections ()
    {
        forward = transform.forward;
        backward = -transform.forward;
        velocityNrm = rb.velocity.normalized;

        forward.y = 0;
        backward.y = 0;
        velocityNrm.y = 0;

        angleFromForward = Vector3.Angle ( forward, velocityNrm );
        angleFromBack = Vector3.Angle ( backward, velocityNrm );
    }

    private void CheckInput ()
    {
        Vector3 forcePosition = transform.position + (transform.up * 3.0f);
        forcePosition += velocityNrm * 5.0f;

        Vector3 brakeForceDirection = Vector3.down;

        if (pc.brakeInput)
        {
            brakeLights.TurnOn ();
            rb.AddForceAtPosition ( brakeForceDirection * brakeDownwardForce * 0.8f * Mathf.InverseLerp ( 0.0f, 25.0f, rb.velocity.sqrMagnitude ), forcePosition, ForceMode.Acceleration );

            Vector3 vel = rb.velocity;
            vel.y = 0;
            rb.AddForce ( -vel * 0.5f, ForceMode.Acceleration );
        }
        else if (pc.verticalInput < 0 && angleFromBack > 90.0f)
        {
            brakeLights.TurnOn ();

            rb.AddForceAtPosition ( brakeForceDirection * brakeDownwardForce, forcePosition, ForceMode.Acceleration );
        }
        else if (pc.verticalInput > 0 && angleFromForward > 90.0f)
        {
            brakeLights.TurnOn ();
            rb.AddForceAtPosition ( brakeForceDirection * brakeDownwardForce, forcePosition, ForceMode.Acceleration );
        }
        else
        {
            brakeLights.TurnOff ();
        }
    }

    private void CheckDrifting ()
    {
        if (angleFromForward > 35.0f && angleFromForward < 160.0f && car.GetNormalisedSpeed > 0.10f)
        {
            for (int i = 0; i < wheels.Count; i++)
            {
                if (wheels[i].trail == null) continue;
                wheels[i].trail.emitting = wheels[i].wheel.IsGrounded;
            }

            if(car.brakeInput)
                MultiplierManager.instance.AddProgress ( 0.2f, "Handbrake Turn" );
            else if (car.GetNormalisedSpeed > 0.5f)
                MultiplierManager.instance.AddProgress ( 0.5f, "High-Speed Drifting" );
            else
                MultiplierManager.instance.AddProgress ( 0.25f, "Drifting" );

            IsDrifting = true;
        }
        else
        {
            for (int i = 0; i < wheels.Count; i++)
            {
                if (wheels[i].trail == null) continue;
                wheels[i].trail.emitting = false;
            }

            IsDrifting = false;
        }
    }

    private void CheckCamera ()
    {
        if (lookBackEnabled)
        {
            if (angleFromForward > 90.0f && pc.verticalInput < 0)
            {
                cameraMovement.lookBack = true;
            }
            else if (pc.verticalInput > 0) { cameraMovement.lookBack = false; }
        }
        else
        {
            cameraMovement.lookBack = false;
        }
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag ( "Destructable" ))
        {
            OnDestroyProp?.Invoke (other.gameObject);
        }
    }

    private void OnDrawGizmos ()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine ( transform.position, transform.position + (forward * 10.0f) );
        Gizmos.color = Color.green;
        Gizmos.DrawLine ( transform.position, transform.position + (velocityNrm * 10.0f) );
    }
}
