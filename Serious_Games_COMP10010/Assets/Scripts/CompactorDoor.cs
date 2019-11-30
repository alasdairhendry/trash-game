using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompactorDoor : MonoBehaviour
{
    [SerializeField] private Transform door;
    [SerializeField] private Transform doorHinge;
    [SerializeField] private Transform grinder;
    [SerializeField] private Transform spring;

    [SerializeField] private Vector2 doorScaleRange = new Vector2 ();
    [SerializeField] private Vector2 grinderPositionRange = new Vector2 ();
    [SerializeField] private float drag = 3.0f;
    [SerializeField] private float velocityMax = 10.0f;
    [SerializeField] private float speed = 2.0f;

   [SerializeField] private float velocity = 0.0f;
   [SerializeField] private bool countingUp = true;
   [SerializeField] float scale = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    [ContextMenu("Add")]
    public void Add ()
    {
        Debug.Log ( Mathf.Abs ( doorHinge.position.y - grinder.position.y ) );
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //grinder.transform.position = new Vector3 ( grinder.transform.position.x, spring.position.y, grinder.transform.position.z );

        //return;

        float d = Mathf.Abs ( doorHinge.position.y - grinder.position.y );
        //float s = Mathf.Clamp ( d * 100.0f, doorScaleRange.x, doorScaleRange.y ) + 50;

        float pNrm = Mathf.InverseLerp ( grinderPositionRange.x, grinderPositionRange.y, d );
        float s = Mathf.Lerp ( doorScaleRange.y, doorScaleRange.x, pNrm );
        door.localScale = new Vector3 ( 100.0f, 100.0f, s );
        //Debug.Log ( "Dist " + d.ToString ( "00.00" ) + "  -  " + s.ToString ( "00.00" ) );
        //Debug.Log ( s );
    }

    //private void LateUpdate ()
    //{
    //    grinder.transform.position = spring.transform.position;
    //}

    //private void LateUpdate ()
    //{
    //    grinder.transform.position = doorHinge.transform.position;
    //}
}
