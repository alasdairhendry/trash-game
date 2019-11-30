using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent ( typeof ( Rigidbody ) )]
[RequireComponent ( typeof ( CharacterMovement ) )]
public class Character : MonoBehaviour
{
    public enum State { Standing, Falling, Driving }
    public State currentState { get; protected set; } = State.Standing;

    [SerializeField] private float standingDrag = 6.0f;
    [SerializeField] private float fallingDrag = 0.0f;

    private bool canAim = true;
    private bool isAiming = false;

    public bool IsAiming
    {
        get
        {
            if (!canAim)
            {
                if (isAiming)
                {
                    isAiming = false;
                    OnAimChanged ();
                }
            }
            else
            {
                if (cInput.isAimInput || cWeapon.ShouldAim)
                {
                    if (!isAiming)
                    {
                        isAiming = true;
                        OnAimChanged ();
                    }
                }
                else if(!cInput.isAimInput && !cWeapon.ShouldAim)
                {
                    if (isAiming)
                    {
                        isAiming = false;
                        OnAimChanged ();
                    }
                }
            }

            return isAiming;
        }
    }

    public bool isCrouching
    {
        get
        {
            if (isCrouchingInput)
            {
                return true;
            }

            if (!isCrouchingInput && !canStand)
            {
                return true;
            }

            return false;
        }
    }
    public bool isRunning { get; set; }
    public bool shouldJump { get; set; }

    public bool canStand { get; set; }
    public bool isCrouchingInput { get; set; }

    public System.Action OnAimChanged;

    public new Rigidbody rigidbody { get; protected set; }
    public CharacterMovement cMovement { get; protected set; }
    public CharacterIK cIK { get; protected set; }
    public CharacterDrag cDrag { get; protected set; }
    public CharacterPhysics cPhysics { get; protected set; }
    public CharacterInput cInput { get; protected set; }
    public CharacterWeapon cWeapon { get; protected set; }
    public CharacterTargetMatch cTargetMatch { get; protected set; }
    public PlayerCameraController cCameraController { get; protected set; }
    public bool isGrounded { get; set; } = false;

    public new Collider collider;

    [SerializeField] private LayerMask walkableLayers;
    [SerializeField] private float stepHeight = 0.6f;
    [SerializeField] private float standHeight = 0.6f;
    [SerializeField] private bool isAI = false;
    [SerializeField] private GameObject cameraPrefab;

    public bool IsAI { get { return isAI; } }

    private void Awake ()
    {
        rigidbody = GetComponent<Rigidbody> ();
        cMovement = GetComponent<CharacterMovement> ();
        cPhysics = GetComponent<CharacterPhysics> ();
        cIK = GetComponent<CharacterIK> ();
        cDrag = GetComponent<CharacterDrag> ();
        cInput = GetComponent<CharacterInput> ();
        cWeapon = GetComponent<CharacterWeapon> ();
        cTargetMatch = GetComponent<CharacterTargetMatch> ();

        if (FindObjectOfType<PlayerCameraController> () == null)
            cCameraController = Instantiate ( cameraPrefab ).GetComponent<PlayerCameraController> ().SetTarget ( this.transform );
        else cCameraController = FindObjectOfType<PlayerCameraController> ().SetTarget ( this.transform );
    }

    private void Update ()
    {
        CheckStateUpdate ();

        if (isAI) return;

        if (IsAiming)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            if (Input.GetKeyDown ( KeyCode.L ))
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }       

