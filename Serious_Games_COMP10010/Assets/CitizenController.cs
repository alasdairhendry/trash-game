using UnityEngine;

public class CitizenController : MonoBehaviour
{
    [SerializeField] private Bounds cityBounds = new Bounds ();
    [SerializeField] private Bounds townBounds = new Bounds ();
    [SerializeField] private GameObject citizenPrefab;

    [SerializeField] private int maxCityCitizens = 500;
    [SerializeField] private int maxTownCitizens = 300;

    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube ( cityBounds.center, cityBounds.size );
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube ( townBounds.center, townBounds.size );
    }

    private void Awake ()
    {
        for (int i = 0; i < maxCityCitizens; i++)
        {
            SpawnCitizen ( Citizen.CitizenType.City );
        }

        for (int i = 0; i < maxTownCitizens; i++)
        {
            SpawnCitizen ( Citizen.CitizenType.Town );
        }
    }

    [NaughtyAttributes.Button]
    private void SpawnCitizen ()
    {
        if(Random.value >= 0.5f)
        {
            SpawnCitizen ( Citizen.CitizenType.City );
        }
        else
        {
            SpawnCitizen ( Citizen.CitizenType.Town );
        }
    }

    private void SpawnCitizen(Citizen.CitizenType type)
    {
        GameObject go = Instantiate ( citizenPrefab );
        Vector3 position = RandomPointInBounds ( (type == Citizen.CitizenType.City) ? cityBounds : townBounds );
        go.GetComponent<Citizen> ().Initialise ( type, position );
        go.transform.SetParent ( this.transform );
    }

    public Vector3 RandomPointInBounds (Bounds bounds)
    {
        return new Vector3 (
            Random.Range ( bounds.min.x, bounds.max.x ),
            Random.Range ( bounds.min.y, bounds.max.y ),
            Random.Range ( bounds.min.z, bounds.max.z )
        );
    }

}


