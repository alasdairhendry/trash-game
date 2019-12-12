
// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;
using System;

public class PlayerCar : MonoBehaviour
{

    // car physics calculations/input stuff
    public Vector3 acceleration;
    public Transform truckTransform;
    public float verticalInput { get; protected set; }
    public float horizontalInput { get; protected set; } //horizontal input control, either mobile control or keyboard
    public bool brakeInput { get; protected set; }
    public bool isFollowing { get; protected set; }
    public float GetNormalisedSpeed
    {
        get
        {
            return Mathf.InverseLerp ( 0.0f, maxSpeed, currentSpeed );
        }
    }

    private float deadZone = .001f;
    private Vector3 localRightDirection;
    private Vector3 localUpDirection;
    private Vector3 velocity;
    private Vector3 flatYVelocity;
    private Vector3 relativeVelocity;
    private Vector3 localForwardDirection;
    private Vector3 flatYLocalForwardDirection;
    private Transform localTransform;
    private new Rigidbody rigidbody;
    private Vector3 engineForce;

    private Vector3 turnDirection;
    private Vector3 simulatedGripForce;
    private float movementDirectionModifier;
    private float currentTurningAmount;
    private float carMass;
    private Transform[] wheelTransform = new Transform[4]; //these are the transforms for our 4 wheels
    public float actualGrip;
    private float maxSpeedToTurn = .2f; //keeps car from turning until it's reached this value
    private float inverseMaxAngularSpeed = 2.5f;
    [SerializeField] private float inverseTurnModifier = 2.5f;

    // the physical transforms for the car's wheels
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;

    public RaycastWheelSimple frontLeftRayCast;
    public RaycastWheelSimple frontRightRayCast;

    //these transform parents will allow wheels to turn for steering/separates steering turn from acceleration turning
    public Transform LFWheelTransform;
    public Transform RFWheelTransform;

    // car physics adjustments
    public float power = 300;
    public float maxSpeed = 50;
    public float carGrip = 70;
    public float turnSpeed = 3.0f;  //keep this value somewhere between 2.5f and 6.0f

    private float slideSpeed;
    private float currentSpeed;

    private Vector3 globalRightDirection;
    private Vector3 globalForwardDirection;
    private Vector3 tempVEC;

    private float wheelXRotation = 0;
    private float turnSpeedModifier = 1.0f;
    private float deviceAccelerometerSensitivity = 2; //how sensitive our mobile accelerometer will be
    [SerializeField] private GameObject mainCamera;
    private new AudioSource audio;

    void Start ()
    {
        Initialize ();
    }

    void Initialize ()
    {
        // Cache a reference to our car's transform
        localTransform = transform;
        // cache the rigidbody for our car
        rigidbody = GetComponent<Rigidbody> ();
        // cache our vector up direction
        localUpDirection = localTransform.up;
        // cache the mass of our vehicle
        carMass = GetComponent<Rigidbody> ().mass;
        // cache the Forward World Vector for our car
        globalForwardDirection = Vector3.forward;
        // cache the World Right Vector for our car
        globalRightDirection = Vector3.right;
        // call to set up our wheels array
        SetUpWheels ();
        // we set a COG here and lower the center of mass to a
        //negative value in Y axis to prevent car from flipping over
        rigidbody.centerOfMass = new Vector3 ( 0f, -1.7f, .35f );
        audio = GetComponent<AudioSource> ();
        frontLeftRayCast = frontLeftWheel.GetComponentInParent<RaycastWheelSimple> ();
        frontRightRayCast = frontRightWheel.GetComponentInParent<RaycastWheelSimple> ();

    }

    public void ReceiveInput (Vector2 vector2, bool v)
    {
        horizontalInput = vector2.x;
        verticalInput = vector2.y;
        brakeInput = v;
    }

    private void OnGUI ()
    {
        //GUI.Label ( new Rect ( 1080 - 64, 1080 - 64, 256, 256 ), (GetComponent<Rigidbody> ().angularVelocity.ToString()) );
        //GUI.Label ( new Rect ( 1080 - 64, 1080 - 128, 256, 256 ), turnVec.ToString() );

        GUI.Box ( new Rect ( 0.0f, 0.0f, Mathf.Lerp ( 0.0f, 1.0f, Mathf.InverseLerp ( 1.0f, inverseTurnModifier, turnSpeedModifier ) ) * 512, 64 ), "" );
    }

    void Update ()
    {
        // call the function to start processing all vehicle physics
        CarPhysicsUpdate ();


        //call the function to see what input we are using and apply it
        CheckInput ();


        if (Input.GetKeyDown ( KeyCode.E ))
        {
            if (isFollowing)
            {
                // not in trucks

                truckTransform.GetComponent<CharacterPhysics> ().SetGravity ( false );
                truckTransform.GetComponent<Rigidbody> ().detectCollisions = false;
                truckTransform.gameObject.AddComponent<FixedJoint> ().connectedBody = GetComponent<Rigidbody> ();
                truckTransform.gameObject.GetComponent<FixedJoint> ().connectedMassScale = 0.0f;
                truckTransform.gameObject.GetComponent<FixedJoint> ().massScale = 0.0f;
                truckTransform.SetParent ( this.transform );
                truckTransform.localPosition = new Vector3 ( -0.75f, 1.0f, 2.0f );
                isFollowing = false;
                ReceiveInput ( Vector2.zero, false );
                mainCamera.SetActive ( true );
            }
            else
            {
                // player in truck

                truckTransform.GetComponent<CharacterPhysics> ().SetGravity ( true );
                truckTransform.localPosition = new Vector3 ( -2.0f, 0.0f, 0.0f );
                truckTransform.GetComponent<Rigidbody> ().detectCollisions = true;
                Destroy ( truckTransform.gameObject.GetComponent<FixedJoint> () );
                truckTransform.SetParent ( null );
                isFollowing = true;
                ReceiveInput ( Vector2.zero, false );
                mainCamera.SetActive ( false );
            }
        }
    }

