using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AreaPanel : MonoBehaviour
{
    [SerializeField] private Image fillBar;
    [SerializeField] private TextMeshProUGUI textLabelOne;
    [SerializeField] private TextMeshProUGUI textLabelTwo;
    [Space]
    [SerializeField] private TrashSpawner spawner;
    [SerializeField] private Area area;
    private List<TrashPickupSpawn> spawns = new List<TrashPickupSpawn> ();
    float target = 0.0f;

    private void Start ()
    {
        textLabelOne.text = area.AreaName;
        textLabelTwo.text = area.AreaName;
        spawns = spawner.spawnsByArea[area];
        target = Mathf.Lerp ( 0.0f, 1.0f, (float)spawner.spawnDataByArea[this.area].currentTrashInArea / (float)spawns.Count );
        spawner.OnTrashSpawned += OnTrashSpawnedInArea;
        spawner.OnTrashCollected += OnTrashCollectedInArea;

        if (area.AreaName == "Mariners Dock")
        {
            Debug.Log ( spawns.Count );
        }
    }

    private void OnTrashSpawnedInArea (Area area, TrashPickupSpawn spawn)
    {
        if (this.area == area)
        {
            target = Mathf.Lerp ( 0.0f, 1.0f, (float)spawner.spawnDataByArea[this.area].currentTrashInArea / (float)spawns.Count );
        }
    }

    private void OnTrashCollectedInArea (Area area, TrashPickupSpawn spawn, Trash trash)
    {
        if (this.area == area)
        {
            target = Mathf.Lerp ( 0.0f, 1.0f, (float)spawner.spawnDataByArea[this.area].currentTrashInArea / (float)spawns.Count );
        }
    }

    private void Update ()
    {
        fillBar.fillAmount = Mathf.Lerp ( fillBar.fillAmount, target, Time.deltaTime * 2.0f );
    }
}
