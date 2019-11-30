using UnityEngine;
using UnityEngine.AI;

public class CitizenNavigation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float travelRadius;
    private NavMeshPath currentPath;
    private Vector3[] pathNodes;

    private bool isNavigating = false;
    private int pathCornerIndex;

    private float forwardTarget = 0.0f;

    [SerializeField] private float forwardDamping = 5.0f;
    [SerializeField] private float verifyPathIndexDistance = 1.0f;
    [SerializeField] private float lookRotation = 7.5f;

    NavMeshAgent agent;

    [SerializeField] private Transform overrideTransform;

    public void Initialise ()
    {
        SetupAgent ();
        CreateNewPath ();
    }

    private void SetupAgent ()
    {
        agent = GetComponent<NavMeshAgent> ();

        NavMeshHit hit;

        if (NavMesh.SamplePosition ( transform.position, out hit, 250.0f, 1 << NavMesh.GetAreaFromName ( "Pavement" ) ))
        {
            agent.Warp ( hit.position );
        }

        agent.SetAreaCost ( NavMesh.GetAreaFromName ( "Pavement" ), 1 );
        agent.SetAreaCost ( NavMesh.GetAreaFromName ( "Crossing" ), 1 );
        agent.SetAreaCost ( NavMesh.GetAreaFromName ( "Walkable" ), 999 );

        //agent.enabled = false;

        float speed = Random.Range ( 0.8f, 1.4f );
        animator.speed = speed;
        lookRotation *= speed;
    }

    private void CreateNewPath ()
    {
        currentPath = new NavMeshPath ();

        Vector3 targetPosition = transform.position + (Random.insideUnitSphere * travelRadius);
        targetPosition.y = 0;

        if (overrideTransform != null)
            targetPosition = overrideTransform.position;

        NavMeshHit hit;

        if(NavMesh.SamplePosition(targetPosition, out hit, travelRadius, 1 << NavMesh.GetAreaFromName ( "Pavement" ) ))
        {
            targetPosition = hit.position;
        }
        else
        {
            Debug.Log ( "Nahhh" );
            return;
        }

        if (agent.CalculatePath ( targetPosition, currentPath ))
        {
            if (currentPath.status == NavMeshPathStatus.PathComplete)
            {
                BeginNavigation ();
            }
            else
            {
                Invoke ( nameof ( CreateNewPath ), Random.Range ( 2.5f, 10.0f ) );
            }
        }

        //if (NavMesh.CalculatePath ( transform.position, targetPosition, new NavMeshQueryFilter () { agentTypeID = agent.agentTypeID, areaMask = NavMesh.AllAreas }, currentPath ))
        //{
        //    if (currentPath.status == NavMeshPathStatus.PathComplete)
        //    {
        //        BeginNavigation ();
        //    }
        //    else
        //    {
        //        Invoke ( nameof ( CreateNewPath ), Random.Range ( 2.5f, 10.0f ) );
        //    }
        //}
    }

    private void Update ()
    {
        if (agent == null) return;
        if (overrideTransform != null)
            CreateNewPath ();

        UpdateNavigation ();
    }

    private void BeginNavigation ()
    {
        isNavigating = true;
        pathCornerIndex = 0;
        pathNodes = currentPath.corners;

        //NavMeshHit hit;

        //for (int i = 0; i < pathNodes.Length; i++)
        //{
        //    Vector3 random = Random.insideUnitSphere * 2.0f;
        //    random.y = 0;
        //    pathNodes[i] += random;

        //    if(NavMesh.SamplePosition(pathNodes[i], out hit, 2.0f, NavMesh.AllAreas ))
        //    {
        //        pathNodes[i] = hit.position;
        //    }
        //}
    }

    private void OnDrawGizmosSelected ()
    {
        if (pathNodes == null) return;
        for (int i = 0; i < pathNodes.Length - 1; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine ( pathNodes[i], pathNodes[i + 1] );
        }
    }

    private void UpdateNavigation ()
    {
        animator.SetFloat ( "forward", Mathf.Lerp ( animator.GetFloat ( "forward" ), forwardTarget, forwardDamping * Time.deltaTime ) );

        if (!isNavigating) return;

        if (Vector3.Distance ( transform.position, Camera.main.transform.position ) > 100.0f) return;

        if (pathCornerIndex >= pathNodes.Length - 1)
        {
            forwardTarget = 0.0f;
            isNavigating = false;

            Invoke ( nameof ( CreateNewPath ), Random.Range ( 2.5f, 10.0f ) );
        }
        else
        {
            if (Vector3.Distance ( transform.position, pathNodes[pathCornerIndex + 1] ) < verifyPathIndexDistance)
            {
                pathCornerIndex++;
                return;
            }

            Vector3 dir = pathNodes[pathCornerIndex + 1] - transform.position;
            dir.Normalize ();
            dir.y = 0.0f;

            Quaternion lookDir = Quaternion.LookRotation ( dir );
            transform.rotation = Quaternion.Slerp ( transform.rotation, lookDir, Time.deltaTime * lookRotation );

            forwardTarget = 0.5f;
            return;
        }       
    }
}
