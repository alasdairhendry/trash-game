using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPhysics : MonoBehaviour
{
    public Character character { get; protected set; }
    public new Rigidbody rigidbody { get; protected set; }

    public float baseMass { get; protected set; } = 1.0f;
    public float baseDrag { get; protected set; } = 1.0f;
    public float baseAngularDrag { get; protected set; } = 1.0f;

    public bool useGravity { get; protected set; } = true;
    public bool isKinematic { get; protected set; } = false;
    public RigidbodyConstraints constraints { get; protected set; }

    private bool defaultsSet = false;

    private void Awake ()
    {
        character = GetComponent<Character> ();
        rigidbody = GetComponent<Rigidbody> ();

        SetDefaults ();
    }

    private void SetDefaults ()
    {
        if (defaultsSet) return;
        defaultsSet = true;

        baseMass = rigidbody.mass;
        baseDrag = rigidbody.drag;
        baseAngularDrag = rigidbody.angularDrag;

        useGravity = rigidbody.useGravity;
        isKinematic = rigidbody.isKinematic;
        constraints = rigidbody.constraints;
    }

    public void SetMass (float value)
    {
        rigidbody.mass = value;
    }

    public void ResetMass ()
    {
        SetDefaults ();
        rigidbody.mass = baseMass;
    }

    public void SetGravity (bool value)
    {
        rigidbody.useGravity = value;
    }

    public void ResetGravity ()
    {
        SetDefaults ();
        rigidbody.useGravity = useGravity;
    }

    public void SetKinematic (bool value)
    {
        rigidbody.isKinematic = value;
    }

    public void ResetKinematic ()
    {
        SetDefaults ();
        rigidbody.isKinematic = isKinematic;
    }



    public void SetConstraints(RigidbodyConstraints constraints)
    {
        rigidbody.constraints = constraints;
    }

    public void ResetConstraints ()
    {
        rigidbody.constraints = constraints;
    }

    public void ResetAll ()
    {
        rigidbody.mass = baseMass;
        rigidbody.drag = baseDrag;
        rigidbody.angularDrag = baseAngularDrag;

        rigidbody.useGravity = useGravity;
        rigidbody.isKinematic = isKinematic;
        rigidbody.constraints = constraints;
    }
}
