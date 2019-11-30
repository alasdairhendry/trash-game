using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAvatar : MonoBehaviour
{
    [SerializeField] private Avatar city;
    [SerializeField] private Avatar town;
    [SerializeField] private Animator anim;

    [NaughtyAttributes.Button]
    public void SetCity ()
    {
        anim.avatar = city;
    }

    [NaughtyAttributes.Button]
    public void SetTown ()
    {
        anim.avatar = town;
    }

}
