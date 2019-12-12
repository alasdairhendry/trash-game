using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class TaskManager : MonoBehaviour
{
    public static TaskManager instance;

    private void Awake ()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy ( this.gameObject );
            return;
        }
    }

    public Task activeTask { get; protected set; }

    public List<Task> tasks { get; protected set; } = new List<Task> ();

    [SerializeField] private float timePerTask = 30.0f;
    private float currentTime = 0.0f;

    [SerializeField] private TextMeshProUGUI taskNameText;
    [SerializeField] private TextMeshProUGUI timeLeftText;
    [SerializeField] private TextMeshProUGUI progressDarkText;
    [SerializeField] private TextMeshProUGUI progressLightText;
    [SerializeField] private Image fillImage;
    [SerializeField] private float fillImageDamp;
    private float fillImageTarget;
    [Space]
    [SerializeField] private AreaCollider truckAreaCollider;
    [SerializeField] private UILineRenderer lr;
    private bool displayAreaNavigation = false;
    private Vector3 targetPosition = new Vector3 ();
    private TrashSpawner spawner;
    NavMeshPath path;
    MiniMap mm;

    private void Start ()
    {
        TickSystem.Tick += Tick;
        path = new NavMeshPath ();
        mm = FindObjectOfType<MiniMap> ();

        CreateTasks ();
        AssignInitialTask ();
        truckAreaCollider.OnAreaChanged += OnTruckAreaChanged;
        InvokeRepeating ( nameof ( CheckAreaNavigation ), 0.25f, 0.25f );
    }

    private void Update ()
    {
        MonitorTime ();
        UpdateActiveTask ();

        fillImage.fillAmount = Mathf.Lerp ( fillImage.fillAmount, fillImageTarget, fillImageDamp * Time.deltaTime );        
    }

    private void Tick()
    {
        if (TickSystem.Equals ( 10 ))
        {
            if (activeTask != null && activeTask.isActive)
            {
                timeLeftText.text = currentTime.ToString ( "0" ) + " secs\nleft";
            }
        }
    }

    private void CreateTasks ()
    {
        tasks.Add ( new Task01 () );
        tasks.Add ( new Task02 () );
        tasks.Add ( new Task03 () );
        tasks.Add ( new Task04 () );
        tasks.Add ( new Task05 () );
        tasks.Add ( new Task06 () );
        tasks.Add ( new Task07 () );
        tasks.Add ( new Task08 () );
        tasks.Add ( new Task09 () );
        tasks.Add ( new Task10 () );
    }

    public void AddTime ()
    {
        currentTime = activeTask.timeAllowed / 2.0f;
    }

    private void AssignInitialTask (Task previousTask = null)
    {
        tasks = Shuffle ( tasks );

        if (previousTask != null)
        {
            if (tasks[0] == previousTask)
            {
                tasks.RemoveAt ( 0 );
                tasks.Add ( previousTask );
            }
        }
        else
        {
            Task task = tasks.FirstOrDefault ( x => x.taskName == "Collect 5 trash" );
            tasks.Remove ( task );
            tasks.Insert ( 0, task );
        }

        AssignTask ( tasks[0] );
    }

    private void AssignTask (Task task)
    {
        activeTask = task;
        currentTime = activeTask.timeAllowed;
        taskNameText.text = activeTask.taskName;
        timeLeftText.text = currentTime.ToString ( "0" ) + " secs\nleft";

        UpdateProgressText ();

        activeTask.Begin ();

        activeTask.OnTaskUpdated += OnTaskUpdated;
        activeTask.OnTaskComplete += OnTaskComplete;

        if (string.IsNullOrEmpty ( activeTask.requiredArea ))
        {
            displayAreaNavigation = false;
            lr.Points = new Vector2[0];
        }
        else
        {
            if (truckAreaCollider.currentArea != null)
            {
                OnTruckAreaChanged ( truckAreaCollider.currentArea );
            }
        }
    }

    [NaughtyAttributes.Button]
    private void UpdateProgressText ()
    {
        progressLightText.text = activeTask.GetProgressString ();
        progressDarkText.text = activeTask.GetProgressString ();
        fillImageTarget = activeTask.GetProgressPercentage ();
    }

    private void MonitorTime ()
    {
        if (activeTask != null && activeTask.isActive && currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            if (currentTime < 0.0f)
            {
                QuestionCanvas.instance.ShowQuestion ();
                currentTime = 0.0f;
            }

            timeLeftText.text = currentTime.ToString ( "0" ) + " secs\nleft";
        }
    }

    private void UpdateActiveTask ()
    {
        if (activeTask != null && activeTask.isActive)
        {
            activeTask.Update ();
        }
    }

    private void OnTaskUpdated(Task task)
    {
        if (task != activeTask) return;
        UpdateProgressText ();
    }

    private void OnTaskComplete (Task task)
    {
        if (task != activeTask) return;

        activeTask.OnTaskUpdated -= OnTaskUpdated;
        activeTask.OnTaskComplete -= OnTaskComplete;

        if (tasks.IndexOf ( task ) < tasks.Count - 1)
        {
            AssignTask ( tasks[tasks.IndexOf ( task ) + 1] );
        }
        else
        {
            AssignInitialTask ( task );
        }

        Debug.Log ( "Task complete" );
    }

    public void FailTask ()
    {
        if (activeTask == null) return;

        Task task = activeTask;
        activeTask.OnTaskUpdated -= OnTaskUpdated;
        activeTask.OnTaskComplete -= OnTaskComplete;

        if (tasks.IndexOf ( task ) < tasks.Count - 1)
        {
            AssignTask ( tasks[tasks.IndexOf ( task ) + 1] );
        }
        else
        {
            AssignInitialTask ( task );
        }

        Debug.Log ( "Task failed" );
    }

    public List<T> Shuffle<T> (List<T> list)
    {
        System.Random rng = new System.Random ();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next ( n + 1 );
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    private void OnTruckAreaChanged(Area newArea)
    {
        if (activeTask != null )
        {
            if (activeTask.isActive)
            {
                if (!string.IsNullOrEmpty ( activeTask.requiredArea ))
                {
                    if(newArea.AreaName != activeTask.requiredArea)
                    {
                        if (spawner == null) spawner = FindObjectOfType<TrashSpawner> ();
                        targetPosition = spawner.SpawnData.FirstOrDefault ( x => x.area.AreaName == activeTask.requiredArea ).area.gameObject.transform.GetChild ( 0 ).position;
                        displayAreaNavigation = true;
                        return;
                    }
                }
            }
        }

        displayAreaNavigation = false;
        lr.Points = new Vector2[0];
    }

    private void CheckAreaNavigation ()
    {
        if (!displayAreaNavigation) return;

        if (NavMesh.CalculatePath ( truckAreaCollider.transform.position, targetPosition, NavMesh.AllAreas, path ))
        {
            Vector2[] mmPoints = new Vector2[path.corners.Length];

            for (int i = 0; i < path.corners.Length; i++)
            {
                path.corners[i] += Vector3.up * 0.01f;
                mmPoints[i] = mm.GetPositionOnMiniMap ( path.corners[i], false, false );
            }

            lr.Points = mmPoints;
        }


    }
}
