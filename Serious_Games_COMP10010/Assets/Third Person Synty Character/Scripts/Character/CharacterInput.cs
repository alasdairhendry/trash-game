using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    public Character character { get; protected set; }

    public Vector2 input { get; set; }
    public Vector2 rawInput { get; set; }

    private void Awake ()
    {
        character = GetComponent<Character> ();
    }

    private void Update ()
    {
        if (character.IsAI) return;
        CheckAim ();
        input = new Vector2 ( Input.GetAxis ( "Horizontal" ), Input.GetAxis ( "Vertical" ) ).normalized;
        rawInput = new Vector2 ( Input.GetAxisRaw ( "Horizontal" ), Input.GetAxisRaw ( "Vertical" ) ).normalized;   

        if (Input.GetKeyDown ( KeyCode.C ))
        {
            character.isCrouchingInput = !character.isCrouchingInput;
        }

        character.isRunning = Input.GetKey ( KeyCode.LeftShift );

        if (Input.GetKeyDown ( KeyCode.Space ))
        {
            character.shouldJump = true;
        }
    }

    public bool isAimInput { get; protected set; } = false;

    private void CheckAim ()
    {
        //if (character.cWeapon.ShouldAim)
        //{
            //isAimInput = true;
            //return;
        //}

        isAimInput = Input.GetKey ( KeyCode.V ) || Input.GetMouseButton ( 1 );
    }
}
