using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLocomotionDraggedBehaviour : StateMachineBehaviour
{
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Character c = animator.gameObject.GetComponent<Character> ();
        c.cDrag.isInDraggedState = true;
        c.cPhysics.SetMass ( 1.0f );
        c.cPhysics.SetConstraints ( RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ );
        
        FixedJoint fj = animator.gameObject.AddComponent<FixedJoint> ();
        fj.connectedBody = animator.gameObject.GetComponent<CharacterDrag> ().target.rigidbody;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Character c = animator.gameObject.GetComponent<Character> ();
        c.cDrag.isInDraggedState = false;
        c.cPhysics.ResetMass ();
        c.cPhysics.ResetConstraints ();

        Destroy ( animator.gameObject.GetComponent<FixedJoint> () );
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
