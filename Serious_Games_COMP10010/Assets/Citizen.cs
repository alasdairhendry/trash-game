using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen : MonoBehaviour
{
    public enum CitizenType { City, Town }
    public CitizenType citizenType { get; protected set; }

    private GameObject activeGraphic;

    [SerializeField] private List<GameObject> cityGraphics = new List<GameObject> ();
    [SerializeField] private List<GameObject> townGraphics = new List<GameObject> ();

    [SerializeField] private List<Material> cityMaterials = new List<Material> ();
    [SerializeField] private List<Material> townMaterials = new List<Material> ();

    [SerializeField] private Avatar cityAvatar;
    [SerializeField] private Avatar townAvatar;
    [SerializeField] private Animator anim;

    [SerializeField] private CitizenNavigation cNav;
    [SerializeField] private CitizenRagdoll cRag;
    [SerializeField] private CitizenController cCon;

    [SerializeField] private GameObject cityMeshRoot;
    [SerializeField] private GameObject townMeshRoot;

    public bool IsCulled { get; protected set; }
    public bool isDead { get; protected set; }

    [NaughtyAttributes.Button]
    public void RandomInit ()
    {
        Initialise ( CitizenType.City, transform.position, FindObjectOfType<CitizenController> () );
    }


    public void Initialise (CitizenType type, Vector3 position, CitizenController cCon)
    {
        this.cCon = cCon;
        citizenType = type;
        transform.position = position;

        int random = 0;

        switch (type)
        {
            case CitizenType.City:
                random = Random.Range ( 0, cityGraphics.Count );

                activeGraphic = cityGraphics[random];
                activeGraphic.SetActive ( true );
                activeGraphic.GetComponent<SkinnedMeshRenderer> ().material = cityMaterials[Random.Range ( 0, cityMaterials.Count )];

                anim.avatar = cityAvatar;
                cityMeshRoot.SetActive ( true );
                townMeshRoot.SetActive ( false );
                break;
            case CitizenType.Town:
                random = Random.Range ( 0, townGraphics.Count );

                activeGraphic = townGraphics[random];
                activeGraphic.SetActive ( true );
                activeGraphic.GetComponent<SkinnedMeshRenderer> ().material = townMaterials[Random.Range ( 0, townMaterials.Count )];

                anim.avatar = townAvatar;
                townMeshRoot.SetActive ( true );
                cityMeshRoot.SetActive ( false );
                break;
        }

        cNav.Initialise ( cCon );
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag ( "truck" ))
        {
            isDead = true;
            cCon.OnDeath ( this );

            GetComponent<Collider> ().enabled = false;
            anim.enabled = false;
            cRag.Die ( citizenType, activeGraphic );

            Rigidbody rb = GetComponentInChildren<Rigidbody> ();

            if (rb)
            {
                rb.AddForce ( (transform.position-  other.transform.position ).normalized * 256.0f, ForceMode.Impulse );
            }
        }
    }

    public void SetIsCulled (bool state)
    {
        if (state != IsCulled)
            IsCulled = state;
    }
}
