using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Third Person Synty Character/Character/Movement Data" )]
public class CharacterMovementData : ScriptableObject
{
    public float walkSpeed = 0.25f;
    public float runModifier = 2.0f;
    public float crouchModifier = 0.5f;
    public float aimModifier = 0.4f;

    public float turnSpeed = 120.0f;
}
