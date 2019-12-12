using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupCanvas : MonoBehaviour
{
    public static PopupCanvas instance;

    private void Awake ()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy ( this.gameObject );
            return;
        }
    }

    [SerializeField] private GameObject greenPrefab;
    [SerializeField] private GameObject redPrefab;
    private GameObject currentText;

    public void DisplayRed (string text)
    {
        if (currentText) Destroy ( currentText );
        currentText = Instantiate ( redPrefab );
        currentText.transform.SetParent ( this.transform );
        currentText.transform.localPosition = Vector3.zero;
        currentText.transform.eulerAngles = Vector3.zero;
        currentText.transform.localScale = Vector3.one;
        currentText.GetComponent<TextMeshProUGUI> ().text = text;
    }

    public void DisplayGreen (string text)
    {
        if (currentText) Destroy ( currentText );
        currentText = Instantiate ( greenPrefab );
        currentText.transform.SetParent ( this.transform );
        currentText.transform.localPosition = Vector3.zero;
        currentText.transform.eulerAngles = Vector3.zero;
        currentText.transform.localScale = Vector3.one;
        currentText.GetComponent<TextMeshProUGUI> ().text = text;
    }
}
