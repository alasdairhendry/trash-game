using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIK : MonoBehaviour
{
    public Character character { get; protected set; }
    protected Animator animator;

    public bool isActiveRightHand = false;    
    public Transform rightHandTarget = null;
    public float rightHandTargetWeight = 0.0f;
    public float rightHandCurrentWeight = 0.0f;
    public float rightHandWeightDamping = 7.5f;
    [Space]
    public bool isActiveLeftHand = false;
    public Transform leftHandTarget = null;
    public float leftHandTargetWeight = 0.0f;
    public float leftHandCurrentWeight = 0.0f;
    public float leftHandWeightDamping = 7.5f;
    [Space]
    public Vector3 leftHandTargetPosition = new Vector3 ();
    public Quaternion leftHandTargetRotation = new Quaternion ();
    [Space]
    public Transform lookTarget = null;
    public Transform rightHandRootPivot;
    public Transform rightHandPivot;
    [Space]
    public bool debugRightIK = false;

    private void Awake ()
    {
        character = GetComponent<Character> ();
        character.cWeapon.onWeaponEquiped += OnWeaponEquipped;
        character.cWeapon.onWeaponUnequiped += OnWeaponUnequipped;
        character.cWeapon.onWeaponHolstered += OnWeaponHolstered;
        character.cWeapon.onWeaponUnholstered += OnWeaponUnholstered;

        character.OnAimChanged += OnAimChanged;
    }

    void Start ()
    {
        animator = GetComponent<Animator> ();
    }

    void OnAnimatorIK ()
    {
        SetLeftHandPosition ();
        RightHandIK ();
        LeftHandIK ();
        SetLookAtPosition ();
    }

    private float recoilCounter = 0.0f;
    public RecoilData recoilData = null;
    public AudioSource src;

    private void Update ()
    {
        MatchWeaponIK ();
        HandleDamping ();
        WeaponAimPosition ();
        MonitorRecoil ();
    }

    private void MonitorRecoil ()
    {
        if (recoilCounter <= 1.0f)
        {
            recoilCounter += Time.deltaTime * recoilData.time;

            if (recoilCounter > 1.0f) recoilCounter = 1.0f;
        }

        float lerp = 10.0f;
        float isEmptyClipMultiplier = (character.cWeapon.currentRounds > 0) ? 1.0f : 0.5f;

        rightHandRootPivot.localRotation = Quaternion.Slerp ( rightHandRootPivot.localRotation, Quaternion.Euler ( recoilData.rotationalDirection * recoilData.rotationalCurve.Evaluate ( recoilCounter ) * recoilData.visualMultiplier * isEmptyClipMultiplier ), Time.deltaTime * lerp );
        rightHandRootPivot.localPosition = Vector3.Lerp ( rightHandRootPivot.localPosition, recoilData.positionalDirection * recoilData.positionalCurve.Evaluate ( recoilCounter ) * recoilData.visualMultiplier * isEmptyClipMultiplier, Time.deltaTime * lerp );

        FindObjectOfType<PlayerCameraController> ().SetXRecoil ( recoilData.cameraXAmount * recoilData.cameraXCurve.Evaluate ( recoilCounter ) * UnityEngine.Random.Range ( recoilData.rangeXModifier.x, recoilData.rangeXModifier.y ) * recoilData.cameraMultiplier * isEmptyClipMultiplier );
        FindObjectOfType<PlayerCameraController> ().SetYRecoil ( recoilData.cameraYAmount * recoilData.cameraYCurve.Evaluate ( recoilCounter ) * UnityEngine.Random.Range ( recoilData.rangeYModifier.x, recoilData.rangeYModifier.y ) * recoilData.cameraMultiplier * isEmptyClipMultiplier );
    }

    private void HandleDamping ()
    {
        if (!isActiveLeftHand)
        {
            leftHandTargetWeight = 0.0f;
        }

        if (!isActiveRightHand)
        {
            rightHandTargetWeight = 0.0f;
        }

        leftHandCurrentWeight = Mathf.Lerp ( leftHandCurrentWeight, leftHandTargetWeight, leftHandWeightDamping * Time.deltaTime );
        rightHandCurrentWeight = Mathf.Lerp ( rightHandCurrentWeight, rightHandTargetWeight, rightHandWeightDamping * Time.deltaTime );
    }

    private void WeaponAimPosition ()
    {
        Vector3 dir = (character.cWeapon.AimPosition - rightHandPivot.transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation ( dir, Vector3.Cross ( dir, -Vector3.up ).normalized );
        rightHandPivot.rotation = lookRot;
    }

    private void OnWeaponHolstered (WeaponData obj)
    {
        isActiveLeftHand = false;
        isActiveRightHand = false;
    }

    private void OnWeaponUnholstered (WeaponData obj)
    {
        if (character.cWeapon.currentWeaponData.weaponType == WeaponData.WeaponType.Rifle)
        {
            isActiveLeftHand = true;
            leftHandTargetWeight = 1.0f;
        }
    }

    private void OnWeaponEquipped (WeaponData data)
    {
        if (character.cWeapon.currentWeaponData.weaponType == WeaponData.WeaponType.Rifle && !character.cWeapon.isHolstered)
        {
            isActiveLeftHand = true;
            leftHandTargetWeight = 1.0f;
        }
    }

    private void OnWeaponUnequipped (WeaponData data)
    {
        isActiveLeftHand = false;
        isActiveRightHand = false;
    }

    private void OnAimChanged()
    {
        isActiveRightHand = false;

        if (!character.IsAiming)
        {
            if (character.cWeapon.isEquipped && character.cWeapon.currentWeaponData.weaponType == WeaponData.WeaponType.Pistol)
            {
                isActiveLeftHand = false;
            }
        }
    }

    private void SetLeftHandPosition ()
    {
        leftHandTarget.position = leftHandTargetPosition;
        leftHandTarget.rotation = leftHandTargetRotation;
    }

    private void RightHandIK ()
    {
        if (animator)
        {
            if (rightHandTarget != null)
            {
                animator.SetIKPositionWeight ( AvatarIKGoal.RightHand, rightHandCurrentWeight );
                animator.SetIKRotationWeight ( AvatarIKGoal.RightHand, rightHandCurrentWeight );
                animator.SetIKPosition ( AvatarIKGoal.RightHand, rightHandTarget.position );
                animator.SetIKRotation ( AvatarIKGoal.RightHand, rightHandTarget.rotation );
            }
        }
    }

    private void LeftHandIK ()
    {
        if (animator)
        {
            if (leftHandTarget != null)
            {
                animator.SetIKPositionWeight ( AvatarIKGoal.LeftHand, leftHandCurrentWeight );
                animator.SetIKRotationWeight ( AvatarIKGoal.LeftHand, leftHandCurrentWeight );
                animator.SetIKPosition ( AvatarIKGoal.LeftHand, leftHandTarget.position );
                animator.SetIKRotation ( AvatarIKGoal.LeftHand, leftHandTarget.rotation );
            }
        }
    }

    private void SetLookAtPosition ()
    {
        animator.SetLookAtPosition ( character.cCameraController.cameraTransform.position + character.cCameraController.cameraTransform.forward * 100.0f );
        animator.SetLookAtWeight ( 1.0f, character.IsAiming ? 1.0f : 0.0f, 1.0f, 1.0f, 1.0f );
    }

    private void MatchWeaponIK ()
    {
        if (character.cWeapon.isEquipped && !character.cWeapon.isHolstered)
        {
            if (character.cWeapon.currentWeaponData.weaponType == WeaponData.WeaponType.Rifle)
            {
                leftHandTargetPosition = character.cWeapon.WeaponHandTarget.position;
                leftHandTargetRotation = character.cWeapon.WeaponHandTarget.rotation;
            }

            if (character.IsAiming)
            {
                if (!debugRightIK)
                {
                    rightHandTarget.localPosition = character.cWeapon.GetCurrentIKData.position;
                    rightHandTarget.localEulerAngles = character.cWeapon.GetCurrentIKData.eulerAngles;
                }
                else
                {
                    character.cWeapon.GetCurrentIKData.position = rightHandTarget.localPosition;
                    character.cWeapon.GetCurrentIKData.eulerAngles = rightHandTarget.localEulerAngles;
                }

                if (character.cWeapon.currentWeaponData.weaponType == WeaponData.WeaponType.Pistol)
                {
                    leftHandTargetPosition = character.cWeapon.WeaponHandTarget.position;
                    leftHandTargetRotation = character.cWeapon.WeaponHandTarget.rotation;
                }

                isActiveLeftHand = true;
                isActiveRightHand = true;

                leftHandTargetWeight = 1.0f;
                rightHandTargetWeight = 1.0f;
                return;
            }
        }
    }

    public void SetRightHand (Transform target)
    {
        rightHandTarget = target;
    }

    public void OpeningCarDoor(bool state, Transform target)
    {
        if (state)
        {
            isActiveLeftHand = true;
            leftHandTargetPosition = target.position;
            leftHandTargetRotation = target.rotation;
            leftHandTargetWeight = 1.0f;
        }
        else
        {
            isActiveLeftHand = false;
            leftHandTargetWeight = 0.0f;
        }
    }

    public void AddRecoil (RecoilData data)
    {
        recoilData = data;
        recoilCounter = 0.0f;
    }
}
