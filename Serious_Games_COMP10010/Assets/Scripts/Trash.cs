using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    [SerializeField] private GameObject particlePrefab;
    public TrashType type = TrashType.Bag;
}