        if (isCrouching)
        {
            GetComponent<CapsuleCollider> ().center = new Vector3 ( 0.0f, 0.75f, 0.0f );
            GetComponent<CapsuleCollider> ().height = 1.0f;
        }
        else
        {
            GetComponent<CapsuleCollider> ().center = new Vector3 ( 0.0f, 1.1f, 0.0f );
            GetComponent<CapsuleCollider> ().height = 1.4f;
        }
    }

    private void FixedUpdate ()
    {
        CheckStateFixed ();
    }

    private void CheckStateUpdate ()
    {
        switch (currentState)
        {
            case State.Standing:
                cMovement.OnUpdate ();
                cDrag.OnUpdate ();
                isGrounded = CheckIsGrounded ();
                CheckCanStand ();
                rigidbody.drag = standingDrag;
                break;
            case State.Falling:
                cMovement.OnUpdate ();
                isGrounded = CheckIsGrounded ();
                rigidbody.drag = fallingDrag;
                break;
            case State.Driving:
  
                break;
            default:
                break;
        }
    }

    private void CheckStateFixed ()
    {
        switch (currentState)
        {
            case State.Standing:
                cMovement.OnFixedUpdate ();
                cDrag.OnFixedUpdate ();
                break;
            case State.Falling:
                cMovement.OnFixedUpdate ();
                break;
            case State.Driving:

                break;
            default:
                break;
        }
    }    

    public void SetInput (Vector2 value)
    {
        cInput.input = value;
        cInput.rawInput = value;
    }

    public void SetCurrentState (State state)
    {
        currentState = state;
        OnStateChanged ();
    }

    private void OnStateChanged ()
    {
        switch (currentState)
        {
            case State.Standing:
                canAim = true;
                break;
            case State.Falling:
                canAim = false;
                break;
            case State.Driving:
                canAim = false;
                if (!cWeapon.isHolstered)
                    cWeapon.SetHolsterState ();
                break;
        }
    }

    private bool CheckIsGrounded ()
    {
        Vector3 origin = transform.position;
        Vector3[] origins = new Vector3[5] { transform.position, transform.position + transform.forward * 0.25f, transform.position + transform.right * 0.25f, transform.position - transform.forward * 0.25f, transform.position - transform.right * 0.25f };
        origin.y += stepHeight;

        Vector3 dir = -Vector3.up;
        float dist = stepHeight + 0.1f;

        RaycastHit hit;

        for (int i = 0; i < origins.Length; i++)
        {
            origins[i].y += stepHeight;

            Debug.DrawRay ( origins[i], dir * dist, Color.red );

            if (Physics.Raycast ( origins[i], dir, out hit, dist, walkableLayers ))
            {
                Vector3 targetPosition = hit.point;

                transform.position = new Vector3 ( transform.position.x, targetPosition.y, transform.position.z );

                SetCurrentState ( Character.State.Standing );
                if (GetComponent<Animator> ().GetBool ( "falling" ))
                    GetComponent<Animator> ().SetBool ( "falling", false );
                return true;
            }
        }      

        if (Physics.Raycast ( transform.position, dir, out hit, Mathf.Infinity, walkableLayers ))
        {
            float distanceToGround = Vector3.Distance ( transform.position, hit.point );

            if (distanceToGround > 1.0f)
            {
                if (!GetComponent<Animator> ().GetBool ( "falling" ))
                    GetComponent<Animator> ().SetBool ( "falling", true );
            }

            if (distanceToGround < 1.0f)
            {
                if (rigidbody.velocity.magnitude > 7.5f)
                    GetComponent<Animator> ().SetTrigger ( "softLand" );
            }
        }

        if (cDrag.isDragging) cDrag.OnEndDrag ();
        if (cDrag.isBeingDragged) cDrag.OnEndDragged ();
        SetCurrentState ( Character.State.Falling );
        Debug.Log ( "is NOT grounded" );
        return false;
    }

    private bool CheckCanStand ()
    {
        Vector3 origin = transform.position;

        Vector2 inputDir = cInput.rawInput;

        //origin += transform.forward * inputDir.y;
        //origin += transform.right * inputDir.x;

        if (canStand)
        {

        }
        else
        {
            //origin -= transform.forward * inputDir.y;
            //origin -= transform.right * inputDir.x;
        }

        origin.y += 0.5f;

        Vector3 dir = Vector3.up;

        Ray ray = new Ray ( origin, dir );
        RaycastHit hit;

        if (Physics.Raycast ( ray, out hit, standHeight - 0.5f, walkableLayers ))
        {
            if (canStand)
            {
                //isCrouching = true;
            }

            if (hit.distance > 1.5f - 0.5f)
                canStand = false;
            else
            {
                canStand = true;
            }
        }
        else
        {
            if (!canStand)
            {
                //isCrouching = false;
                //Debug.Log ( "hello" );
            }

            canStand = true;
        }

        return canStand;
    }
}
