
// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;
using System;

public class PlayerCar : MonoBehaviour {

	// car physics calculations/input stuff
	public Vector3 accel;
	public Transform playerTransform;
	public float verticalInput { get; protected set; }
	public float horizontalInput { get; protected set; } //horizontal input control, either mobile control or keyboard
	public bool brakeInput { get; protected set; }
	public bool isFollowing { get; protected set; }
    public float GetNormalisedSpeed
    {
        get
        {
            return Mathf.InverseLerp ( 0.0f, maxSpeed, mySpeed );
        }
    }

	private float deadZone = .001f;
	private Vector3 myRight;
	private Vector3 velo;
	private Vector3 flatVelo;
	private Vector3 relativeVelocity;
	private Vector3 dir;
	private Vector3 flatDir;
	private Vector3 carUp;
	private Transform carTransform;
	private Rigidbody carRigidbody;
	private Vector3 engineForce;

	private Vector3 turnVec;
	private Vector3 imp;
	private float rev;
	private float actualTurn;
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
	public float mySpeed;

	private Vector3 carRight;
	private Vector3 carFwd;
	private Vector3 tempVEC; 

	private float xr = 0;

	private AudioSource audio;

	void  Start (){
		Initialize();
	}

	void  Initialize (){   
		// Cache a reference to our car's transform
		carTransform = transform;
		// cache the rigidbody for our car
		carRigidbody = GetComponent<Rigidbody>();
		// cache our vector up direction
		carUp = carTransform.up;
		// cache the mass of our vehicle
		carMass = GetComponent<Rigidbody>().mass;
		// cache the Forward World Vector for our car
		carFwd = Vector3.forward;
		// cache the World Right Vector for our car
		carRight = Vector3.right;
		// call to set up our wheels array
		setUpWheels();
		// we set a COG here and lower the center of mass to a
		//negative value in Y axis to prevent car from flipping over
		carRigidbody.centerOfMass = new Vector3(0f,-1.7f,.35f); 
		audio = GetComponent<AudioSource> ();
		frontLeftRayCast = frontLeftWheel.GetComponentInParent <RaycastWheelSimple> ();
		frontRightRayCast = frontRightWheel.GetComponentInParent <RaycastWheelSimple> ();

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

        GUI.Box ( new Rect ( 0.0f, 0.0f, Mathf.Lerp ( 0.0f, 1.0f, Mathf.InverseLerp ( 1.0f, inverseTurnModifier, modifier ) ) * 512, 64 ), "" );
    }

    void  Update (){
		// call the function to start processing all vehicle physics
		carPhysicsUpdate();

	
		//call the function to see what input we are using and apply it
		checkInput();


        if (Input.GetKeyDown ( KeyCode.E ))
        {
            if (isFollowing)
            {
                // not in trucks

                playerTransform.GetComponent<CharacterPhysics> ().SetGravity ( false );
                playerTransform.GetComponent<Rigidbody> ().detectCollisions = false;
                playerTransform.gameObject.AddComponent<FixedJoint> ().connectedBody = GetComponent<Rigidbody> ();
                playerTransform.gameObject.GetComponent<FixedJoint> ().connectedMassScale = 0.0f;
                playerTransform.gameObject.GetComponent<FixedJoint> ().massScale = 0.0f;
                playerTransform.SetParent ( this.transform );
                playerTransform.localPosition = new Vector3 ( -0.75f, 1.0f, 2.0f );
                isFollowing = false;
                ReceiveInput ( Vector2.zero, false );
                mainCamera.SetActive ( true );
            }
            else
            {
                // player in truck

                playerTransform.GetComponent<CharacterPhysics> ().SetGravity ( true );
                playerTransform.localPosition = new Vector3 ( -2.0f, 0.0f, 0.0f );
                playerTransform.GetComponent<Rigidbody> ().detectCollisions = true;
                Destroy ( playerTransform.gameObject.GetComponent<FixedJoint> () );
                playerTransform.SetParent ( null );
                isFollowing = true;
                ReceiveInput ( Vector2.zero, false );
                mainCamera.SetActive ( false );
            }
        }
	}

	void  LateUpdate (){
		// this function makes the visual 3d wheels rotate and turn
		rotateVisualWheels();

		//this is where we send to a function to do engine sounds
		engineSound();
	}

