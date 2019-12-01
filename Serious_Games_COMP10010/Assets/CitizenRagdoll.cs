using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CitizenRagdoll : MonoBehaviour
{
    [SerializeField] private Animator cityRagdollAnimator;
    [SerializeField] private Animator townRagdollAnimator;
    [SerializeField] private GameObject bloodParticles;
    [SerializeField] private Transform bloodSmear;
    [SerializeField] private List<Transform> cityRagdollBones = new List<Transform> ();
    private bool isDead = false;

    public void Die (Citizen.CitizenType type, GameObject activeObject)
    {
        if (isDead) return;
        isDead = true;

        Animator anim;
        switch (type)
        {
            case Citizen.CitizenType.City:
                anim = cityRagdollAnimator;
                break;
            case Citizen.CitizenType.Town:
                anim = townRagdollAnimator;
                break;
            default:
                anim = cityRagdollAnimator;
                break;
        }

        BipedRagdollReferences r = BipedRagdollReferences.FromAvatar ( anim );
        BipedRagdollCreator.Options options = BipedRagdollCreator.AutodetectOptions ( r );
        options.weight = 75.0f;
        options.colliderLengthOverlap = -.2f;
        options.jointRange = 1.25f;
        BipedRagdollCreator.Create ( r, options );
        activeObject.GetComponent<SkinnedMeshRenderer> ().updateWhenOffscreen = true;
        bloodParticles.SetActive ( true );

        bloodSmear.transform.SetParent ( null );
        bloodSmear.transform.position = new Vector3 ( transform.position.x, 0.02f, transform.position.z );
        bloodSmear.localScale = new Vector3 ( Random.Range ( 1.0f, 4.0f ), Random.Range ( 2.0f, 6.0f ), 1.0f );
        bloodSmear.gameObject.SetActive ( true );
    }
}
