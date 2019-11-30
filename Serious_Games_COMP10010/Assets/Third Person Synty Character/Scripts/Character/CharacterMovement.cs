using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterMovement : MonoBehaviour
{
    public Character character { get; protected set; }

    [SerializeField] private CharacterMovementData movementData;

    private Vector3 lastMousePosition = new Vector3 ();

    public float GetCurrentSpeed
    {
        get
        {
            return 0.0f;
            //if (character.currentState == Character.State.Aiming)
            //    return movementData.walkSpeed * movementData.aimModifier;

            //if (character.currentState == Character.State.Crouching)
            //    return movementData.walkSpeed * movementData.crouchModifier;

            if (character.currentState == Character.State.Standing)
                return character.isRunning ? movementData.walkSpeed * movementData.runModifier : movementData.walkSpeed;

            return 0.0f;
        }
    }

    private void Awake ()
    {
        character = GetComponent<Character> ();
        lastMousePosition = Input.mousePosition;
    }

    public Transform jumpTarget;
    [SerializeField] private bool doJumpTarget = false;

    public void OnUpdate ()
    {
        if (Input.GetKeyDown ( KeyCode.Space ))
        {
            if (doJumpTarget)
            {
                character.GetComponent<Animator> ().SetTrigger ( "jump-to-climb" );
                character.cTargetMatch.SetData
                (
                    new CharacterTargetMatch.MatchData ( jumpTarget.position, jumpTarget.rotation, AvatarTarget.LeftHand, Vector3.one, 1.0f, 0.133f, 0.421f ),
                    new CharacterTargetMatch.MatchData ( jumpTarget.position, jumpTarget.rotation, AvatarTarget.LeftFoot, Vector3.one, 0.0f, 0.519f, 0.890f )
                );
            }
            else
            {
                character.GetComponent<Animator> ().SetTrigger ( "jump" );
            }
        }
    }

    public void OnFixedUpdate ()
    {
        if (character.cDrag.isBeingDragged) return;
        Movement ();
        Rotation ();
    }

    private void Movement ()
    {
        //character.rigidbody.AddForce ( character.rawInput.x * transform.right * GetCurrentSpeed, ForceMode.VelocityChange );
        //character.rigidbody.AddForce ( Mathf.Abs( character.rawInput.magnitude ) * transform.forward * GetCurrentSpeed, ForceMode.VelocityChange );
    }

    private void Rotation ()
    {
        Vector3 moveDirection = new Vector3 ( 0.0f, 0.0f, 0.0f );

        if (character.IsAiming)
            moveDirection += character.cCameraController.LookTransform.forward;
        else
            moveDirection += character.cCameraController.LookTransform.forward * character.cInput.rawInput.y;

        if (!character.IsAiming)
            moveDirection += character.cCameraController.LookTransform.right * character.cInput.rawInput.x;

        moveDirection.Normalize ();

        if (moveDirection == Vector3.zero) return;

        Quaternion lookDirection = Quaternion.LookRotation ( moveDirection );
        Quaternion targetRotation = Quaternion.Slerp ( transform.rotation, lookDirection, movementData.turnSpeed * Time.deltaTime );
        transform.rotation = targetRotation;
    }
}

public static class Extensions
{
    public static bool IsEpsilon(this Vector3 value, Vector3 target, float epsilon )
    {
        if(Vector3.SqrMagnitude(value - target) < epsilon)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static float Map (this float value, float inputFrom, float inputTo, float outputFrom, float outputTo)
    {
        return (value - inputFrom) / (inputTo - inputFrom) * (outputTo - outputFrom) + outputFrom;
    }

    public static float Map (this int value, int inputFrom, int inputTo, float outputFrom, float outputTo)
    {
        return ((float)value - (float)inputFrom) / ((float)inputTo - (float)inputFrom) * (outputTo - outputFrom) + outputFrom;
    }

}