	void  setUpWheels (){   
		if((null == frontLeftWheel || null == frontRightWheel || null == rearLeftWheel || null == rearRightWheel ))
		{
			Debug.LogError("One or more of the wheel transforms have not been plugged in on the car");
			Debug.Break();
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

	private Vector3 rotationAmount;

	void  rotateVisualWheels (){    
		// front wheels visual rotation while steering the car

		xr += (float)(relativeVelocity.z * 1.6 * Time.deltaTime * Mathf.Rad2Deg);

		for (int i = 0; i < 4; i++) {
			float yr = 0;

			if (wheelTransform [i] == RFWheelTransform || wheelTransform [i] == LFWheelTransform) 
				yr = horizontalInput * 30;

				wheelTransform[i].localRotation = Quaternion.Euler(new Vector3 (xr, yr, 0));
		}
		/*
		Vector3 rotationAmount = carRight *(float)(relativeVelocity.z * 1.6 * Time.deltaTime * Mathf.Rad2Deg);

		wheelTransform[0].Rotate(rotationAmount);
		wheelTransform[1].Rotate(rotationAmount);
		wheelTransform[2].Rotate(rotationAmount);
		wheelTransform[3].Rotate(rotationAmount);
		*/

		if (xr >= 360f)
			xr -= 360f;
		
	}

	private float deviceAccelerometerSensitivity = 2; //how sensitive our mobile accelerometer will be
   [SerializeField]  private GameObject mainCamera;

    void  checkInput (){   
		
	
		//Use the Keyboard for all car input
		horizontalInput = Input.GetAxisRaw("Horizontal");
		verticalInput = Input.GetAxisRaw ( "Vertical");
        brakeInput = Input.GetKey ( KeyCode.Space );

	} 

	void  carPhysicsUpdate (){



        //grab all the physics info we need to calc everything
        myRight = carTransform.right;

		// find our velocity
		Vector3 velo = carRigidbody.velocity;

		Vector3 tempVEC = new Vector3(velo.x,0,velo.z);

		// figure out our velocity without y movement - our flat velocity
		flatVelo = tempVEC;

		// find out which direction we are moving in
		dir = transform.TransformDirection(carFwd);

		tempVEC = new Vector3(dir.x,0,dir.z);

		// calculate our direction, removing y movement - our flat direction
		flatDir = Vector3.Normalize(tempVEC);

		// calculate relative velocity
		relativeVelocity = carTransform.InverseTransformDirection(flatVelo);

		// calculate how much we are sliding (find out movement along our x axis)
		slideSpeed = Vector3.Dot(myRight,flatVelo);

		// calculate current speed (the magnitude of the flat velocity)
		mySpeed = flatVelo.magnitude;

		// check to see if we are moving in reverse
		rev = Mathf.Sign(Vector3.Dot(flatVelo,flatDir));

		// calculate engine force with our flat direction vector and acceleration
		engineForce = ( flatDir * ( power * verticalInput ) * carMass);

		// do turning
		actualTurn = horizontalInput;

		// if we're in reverse, we reverse the turning direction too
		if(rev < -0.1f && verticalInput < 0)
		{
			actualTurn =- actualTurn;
		}

		// calculate torque for applying to our rigidbody
		turnVec =((( carUp * turnSpeed ) * actualTurn ) * carMass )* 800;

		// calculate impulses to simulate grip by taking our right vector, reversing the slidespeed and
		// multiplying that by our mass, to give us a completely 'corrected' force that would completely
		// stop sliding. we then multiply that by our grip amount (which is, technically, a slide amount) which
		// reduces the corrected force so that it only helps to reduce sliding rather than completely
		// stop it 

		actualGrip = Mathf.Lerp(100, carGrip, mySpeed * 0.02f);
		imp = myRight * ( -slideSpeed * carMass * actualGrip);
	}

	void  slowVelocity (){
		carRigidbody.AddForce(-flatVelo * 0.8f);
	}

	//this controls the sound of the engine audio by adjusting the pitch of our sound file
	void  engineSound (){

		if (audio) 
		{
			audio.pitch = 0.30f + mySpeed * 0.025f;

			if (mySpeed > 30) 
			{
				audio.pitch = 0.25f + mySpeed * 0.015f;
			}
			if (mySpeed > 40) 
			{
				audio.pitch = 0.20f + mySpeed * 0.013f;
			}
			if (mySpeed > 49) 
			{
				audio.pitch = 0.15f + mySpeed * 0.011f;
			}
			//ensures we dont exceed to crazy of a pitch by resetting it back to default 2
			if (audio.pitch > 2.0f) {
				audio.pitch = 2.0f;
			}
		}
	}

	void  FixedUpdate (){

        DoEngine ();
        DoTurn ();

		// apply forces to our rigidbody for grip
		carRigidbody.AddForce( imp  * Time.deltaTime );
	}

    private void DoEngine ()
    {
        if (brakeInput)
        {
            //carRigidbody.AddForce ( -carRigidbody.velocity * 5 );
            carRigidbody.drag = 0.5f;
            return;
        }

        carRigidbody.drag = 0.2f;

        if (mySpeed < maxSpeed)
        {
            // apply the engine force to the rigidbody
            carRigidbody.AddForce ( engineForce * Time.deltaTime );
        }
    }

    float modifier = 1.0f;
    private void DoTurn ()
    {
        //if we're going to slow to allow kart to rotate around
        //if (mySpeed > maxSpeedToTurn)
        //{
        // apply torque to our rigidbody
        //Debug.Log ( );
         modifier = 1.0f;

        if(turnVec.y < 0 && carRigidbody.angularVelocity.y > 0)
        {
            modifier = Mathf.Lerp ( 1, inverseTurnModifier, Mathf.InverseLerp ( 0.0f, inverseMaxAngularSpeed, carRigidbody.angularVelocity.y ) );
        }
        else if (turnVec.y > 0 && carRigidbody.angularVelocity.y < 0)
        {
            modifier = Mathf.Lerp ( 1, inverseTurnModifier, Mathf.InverseLerp ( 0.0f, -inverseMaxAngularSpeed, carRigidbody.angularVelocity.y ) );
        }

        carRigidbody.AddTorque ( turnVec * Time.deltaTime * (modifier) );
        //}
        //else if (mySpeed < maxSpeedToTurn)
        //{
            //return;
        //}
    }
}