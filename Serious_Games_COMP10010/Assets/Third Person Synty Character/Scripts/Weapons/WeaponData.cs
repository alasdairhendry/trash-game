using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu ( menuName = "Third Person Synty Character/Weapons/New Weapon" )]
public class WeaponData : ScriptableObject
{
    public enum WeaponType { Pistol, Rifle }
    public enum FireType { Single, Burst, Auto }

    public string weaponName;
    [Space]
    public WeaponType weaponType = WeaponType.Pistol;
    public HumanBodyBones activeBodyPart = HumanBodyBones.RightHand;
    public TransformData ikData;
    public TransformData inVehicleIkData;
    [Space]
    public TransformData holsterData;
    public HumanBodyBones holsterBodyPart = HumanBodyBones.RightHand;
    [Space]
    public float fireRate = 60.0f;  // Rounds per minute
    public float burstDelay = 60.0f;  // Delay Between burst fires. Calculated in Rounds per Minute to keep uniformity in calculations
    public float reloadTime = 1.2f;
    public int clipSize = 30;
    [Space]
    public float maxDistance;
    public float maxDamage;
    public AnimationCurve damageByDistanceFalloff;
    [Space]
    public List<FireType> fireTypes = new List<FireType> ();
    [Space]
    public AudioClip audioClipFire;
    public AudioClip audioClipEmptyFire;
    public AudioClip audioClipReload;
    public RecoilData recoilData;
    [Space]
    public GameObject prefab;
    public GameObject muzzlePrefab;
    public Vector3 offsetPosition;
    public Vector3 offsetRotation;
    public Vector3 localScale;
    public Vector3 clipTextLocalPosition;
    public Vector3 clipTextHolsteredLocalPosition;
}

#if UNITY_EDITOR
public static class CopyComponent
{
    public static Vector3 pos;
    public static Vector3 rot;
    public static Vector3 scale;

    [MenuItem( "Tools/Third Person Synty Character/Copyer/Copy Transform" )]
    public static void CopyTransform ()
    {
        pos = Selection.gameObjects[0].GetComponent<Transform> ().localPosition;
        rot = Selection.gameObjects[0].GetComponent<Transform> ().localEulerAngles;
        scale = Selection.gameObjects[0].GetComponent<Transform> ().localScale;
    }

    [MenuItem ( "Tools/Third Person Synty Character/Copyer/Paste Weapon Transform" )]
    public static void PasteWeaponTransform ()
    {
        ((WeaponData)Selection.objects[0]).offsetPosition = pos;
        ((WeaponData)Selection.objects[0]).offsetRotation = rot;
        ((WeaponData)Selection.objects[0]).localScale = scale;
        EditorUtility.SetDirty ( Selection.objects[0] );
        AssetDatabase.SaveAssets ();
    }

    [MenuItem ( "Tools/Third Person Synty Character/Copyer/Paste Transform Data" )]
    public static void PasteTransformData ()
    {
        ((TransformData)Selection.objects[0]).position = pos;
        ((TransformData)Selection.objects[0]).eulerAngles = rot;
        ((TransformData)Selection.objects[0]).localScale = scale;
        EditorUtility.SetDirty ( Selection.objects[0] );
        AssetDatabase.SaveAssets ();
    }
}
#endif
