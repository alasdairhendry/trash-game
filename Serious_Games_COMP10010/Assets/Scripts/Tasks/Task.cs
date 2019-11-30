using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Task
{
    protected Task ()
    {

    }

    public string taskName { get; protected set; } = "New Task";
    public string requiredArea { get; protected set; } = "";
    public float timeAllowed = 0.0f;

    public bool isActive { get; protected set; } = false;
    public System.Action<Task> OnTaskUpdated { get; set; }
    public System.Action<Task> OnTaskComplete { get; set; }

    public virtual void Begin ()
    {
        isActive = true;
    }

    public virtual void Update () { }

    protected virtual void End ()
    {
        isActive = false;
        OnTaskComplete?.Invoke ( this );
    }

    public abstract string GetProgressString ();

    public abstract float GetProgressPercentage ();
}

public abstract class TaskInt : Task
{
    public int currentProgress { get; protected set; } = 0;
    public int targetProgress { get; protected set; } = 0;

    protected override void End ()
    {
        base.End ();
        currentProgress = 0;
    }

    public override string GetProgressString ()
    {
        return (currentProgress.ToString ( "0" ) + "/" + targetProgress.ToString ( "0" )).ToString ();
    }

    public override float GetProgressPercentage ()
    {
        return (float)currentProgress / (float)targetProgress;
    }
}

public abstract class TaskFloat : Task
{
    public float currentProgress { get; protected set; } = 0.0f;
    public float targetProgress { get; protected set; } = 0.0f;

    protected override void End ()
    {
        base.End ();
        currentProgress = 0.0f;
    }

    public override string GetProgressString ()
    {
        return currentProgress.ToString ( "0.0" ) + " secs";
    }

    public override float GetProgressPercentage ()
    {
        return currentProgress / targetProgress;
    }
}

public class Task01 : TaskInt
{
    public Task01 ()
    {
        base.taskName = "Collect 5 trash";
        base.requiredArea = "";
        base.timeAllowed = 60.0f;
        base.currentProgress = 0;
        base.targetProgress = 5;
        base.isActive = false;
    }

    public override void Begin ()
    {
        base.Begin ();
        GameObject.FindObjectOfType<TrashSpawner> ().OnTrashCollected += OnTrashCollected;
    }

    private void OnTrashCollected (Area area, TrashPickupSpawn spawn, Trash trashCollected)
    {
        currentProgress++;

        OnTaskUpdated?.Invoke ( this );

        if (currentProgress >= targetProgress)
            End ();
    }

    protected override void End ()
    {
        GameObject.FindObjectOfType<TrashSpawner> ().OnTrashCollected -= OnTrashCollected;
        base.End ();
    }
}

public class Task02 : TaskInt
{
    public Task02 ()
    {
        base.taskName = "Collect 5 trash at Ocean Terrace";
        base.requiredArea = "Ocean Terrace";
        base.timeAllowed = 60.0f;
        base.currentProgress = 0;
        base.targetProgress = 5;
        base.isActive = false;
    }

    public override void Begin ()
    {
        base.Begin ();
        GameObject.FindObjectOfType<TrashSpawner> ().OnTrashCollected += OnTrashCollected;
    }

    private void OnTrashCollected (Area area, TrashPickupSpawn spawn, Trash trashCollected)
    {
        if (area.AreaName != "Ocean Terrace") return;

        currentProgress++;

        OnTaskUpdated?.Invoke ( this );

        if (currentProgress >= targetProgress)
            End ();
    }

    protected override void End ()
    {
        GameObject.FindObjectOfType<TrashSpawner> ().OnTrashCollected -= OnTrashCollected;
        base.End ();
    }
}

public class Task03 : TaskInt
{
    public Task03 ()
    {
        base.taskName = "Collect 5 trash at The Outskirts";
        base.requiredArea = "The Outskirts";
        base.timeAllowed = 60.0f;
        base.currentProgress = 0;
        base.targetProgress = 5;
        base.isActive = false;
    }

    public override void Begin ()
    {
        base.Begin ();
        GameObject.FindObjectOfType<TrashSpawner> ().OnTrashCollected += OnTrashCollected;
    }

