using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMatchBehaviour : StateMachineBehaviour
{
    public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterTargetMatch ctm = animator.GetComponent<CharacterTargetMatch> ();
        ctm.GetComponent<Collider> ().isTrigger = true;
        ctm.GetComponent<Rigidbody> ().useGravity = false;
    }

    override public void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterTargetMatch ctm = animator.GetComponent<CharacterTargetMatch> ();
        CharacterTargetMatch.MatchData data = ctm.CurrentData ( stateInfo.normalizedTime );
        if (data == null) return;
        animator.MatchTarget ( data.targetPosition, data.targetRotation, data.targetBodyPath, new MatchTargetWeightMask ( data.positionWeight, data.rotationWeight ), data.start, data.end );
    }

    override public void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterTargetMatch ctm = animator.GetComponent<CharacterTargetMatch> ();
        ctm.GetComponent<Collider> ().isTrigger = false;
        ctm.GetComponent<Rigidbody> ().useGravity = true;
    }
}
