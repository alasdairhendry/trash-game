using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapObject : MonoBehaviour
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private Vector2 dimensions;
    private MiniMap miniMap;

    private void Start ()
    {
        miniMap = FindObjectOfType<MiniMap> ();
        miniMap.AddObject ( this, sprite, dimensions );
    }

    private void OnDestroy ()
    {
        miniMap.RemoveObject ( this );
    }
}
