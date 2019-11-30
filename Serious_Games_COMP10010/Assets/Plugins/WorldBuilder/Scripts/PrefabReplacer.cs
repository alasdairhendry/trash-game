using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabReplacer : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabs = new List<GameObject> ();

    public enum View { List, Grid, Details }

    public View view { get; set; } = View.Details;

    public List<GameObject> Prefabs { get => prefabs; set => prefabs = value; }
}
