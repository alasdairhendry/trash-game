using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBoxes : MonoBehaviour
{
    [SerializeField] GameObject box;
    [SerializeField] int gridSize;
    [SerializeField] float gridSpace;

    private void Awake ()
    {
        Vector3 pos = new Vector3 ();
        pos.x = -((gridSize / 2.0f) * gridSpace);
        pos.z = -((gridSize / 2.0f) * gridSpace);

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                GameObject go = Instantiate ( box );

                go.transform.position = pos;
                go.transform.SetParent ( this.transform );

                pos.z += gridSpace;
            }

            pos.x += gridSpace;
            pos.z = -((gridSize / 2.0f) * gridSpace);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
