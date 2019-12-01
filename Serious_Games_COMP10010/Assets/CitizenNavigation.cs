using UnityEngine;
using UnityEngine.AI;

public class CitizenNavigation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float travelRadius;
    public NavMeshPath currentPath;
    private float currentForward = 0.0f;

    [SerializeField] private float forwardDamping = 5.0f;
    [SerializeField] private float verifyPathIndexDistance = 1.0f;
    [SerializeField] private float lookRotation = 7.5f;
    private Camera mainCamera;
    private CitizenController cController;
    [SerializeField] private Citizen citizen;

    public float TravelRadius { get => travelRadius; }
    public NavMeshAgent Agent { get; private set; }
    public bool IsNavigating { get; set; } = false;
    public int PathCornerIndex { get; set; }
    public Vector3[] PathNodes { get; set; }
    public float VerifyPathIndexDistance { get => verifyPathIndexDistance; }
    public Quaternion LookDir { get; set; } = Quaternion.identity;
    public float ForwardTarget { get; set; } = 0.0f;
    public Citizen Citizen { get => citizen; }

    private bool isRunning = false;

    public void Initialise (CitizenController cController)
    {
        this.cController = cController;
        currentPath = new NavMeshPath ();
        mainCamera = Camera.main;

        if (SetupAgent ())
        {
            RequestNewPathImmediately ();
            //InvokeRepeating ( nameof ( UpdateNavigation ), 0.25f, 0.25f );
        }
    }

    private bool SetupAgent ()
    {
        Agent = GetComponent<NavMeshAgent> ();

        NavMeshHit hit;

        if (NavMesh.SamplePosition ( transform.position, out hit, 250.0f, 1 << NavMesh.GetAreaFromName ( "Pavement" ) ))
        {
            Agent.Warp ( hit.position );
        }

        if (!Agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition ( transform.position, out hit, 50.0f, NavMesh.AllAreas ))
            {
                Agent.Warp ( hit.position );
            }
        }

        if (!Agent.isOnNavMesh)
        {
            transform.position = hit.position;
            Agent.enabled = false;
            Agent.enabled = true;
        }

        if (!Agent.isOnNavMesh)
        {
            Destroy ( this.gameObject );
            return false;
        }

        Agent.SetAreaCost ( NavMesh.GetAreaFromName ( "Pavement" ), 1 );
        Agent.SetAreaCost ( NavMesh.GetAreaFromName ( "Crossing" ), 1 );
        Agent.SetAreaCost ( NavMesh.GetAreaFromName ( "Walkable" ), 999 );

        float speed = Random.Range ( 0.8f, 1.4f );
        animator.speed = speed;
        lookRotation *= speed;

        return true;
    }

    private void RequestNewPathImmediately ()
    {
        if (IsNavigating) return;
        if (Citizen.isDead) return;
        cController.PerformGetPathQueue ( this );
    }

    private void RequestNewPath ()
    {
        if (IsNavigating) return;
        if (Citizen.isDead) return;
        cController.EnqueueGetPath ( this );
    }

    public void DelayedQueuePath(float delay)
    {
        if (IsNavigating) return;
        if (Citizen.isDead) return;
        Invoke ( nameof ( RequestNewPath ), delay );
    }

    public void DelayedQueueMonitor(float delay)
    {
        if (!IsNavigating) return;
        if (Citizen.isDead) return;
        Invoke ( nameof ( RequestPathToBeMonitored ), delay );
    }

    private void RequestPathToBeMonitored ()
    {
        if (!IsNavigating) return;
        if (Citizen.isDead) return;
        cController.EnqueueMonitorPath ( this );
    }

    public void BeginNavigation ()
    {
        if (Citizen.isDead) return;
        IsNavigating = true;
        PathCornerIndex = 0;
        PathNodes = currentPath.corners;
        isRunning = (Random.value > 0.8f) ? true : false;
        RequestPathToBeMonitored ();
    }

    private void Update ()
    {
        if (Citizen.isDead) return;

        if (currentForward != ForwardTarget)
        {
            currentForward = Mathf.Lerp ( currentForward, ForwardTarget * (isRunning ? 2.0f : 1.0f), forwardDamping * Time.deltaTime );
            animator.SetFloat ( "forward", currentForward );
        }

        if (IsNavigating)
        {
            if (!citizen.IsCulled)
            {
                transform.rotation = Quaternion.Slerp ( transform.rotation, LookDir, Time.deltaTime * lookRotation );
            }
        }
    }

    private void OnDrawGizmosSelected ()
    {
        for (int i = 0; i < PathNodes.Length - 1; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine ( PathNodes[i], PathNodes[i  + 1] );
            Gizmos.color = Color.blue;
            Gizmos.DrawLine ( PathNodes[i], PathNodes[i] + Vector3.up * 5.0f );
        }
    }
}