    void LateUpdate ()
    {
        // this function makes the visual 3d wheels rotate and turn
        RotateVisualWheels ();

        //this is where we send to a function to do engine sounds
        EngineSound ();
    }

    void SetUpWheels ()
    {
        if ((null == frontLeftWheel || null == frontRightWheel || null == rearLeftWheel || null == rearRightWheel))
        {
            Debug.LogError ( "One or more of the wheel transforms have not been plugged in on the car" );
            Debug.Break ();
        }
        else
        {
            //set up the car's wheel transforms
            wheelTransform[0] = frontLeftWheel;
            wheelTransform[1] = rearLeftWheel;
            wheelTransform[2] = frontRightWheel;
            wheelTransform[3] = rearRightWheel;
        }
    }

    void RotateVisualWheels ()
    {
        // front wheels visual rotation while steering the car

        wheelXRotation += (float)(relativeVelocity.z * 1.6 * Time.deltaTime * Mathf.Rad2Deg);

        for (int i = 0; i < 4; i++)
        {
            float wheelYRotation = 0;

            if (wheelTransform[i] == RFWheelTransform || wheelTransform[i] == LFWheelTransform)
                wheelYRotation = horizontalInput * 30;

            wheelTransform[i].localRotation = Quaternion.Euler ( new Vector3 ( wheelXRotation, wheelYRotation, 0 ) );
        }

        if (wheelXRotation >= 360f)
            wheelXRotation -= 360f;

    }

    void CheckInput ()
    {

        //Use the Keyboard for all car input
        horizontalInput = Input.GetAxisRaw ( "Horizontal" );
        verticalInput = Input.GetAxisRaw ( "Vertical" );
        brakeInput = Input.GetKey ( KeyCode.Space );

    }

    void CarPhysicsUpdate ()
    {
        localRightDirection = localTransform.right;

        velocity = rigidbody.velocity;
        flatYVelocity = new Vector3 ( velocity.x, 0, velocity.z );

        localForwardDirection = transform.TransformDirection ( globalForwardDirection );

        tempVEC = new Vector3 ( localForwardDirection.x, 0, localForwardDirection.z );
        flatYLocalForwardDirection = Vector3.Normalize ( tempVEC );

        relativeVelocity = localTransform.InverseTransformDirection ( flatYVelocity );

        slideSpeed = Vector3.Dot ( localRightDirection, flatYVelocity );

        currentSpeed = flatYVelocity.magnitude;

        movementDirectionModifier = Mathf.Sign ( Vector3.Dot ( flatYVelocity, flatYLocalForwardDirection ) );

        engineForce = (flatYLocalForwardDirection * (power * verticalInput) * carMass);

        if (movementDirectionModifier < -0.1f && verticalInput < 0)
            currentTurningAmount = -horizontalInput;
        else
            currentTurningAmount = horizontalInput;

        turnDirection = (((localUpDirection * turnSpeed) * currentTurningAmount) * carMass) * 800;

        actualGrip = Mathf.Lerp ( 100, carGrip, currentSpeed * 0.02f );
        simulatedGripForce = localRightDirection * (-slideSpeed * carMass * actualGrip);
    }

    void SlowVelocity ()
    {
        rigidbody.AddForce ( -flatYVelocity * 0.8f );
    }

    void EngineSound ()
    {

        if (audio)
        {
            audio.pitch = 0.30f + currentSpeed * 0.025f;

            if (currentSpeed > 30)
            {
                audio.pitch = 0.25f + currentSpeed * 0.015f;
            }
            if (currentSpeed > 40)
            {
                audio.pitch = 0.20f + currentSpeed * 0.013f;
            }
            if (currentSpeed > 49)
            {
                audio.pitch = 0.15f + currentSpeed * 0.011f;
            }
            //ensures we dont exceed to crazy of a pitch by resetting it back to default 2
            if (audio.pitch > 2.0f)
            {
                audio.pitch = 2.0f;
            }
        }
    }

    void FixedUpdate ()
    {

        DoEngine ();
        DoTurn ();

        // apply forces to our rigidbody for grip
        rigidbody.AddForce ( simulatedGripForce * Time.deltaTime );
    }

    private void DoEngine ()
    {
        if (brakeInput)
        {
            //carRigidbody.AddForce ( -carRigidbody.velocity * 5 );
            rigidbody.drag = 0.5f;
            return;
        }

        rigidbody.drag = 0.2f;

        if (currentSpeed < maxSpeed)
        {
            // apply the engine force to the rigidbody
            rigidbody.AddForce ( engineForce * Time.deltaTime );
        }
    }

    private void DoTurn ()
    {
        turnSpeedModifier = 1.0f;

        if (turnDirection.y < 0 && rigidbody.angularVelocity.y > 0)
        {
            turnSpeedModifier = Mathf.Lerp ( 1, inverseTurnModifier, Mathf.InverseLerp ( 0.0f, inverseMaxAngularSpeed, rigidbody.angularVelocity.y ) );
        }
        else if (turnDirection.y > 0 && rigidbody.angularVelocity.y < 0)
        {
            turnSpeedModifier = Mathf.Lerp ( 1, inverseTurnModifier, Mathf.InverseLerp ( 0.0f, -inverseMaxAngularSpeed, rigidbody.angularVelocity.y ) );
        }

        rigidbody.AddTorque ( turnDirection * Time.deltaTime * (turnSpeedModifier) );
    }
}