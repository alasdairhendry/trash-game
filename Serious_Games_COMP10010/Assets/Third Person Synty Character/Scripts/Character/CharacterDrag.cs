using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterDrag : MonoBehaviour
{
    public Character character { get; protected set; }

    public bool isDragging { get; protected set; } = false;
    public bool isBeingDragged { get; protected set; } = false;
    public bool isInDraggedState { get; set; } = false;    

    public Character target { get; protected set; }
    public Animator animator { get; protected set; }
    public new Rigidbody rigidbody { get; protected set; }

    public Transform dragIKTarget;

    private void Awake ()
    {
        character = GetComponent<Character> ();
        animator = GetComponent<Animator> ();
        rigidbody = GetComponent<Rigidbody> ();
    }

    public void OnUpdate ()
    {
        CheckInput ();

        if (isBeingDragged && isInDraggedState)
        {
            character.SetInput ( target.cInput.rawInput );
        }
    }

    public void OnFixedUpdate ()
    {

    }

    public void OnBeginDrag (Character to)
    {
        target = to;

        animator.SetTrigger ( "drag" );
        isDragging = true;
    }

    public void OnBeginDragged (Character from)
    {
        target = from;

        transform.position = from.transform.position + (from.transform.forward * 1.0f);
        Vector3 direction = from.transform.position - transform.position;
        direction.y = 0.0f;
        direction.Normalize ();

        transform.rotation = Quaternion.LookRotation ( direction );
        animator.SetBool ( "dragged", true );
        isBeingDragged = true;
    }

    public void OnEndDrag ()
    {
        target = null;

        //animator.SetBool ( "drag", false );
        isDragging = false;
        character.cIK.SetRightHand ( null );
    }

    public void OnEndDragged ()
    {
        target = null;
        character.SetInput ( Vector2.zero );
        animator.SetBool ( "dragged", false );
        isBeingDragged = false;        
    }

    private void CheckInput ()
    {
        if (character.IsAI) return;

        if (Input.GetKeyDown ( KeyCode.T ))
        {
            if (target != null)
            {
                target.cDrag.OnEndDragged ();
                this.OnEndDrag ();
            }

            Character other = FindObjectsOfType<Character> ().FirstOrDefault ( x => x.IsAI );

            other.cDrag.OnBeginDragged ( this.character );
            this.OnBeginDrag ( other );
        }

        if (Input.GetKeyDown ( KeyCode.Y ))
        {
            if (target != null)
            {
                target.cDrag.OnEndDragged ();
                this.OnEndDrag ();
            }
        }
    }
}