    private void OnTrashCollected (Area area, TrashPickupSpawn spawn, Trash trashCollected)
    {
        if (area.AreaName != "The Outskirts") return;

        currentProgress++;

        OnTaskUpdated?.Invoke ( this );

        if (currentProgress >= targetProgress)
            End ();
    }

    protected override void End ()
    {
        GameObject.FindObjectOfType<TrashSpawner> ().OnTrashCollected -= OnTrashCollected;
        base.End ();
    }
}

public class Task04 : TaskInt
{
    public Task04 ()
    {
        base.taskName = "Collect 5 trash at City Hall";
        base.requiredArea = "City Hall";
        base.timeAllowed = 60.0f;
        base.currentProgress = 0;
        base.targetProgress = 5;
        base.isActive = false;
    }

    public override void Begin ()
    {
        base.Begin ();
        GameObject.FindObjectOfType<TrashSpawner> ().OnTrashCollected += OnTrashCollected;
    }

    private void OnTrashCollected (Area area, TrashPickupSpawn spawn, Trash trashCollected)
    {
        if (area.AreaName != "City Hall") return;

        currentProgress++;

        OnTaskUpdated?.Invoke ( this );

        if (currentProgress >= targetProgress)
            End ();
    }

    protected override void End ()
    {
        GameObject.FindObjectOfType<TrashSpawner> ().OnTrashCollected -= OnTrashCollected;
        base.End ();
    }
}

public class Task05 : TaskInt
{
    public Task05 ()
    {
        base.taskName = "Collect 5 trash at The Suburbs";
        base.requiredArea = "The Suburbs";
        base.timeAllowed = 60.0f;
        base.currentProgress = 0;
        base.targetProgress = 5;
        base.isActive = false;
    }

    public override void Begin ()
    {
        base.Begin ();
        GameObject.FindObjectOfType<TrashSpawner> ().OnTrashCollected += OnTrashCollected;
    }

    private void OnTrashCollected (Area area, TrashPickupSpawn spawn, Trash trashCollected)
    {
        if (area.AreaName != "The Suburbs") return;

        currentProgress++;

        OnTaskUpdated?.Invoke ( this );

        if (currentProgress >= targetProgress)
            End ();
    }

    protected override void End ()
    {
        GameObject.FindObjectOfType<TrashSpawner> ().OnTrashCollected -= OnTrashCollected;
        base.End ();
    }
}

public class Task06 : TaskInt
{
    public Task06 ()
    {
        base.taskName = "Collect 5 trash at The Offices";
        base.requiredArea = "The Offices";
        base.timeAllowed = 60.0f;
        base.currentProgress = 0;
        base.targetProgress = 5;
        base.isActive = false;
    }

    public override void Begin ()
    {
        base.Begin ();
        GameObject.FindObjectOfType<TrashSpawner> ().OnTrashCollected += OnTrashCollected;
    }

    private void OnTrashCollected (Area area, TrashPickupSpawn spawn, Trash trashCollected)
    {
        if (area.AreaName != "The Offices") return;

        currentProgress++;

        OnTaskUpdated?.Invoke ( this );

        if (currentProgress >= targetProgress)
            End ();
    }

    protected override void End ()
    {
        GameObject.FindObjectOfType<TrashSpawner> ().OnTrashCollected -= OnTrashCollected;
        base.End ();
    }
}

public class Task07 : TaskFloat
{
    private bool multiplierAchieved = false;

    public Task07 ()
    {
        base.taskName = "Hold a 5x multiplier for 15 seconds";
        base.requiredArea = "";
        base.timeAllowed = 60.0f;
        base.currentProgress = 0;
        base.targetProgress = 15;
        base.isActive = false;
    }

    public override void Begin ()
    {
        base.Begin ();
        MultiplierManager.instance.OnMultiplierChanged += OnMultiplierChanged;
        if (MultiplierManager.instance.GetCurrentMultiplier == 5) multiplierAchieved = true;
    }

    public override void Update ()
    {
        base.Update ();

        if (multiplierAchieved)
        {
            base.currentProgress += Time.deltaTime;
            OnTaskUpdated?.Invoke ( this );

            if (base.currentProgress > base.targetProgress)
            {
                End ();
            }
        }
    }

