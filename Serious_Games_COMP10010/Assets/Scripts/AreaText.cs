using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AreaText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Animator anim;
    private string targetText;

    public void SetText(string t)
    {
        targetText = t;
        anim.SetBool ( "Change", true );
    }

    public void OnTrigger ()
    {
        text.text = targetText;
        targetText = "";
        anim.SetBool ( "Change", false );
    }
}
