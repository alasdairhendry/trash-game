using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] int amount;
    [SerializeField] private float spacing;

    private void Start ()
    {
        for (int i = 0; i < amount; i++)
        {
            for (int y = 0; y < amount; y++)
            {
                GameObject go = Instantiate ( prefab );
                Vector3 pos = new Vector3 ( (float)i * spacing - ((float)amount * spacing / 2.0f), 0.5f, (float)y * spacing - ((float)amount * spacing / 2.0f) );
                go.transform.position = pos;
            }
        }
    }
}