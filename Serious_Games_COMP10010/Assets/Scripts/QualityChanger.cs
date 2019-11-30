using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QualityChanger : MonoBehaviour
{
    List<string> qualityLevels = new List<string> ();

    // Start is called before the first frame update
    void Start()
    {
        qualityLevels = QualitySettings.names.ToList ();    
    }

    private void OnGUI ()
    {
        GUILayout.BeginHorizontal ();
        GUILayout.Space ( 512 );
        for (int i = 0; i < qualityLevels.Count; i++)
        {
            if (GUILayout.Button ( qualityLevels[i] ))
            {
                QualitySettings.SetQualityLevel ( i, true );
            }
        }
        GUILayout.Space ( 128 );

        if (GUILayout.Button ( "Menu" ))
        {
            SceneManager.LoadScene ( 0 );
        }
        GUILayout.EndHorizontal ();
    }
}
