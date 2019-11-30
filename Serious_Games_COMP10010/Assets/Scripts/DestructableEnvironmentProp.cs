using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableEnvironmentProp : MonoBehaviour
{
    public enum DestroyType { FallOver, Vanish }
    [SerializeField] DestroyType destroyType = DestroyType.FallOver;
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private Vector3 localPosition;
    [SerializeField] private bool disableCollisions = false;

    private bool isDestroyed = false;

    private void OnDrawGizmosSelected ()
    {
        Gizmos.DrawCube ( transform.TransformPoint ( localPosition ), Vector3.one * 0.2f );
    }

    //private void OnCollisionEnter (Collision collision)
    //{
    //    if (collision.collider.CompareTag ( "truck" ))
    //        DestroyProp ( collision.collider );
    //}

    private void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag ( "truck" ))
            DestroyProp ( other );
    }

    private void DestroyProp (Collider other)
    {
        if (isDestroyed) return;
        isDestroyed = true;

        MultiplierManager.instance.AddProgress ( 2.0f, "Destruction" );

        if(particlePrefab != null)
        {
            GameObject go = Instantiate ( particlePrefab );
            go.transform.position = transform.TransformPoint ( localPosition );
        }

        switch (destroyType)
        {
            case DestroyType.FallOver:
                FallOver (other);
                break;
            case DestroyType.Vanish:
                Vanish ();
                break;
        }
    }

    private void FallOver (Collider other)
    {
        GetComponent<Collider> ().isTrigger = false;

        Rigidbody rb = GetComponent<Rigidbody> ();

        if (rb == null)
        {
            gameObject.AddComponent<Rigidbody> ();
        }

        if (disableCollisions)
            Physics.IgnoreCollision ( other, GetComponent<Collider> () );
    }

    private void Vanish ()
    {
        Destroy ( this.gameObject );
    }

    private void OnValidate ()
    {
        if (GetComponent<Collider> () == null)
        {
            MeshCollider m = gameObject.AddComponent<MeshCollider> ();
            m.convex = true;
            m.isTrigger = true;
        }
    }
}
