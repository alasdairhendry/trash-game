using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

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

    private void Start ()
    {
        mainCamera = Camera.main.transform;
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
        go.GetComponent<Citizen> ().Initialise ( type, position, this );
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

    private void Update ()
    {
        MonitorGetPathQueue ();
        MonitorMonitorPathQueue ();

        if (!hadIncident)
            secondsWithoutIncidentTarget += Time.deltaTime;

        secondsWithoutIncidentCurrent = Mathf.Lerp ( secondsWithoutIncidentCurrent, secondsWithoutIncidentTarget, Time.deltaTime * 5.0f );

        secondsWithoutIncidentText.text = Mathf.FloorToInt ( secondsWithoutIncidentCurrent ).ToString("0");
        if(int.Parse(secondsWithoutIncidentText.text) <= 0)
        {
            hadIncident = false;
        }
    }

    Vector3 getPathQueueTargetPosition = new Vector3 ();
    NavMeshHit hit;
    Vector3 monitorPathQueueDirection = new Vector3 ();
    Transform mainCamera;

    Queue<CitizenNavigation> getPathQueue = new Queue<CitizenNavigation> ();

    Queue<CitizenNavigation> monitorPathQueue = new Queue<CitizenNavigation> ();

    public void EnqueueGetPath (CitizenNavigation cNav)
    {
        if (!getPathQueue.Contains ( cNav ))
            getPathQueue.Enqueue ( cNav );
    }

    private void MonitorGetPathQueue ()
    {
        if (getPathQueue.Count > 0)
        {
            for (int i = 0; i < Mathf.Min ( 3, getPathQueue.Count ); i++)
            {
                PerformGetPathQueue ( getPathQueue.Dequeue () );
            }
        }
    }

    public void PerformGetPathQueue(CitizenNavigation cNav)
    {
        if (cNav.IsNavigating) return;

        getPathQueueTargetPosition = cNav.transform.position + (Random.insideUnitSphere * cNav.TravelRadius);
        getPathQueueTargetPosition.y = 0;

        if (NavMesh.SamplePosition ( getPathQueueTargetPosition, out hit, cNav.TravelRadius, 1 << NavMesh.GetAreaFromName ( "Pavement" ) ))
        {
            getPathQueueTargetPosition = hit.position;
        }
        else
        {
            if (!cNav.Citizen.isDead)
                EnqueueGetPath ( cNav );
            return;
        }

        if (cNav.Agent.CalculatePath ( getPathQueueTargetPosition, cNav.currentPath ))
        {
            if (cNav.currentPath.status == NavMeshPathStatus.PathComplete || cNav.currentPath.status == NavMeshPathStatus.PathPartial)
            {
                cNav.BeginNavigation ();
            }
            else
            {
                if (!cNav.Citizen.isDead)
                    EnqueueGetPath ( cNav );
            }
        }
    }

    public void EnqueueMonitorPath(CitizenNavigation cNav)
    {
        if (!monitorPathQueue.Contains ( cNav ))
            monitorPathQueue.Enqueue ( cNav );
    }

   [SerializeField]  bool doMax = false;

    private void MonitorMonitorPathQueue ()
    {
        if (monitorPathQueue.Count > 0)
        {
            if (doMax)
            {
                for (int i = 0; i < monitorPathQueue.Count; i++)
                {
                    CitizenNavigation cNav = monitorPathQueue.Dequeue ();
                    PerformMonitorPathQueue ( cNav );
                }
            }
            else
            {
                for (int i = 0; i < Mathf.Min ( 15, monitorPathQueue.Count ); i++)
                {
                    CitizenNavigation cNav = monitorPathQueue.Dequeue ();
                    PerformMonitorPathQueue ( cNav );
                }
            }
            
        }
    }

    private void PerformMonitorPathQueue (CitizenNavigation cNav)
    {
        if (!cNav.IsNavigating) return;

        if (Vector3.Distance ( cNav.transform.position, mainCamera.position ) > 100.0f)
        {
            cNav.DelayedQueueMonitor ( 0.25f );
            return;
        }

        if (cNav.PathCornerIndex >= cNav.PathNodes.Length - 1)
        {
            // Path Finished.
            cNav.ForwardTarget = 0.0f;
            cNav.IsNavigating = false;
            cNav.DelayedQueuePath ( Random.Range ( 2.5f, 10.0f ) );
        }
        else
        {
            if (Vector3.Distance ( cNav.transform.position, cNav.PathNodes[cNav.PathCornerIndex + 1] ) < cNav.VerifyPathIndexDistance)
            {
                cNav.PathCornerIndex++;
                cNav.DelayedQueueMonitor ( 0.25f );
                return;
            }

            monitorPathQueueDirection = cNav.PathNodes[cNav.PathCornerIndex + 1] - cNav.transform.position;
            monitorPathQueueDirection.Normalize ();
            monitorPathQueueDirection.y = 0.0f;

            cNav.LookDir = Quaternion.LookRotation ( monitorPathQueueDirection );

            cNav.ForwardTarget = 0.5f;
            cNav.DelayedQueueMonitor ( 0.25f );
            return;
        }
    }

    [SerializeField] private UITween secondsWithoutIncidentTween;
    [SerializeField] private TextMeshProUGUI secondsWithoutIncidentText;
    private float secondsWithoutIncidentTarget = 0.0f;
    private float secondsWithoutIncidentCurrent = 0.0f;
    private bool hadIncident = false;

    public void OnDeath(Citizen c)
    {
        secondsWithoutIncidentTarget = 0;
        secondsWithoutIncidentTween.FadeIn (0.25f);
        hadIncident = true;
    }
}


