using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuickParentData : ScriptableObject
{
    /*[HideInInspector] */public List<Data> datas = new List<Data> ();
    /*[HideInInspector] */public bool editMode = false;
    /*[HideInInspector] */public string categoryToAdd = "";
    /*[HideInInspector] */public List<string> categories = new List<string> ();

    [System.Serializable] 
    public class Data
    {
        public int GUID;
        public Transform transform;
        public string scene;
        public string category;
        public string customName;

        public Data (Transform transform, string category, string customName, Scene scene)
        {
            this.GUID = transform.GetInstanceID();
            this.transform = transform;
            this.category = category;
            this.customName = customName;
            this.scene = scene.name;
        }
    }
}
