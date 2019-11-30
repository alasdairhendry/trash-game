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

    public bool IsCulled { get; set; }

    [NaughtyAttributes.Button]
    public void RandomInit ()
    {
        Initialise ( CitizenType.City, transform.position );
    }

    public void Initialise (CitizenType type, Vector3 position)
    {
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

                break;
            case CitizenType.Town:
                random = Random.Range ( 0, townGraphics.Count );

                activeGraphic = townGraphics[random];
                activeGraphic.SetActive ( true );
                activeGraphic.GetComponent<SkinnedMeshRenderer> ().material = townMaterials[Random.Range ( 0, townMaterials.Count )];

                anim.avatar = townAvatar;
                break;
        }

        cNav.Initialise ();
    }
}
