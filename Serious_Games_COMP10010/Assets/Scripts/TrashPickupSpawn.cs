using System;
using UnityEngine;

public class TrashPickupSpawn : MonoBehaviour
{
    [SerializeField] private Area myArea;   
    private TrashSpawner spawner;
    private bool sentToSpawner = false;
    //private bool hasTrash = false;
    private Trash currentTrash = null;

    public Area MyArea { get => myArea; }
    public bool HasTrash { get => currentTrash != null; }

    public GameObject trashParticles;

    private void Awake ()
    {
        if(myArea == null)
        {
            Debug.LogError ( "No area assigned", this.gameObject );
        }
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.GetComponent<Area> () != null)
        {
            if(myArea != other.gameObject.GetComponent<Area> ())
            {
                Debug.LogError ( "Wrong area assigned. Assigned " + myArea.AreaName + " but should be " + other.gameObject.GetComponent<Area> ().AreaName, this.gameObject );
            }
        }
        else if(other.CompareTag("truck") && HasTrash)
        {
            Collect ();
        }
    }

    private void Collect ()
    {
        GameObject go = Instantiate ( trashParticles );
        go.transform.position = transform.position;
        spawner.SetTrashCollected ( myArea, this, currentTrash );
        Destroy ( transform.GetChild ( 0 ).GetChild ( 0 ).gameObject );
        currentTrash = null;
    }

    private void OnDrawGizmos ()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube ( transform.position + Vector3.up, Vector3.one * 2 );
        }
    }

    public void SetUp (TrashSpawner trashSpawner)
    {
        spawner = trashSpawner;
        spawner.AddSpawnByArea ( this );
    }

    public void Spawn (GameObject prefab)
    {
        if (HasTrash)
        {
            Debug.Log ( "There is an error", this.gameObject );
            return;
        }

        GameObject go = Instantiate ( prefab );
        go.transform.SetParent ( this.transform.GetChild ( 0 ) );
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = new Vector3 ( 0.0f, 0.0f, UnityEngine.Random.Range ( -30.0f, 30.0f ) );
        currentTrash = go.GetComponent<Trash> ();
    }
}