    private void OnMultiplierChanged (int multiplier)
    {
        if (multiplier == 5)
        {
            multiplierAchieved = true;
        }
        else
        {
            multiplierAchieved = false;
            base.currentProgress = 0.0f;
        }

        OnTaskUpdated?.Invoke ( this );
    }

    protected override void End ()
    {
        MultiplierManager.instance.OnMultiplierChanged -= OnMultiplierChanged;
        multiplierAchieved = false;
        base.End ();
    }
}

public class Task08 : TaskFloat
{
    private Vector2 previousPosition = new Vector2 ();
    private Transform garbageTruck;

    public Task08 ()
    {
        base.taskName = "Travel 1000m";
        base.requiredArea = "";
        base.timeAllowed = 60.0f;
        base.currentProgress = 0;
        base.targetProgress = 1000;
        base.isActive = false;
    }

    public override void Begin ()
    {
        base.Begin ();
        garbageTruck = GameObject.FindObjectOfType<GarbageTruck> ().transform;
        previousPosition = new Vector2 ( garbageTruck.position.x, garbageTruck.position.z );
    }

    public override void Update ()
    {
        base.Update ();

        currentProgress += Vector2.Distance ( previousPosition, new Vector2 ( garbageTruck.position.x, garbageTruck.position.z ) );
        previousPosition = new Vector2 ( garbageTruck.position.x, garbageTruck.position.z );

        OnTaskUpdated?.Invoke ( this );

        if (base.currentProgress > base.targetProgress)
        {
            End ();
        }
    }

    protected override void End ()
    {
        previousPosition = Vector2.zero;
        garbageTruck = null;
        base.End ();
    }

    public override string GetProgressString ()
    {
        return currentProgress.ToString ( "0" ) + "<size=80%>m</size>";
    }
}

public class Task09 : TaskInt
{
    public Task09 ()
    {
        base.taskName = "Smash into 100 objects";
        base.requiredArea = "";
        base.timeAllowed = 60.0f;
        base.currentProgress = 0;
        base.targetProgress = 100;
        base.isActive = false;
    }

    public override void Begin ()
    {
        base.Begin ();
        GameObject.FindObjectOfType<GarbageTruck> ().OnDestroyProp += OnDestroyProp;
    }

    private void OnDestroyProp (GameObject gameObject)
    {
        currentProgress++;

        OnTaskUpdated?.Invoke ( this );

        if (currentProgress >= targetProgress)
            End ();
    }

    protected override void End ()
    {
        GameObject.FindObjectOfType<GarbageTruck> ().OnDestroyProp -= OnDestroyProp;
        base.End ();
    }
}

public class Task10 : TaskFloat
{
    private Vector2 previousPosition = new Vector2 ();
    private GarbageTruck garbageTruck;

    public Task10 ()
    {
        base.taskName = "Drift for 100m";
        base.requiredArea = "";
        base.timeAllowed = 60.0f;
        base.currentProgress = 0;
        base.targetProgress = 100;
        base.isActive = false;
    }

    public override void Begin ()
    {
        base.Begin ();
        garbageTruck = GameObject.FindObjectOfType<GarbageTruck> ();
        previousPosition = new Vector2 ( garbageTruck.transform.position.x, garbageTruck.transform.position.z );
    }

    public override void Update ()
    {
        base.Update ();

        if (garbageTruck.IsDrifting)
        {
            currentProgress += Vector2.Distance ( previousPosition, new Vector2 ( garbageTruck.transform.position.x, garbageTruck.transform.position.z ) );
            OnTaskUpdated?.Invoke ( this );

            if (base.currentProgress > base.targetProgress)
            {
                End ();
            }
        }

        if (garbageTruck != null)
            previousPosition = new Vector2 ( garbageTruck.transform.position.x, garbageTruck.transform.position.z );
    }

    protected override void End ()
    {
        previousPosition = Vector2.zero;
        garbageTruck = null;
        base.End ();
    }

    public override string GetProgressString ()
    {
        return currentProgress.ToString ( "0" ) + "<size=80%>m</size>";
    }
}
