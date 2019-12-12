using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    private List<TrashPickupSpawn> spawns = new List<TrashPickupSpawn> ();

    [SerializeField] private List<GameObject> trashPrefabs = new List<GameObject> ();
    [SerializeField] private List<GameObject> plasticPrefabs = new List<GameObject> ();
    [SerializeField] private List<GameObject> paperPrefabs = new List<GameObject> ();
    [SerializeField] private List<GameObject> glassPrefabs = new List<GameObject> ();
    [SerializeField] private List<GameObject> foodPrefabs = new List<GameObject> ();
    [SerializeField] private List<GameObject> metalPrefabs = new List<GameObject> ();

    [SerializeField] private List<AreaSpawnData> spawnData = new List<AreaSpawnData> ();

    [System.Serializable]
    public class AreaSpawnData
    {
        public Area area;
        public float respawnDelay = 5.0f;
        public int maximumTrash = 5;
        public bool spawnMaxOnAwake = true;
        public int currentTrashInArea;
    }

    public Dictionary<Area, List<TrashPickupSpawn>> spawnsByArea { get; protected set; } = new Dictionary<Area, List<TrashPickupSpawn>> ();
    public Dictionary<Area, AreaSpawnData> spawnDataByArea { get; protected set; } = new Dictionary<Area, AreaSpawnData> ();
    public List<AreaSpawnData> SpawnData { get => spawnData; set => spawnData = value; }

    public System.Action<Area, TrashPickupSpawn> OnTrashSpawned;
    public System.Action<Area, TrashPickupSpawn, Trash> OnTrashCollected;
    [SerializeField] private bool DEBUG_SPAWNS = false;

    private void Awake ()
    {
        spawns = GetComponentsInChildren<TrashPickupSpawn> ().ToList();

        for (int i = 0; i < spawns.Count; i++)
        {
            spawns[i].SetUp (this);
        }

        for (int i = 0; i < spawnData.Count; i++)
        {
            spawnDataByArea.Add ( spawnData[i].area, spawnData[i] );

            if (spawnData[i].spawnMaxOnAwake)
            {
                SpawnTrash ( spawnData[i], spawnData[i].maximumTrash );
            }
            else
            {
                SpawnTrash ( spawnData[i], 0 );
            }
        }

        if (DEBUG_SPAWNS)
        {
            foreach (KeyValuePair<Area, List<TrashPickupSpawn>> item in spawnsByArea)
            {
                Debug.Log ( item.Key.AreaName + " has " + item.Value.Count.ToString () + " spawns" );
            }
        }
    }

    private void SpawnTrash(AreaSpawnData areaData, int amount)
    {
        List<TrashPickupSpawn> availableTrashSpawnPoints = spawnsByArea[areaData.area].Where ( x => x.HasTrash == false ).ToList ();

        int amountToSpawn = Mathf.Min ( amount, availableTrashSpawnPoints.Count );
        Shuffle<TrashPickupSpawn> ( ref availableTrashSpawnPoints );

        List<TrashType> trashDistributionWeighting = new List<TrashType> ()
        {
                TrashType.Bag,
                TrashType.Bag,
                TrashType.Bag,
                TrashType.Bag,
                TrashType.Bag,
                TrashType.Plastic,
                TrashType.Paper,
                TrashType.Glass,
                TrashType.Food,
                TrashType.Metal
        };

        for (int i = 0; i < amountToSpawn; i++)
        {
            GameObject selectedTrashPrefab = null;
            TrashType randomType = trashDistributionWeighting[Random.Range ( 0, trashDistributionWeighting.Count )];

            switch (randomType)
            {
                case TrashType.Bag:
                    selectedTrashPrefab = trashPrefabs[Random.Range ( 0, trashPrefabs.Count )];
                    break;
                case TrashType.Plastic:
                    selectedTrashPrefab = plasticPrefabs[Random.Range ( 0, plasticPrefabs.Count )];
                    break;
                case TrashType.Paper:
                    selectedTrashPrefab = paperPrefabs[Random.Range ( 0, paperPrefabs.Count )];
                    break;
                case TrashType.Glass:
                    selectedTrashPrefab = glassPrefabs[Random.Range ( 0, glassPrefabs.Count )];
                    break;
                case TrashType.Food:
                    selectedTrashPrefab = foodPrefabs[Random.Range ( 0, foodPrefabs.Count )];
                    break;
                case TrashType.Metal:
                    selectedTrashPrefab = metalPrefabs[Random.Range ( 0, metalPrefabs.Count )];
                    break;
            }

            availableTrashSpawnPoints[i].Spawn ( selectedTrashPrefab );
            OnTrashSpawned?.Invoke ( areaData.area, availableTrashSpawnPoints[i] );
            areaData.currentTrashInArea++;
        }
    }

    public static void Shuffle<T> (ref List<T> list)
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
    }

    public void AddSpawnByArea(TrashPickupSpawn spawn)
    {
        if (spawnsByArea.ContainsKey ( spawn.MyArea ))
        {
            spawnsByArea[spawn.MyArea].Add ( spawn );
        }
        else
        {
            spawnsByArea.Add ( spawn.MyArea, new List<TrashPickupSpawn> () { spawn } );
        }
    }

    public void SetTrashCollected (Area area, TrashPickupSpawn spawn, Trash trashCollected)
    {
        spawnDataByArea[area].currentTrashInArea--;
        OnTrashCollected?.Invoke ( area, spawn, trashCollected );
        SpawnTrash ( spawnDataByArea[area], 1 );
        TrashManager.instance.AddTrash ( trashCollected.type );

        if(trashCollected.type == TrashType.Bag)
        MultiplierManager.instance.AddProgress ( 7.5f, "Cleaning Up" );
        else
        MultiplierManager.instance.AddProgress ( 15.0f, "Recycled " + trashCollected.type.ToString() );
    }
}
