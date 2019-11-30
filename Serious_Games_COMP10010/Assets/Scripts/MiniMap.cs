using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float worldXThreshold;
    [SerializeField] private float worldZThreshold;
    [SerializeField] private float imageDimensions = 4096;
    [SerializeField] private RectTransform minimapRectTransform;
    [SerializeField] private RectTransform miniMapRotationRoot;
    [SerializeField] private RectTransform playerIconRectTransform;
    [SerializeField] private bool rotatePlayer = true;

    private Vector3 targetNormalisedPosition = new Vector3 ();
    private Vector2 miniMapLerpedPosition = new Vector3 ();

    Dictionary<MiniMapObject, GameObject> objects = new Dictionary<MiniMapObject, GameObject> ();

    private void Update ()
    {
        minimapRectTransform.anchoredPosition = GetPositionOnMiniMap ( target.position, true, true );
        SetRotation ();
    }

    private void SetRotation ()
    {
        if (rotatePlayer)
        {
            playerIconRectTransform.localEulerAngles = new Vector3 ( 0.0f, 0.0f, -target.localEulerAngles.y );
        }
        else
        {
            miniMapRotationRoot.localEulerAngles = new Vector3 ( 0.0f, 0.0f, target.localEulerAngles.y );
        }
    }

    public Vector2 GetPositionOnMiniMap (Vector3 worldPosition, bool flipX, bool flipZ)
    {
        Vector3 normalisedPosition = GetTargetNormalisedPosition ( worldPosition, flipX, flipZ );
        Vector2 minimapPosition = GetMinimapNormalisedPosition ( normalisedPosition );
        return minimapPosition;
    }

    private Vector3 GetTargetNormalisedPosition (Vector3 position, bool flipX, bool flipZ)
    {
        float x = Mathf.InverseLerp ( -worldXThreshold, worldXThreshold, (flipX) ? -position.x : position.x );
        float y = 0;
        float z = Mathf.InverseLerp ( -worldZThreshold, worldZThreshold, (flipZ) ? -position.z : position.z );
        return new Vector3 ( x, y, z );
    }

    private Vector2 GetMinimapNormalisedPosition (Vector3 normalisedPosition)
    {
        float x = Mathf.Lerp ( -(imageDimensions / 2.0f), imageDimensions / 2.0f, normalisedPosition.x );
        float z = Mathf.Lerp ( -(imageDimensions / 2.0f), imageDimensions / 2.0f, normalisedPosition.z );
        return new Vector2 ( x, z );
    }     

    public void AddObject (MiniMapObject obj, Sprite sprite, Vector2 dimensions)
    {
        if (!objects.ContainsKey ( obj ))
        {
            GameObject go = new GameObject ( "minimap object" );
            go.transform.SetParent ( minimapRectTransform.transform );

            RectTransform rect = go.AddComponent<RectTransform> ();
            Image image = go.AddComponent<Image> ();

            image.sprite = sprite;
            rect.sizeDelta = dimensions;

            rect.anchoredPosition = GetPositionOnMiniMap ( obj.transform.position, false, false );

            objects.Add ( obj, go );
        }
    }

    public void RemoveObject (MiniMapObject obj)
    {
        if (objects.ContainsKey ( obj ))
        {
            Destroy ( objects[obj] );
            objects.Remove ( obj );
        }
    }
}
