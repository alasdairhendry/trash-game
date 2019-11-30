using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public Character character { get; protected set; }
    public CharacterIK characterIK { get; protected set; }
    public Animator animator { get; protected set; }

    [SerializeField] private float locomotionLerp = 10.0f;
    [SerializeField] private float runningStanceLerp = 2.5f;
    [SerializeField] private float fightStanceLerp = 5.0f;
    [SerializeField] private float crouchStanceLerp = 5.0f;

    private void Awake ()
    {
        character = GetComponent<Character> ();
        characterIK = GetComponent<CharacterIK> ();
        animator = GetComponent<Animator> ();
    }

    private void Update ()
    {
        if (character.currentState == Character.State.Driving || character.currentState == Character.State.Falling)
        {
            animator.SetFloat ( "forward", Mathf.Lerp ( animator.GetFloat ( "forward" ), 0.0f, Time.deltaTime * locomotionLerp ) );
            animator.SetFloat ( "sideway", Mathf.Lerp ( animator.GetFloat ( "sideway" ), 0.0f, Time.deltaTime * locomotionLerp ) );
            animator.SetFloat ( "running", Mathf.Lerp ( animator.GetFloat ( "running" ), 0.0f, Time.deltaTime * locomotionLerp ) );
            return;
        }


        if (character.shouldJump)
        {
            animator.SetTrigger ( "jump" );
            animator.ResetTrigger ( "jump" );
            character.shouldJump = false;
        }

        float fwd = (character.cInput.input.magnitude);
        if(character.IsAiming)
        {
            animator.SetFloat ( "forward", Mathf.Lerp ( animator.GetFloat ( "forward" ), character.cInput.input.y, Time.deltaTime * locomotionLerp ) );
            animator.SetFloat ( "sideway", Mathf.Lerp ( animator.GetFloat ( "sideway" ), character.cInput.input.x, Time.deltaTime * locomotionLerp ) );
        }
        else
        {
            animator.SetFloat ( "forward", Mathf.Lerp ( animator.GetFloat ( "forward" ), fwd, Time.deltaTime * locomotionLerp ) );
            animator.SetFloat ( "sideway", Mathf.Lerp ( animator.GetFloat ( "sideway" ), fwd, Time.deltaTime * locomotionLerp ) );
        }
       
        animator.SetFloat ( "running", Mathf.Lerp ( animator.GetFloat ( "running" ), character.isRunning ? 1.0f : 0.0f, Time.deltaTime * runningStanceLerp ) );

        animator.SetFloat ( "fightstance", Mathf.Lerp ( animator.GetFloat ( "fightstance" ), character.IsAiming ? 1.0f : 0.0f, Time.deltaTime * fightStanceLerp ) );
        animator.SetFloat ( "crouchstance", Mathf.Lerp ( animator.GetFloat ( "crouchstance" ), character.isCrouching ? 1.0f : 0.0f, Time.deltaTime * crouchStanceLerp ) );
        animator.SetBool ( "weaponEquipped", character.cWeapon.isEquipped && !character.cWeapon.isHolstered );
        animator.SetBool ( "isPistol", character.cWeapon.weaponIsPistol );
    }

    public void OnCompleteAnimation_GrabMain ()
    {
        characterIK.SetRightHand ( character.cDrag.target.cDrag.dragIKTarget );
    }
}
