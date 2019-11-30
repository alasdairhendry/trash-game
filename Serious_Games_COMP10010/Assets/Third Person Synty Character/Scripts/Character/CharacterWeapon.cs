using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CharacterWeapon : MonoBehaviour
{
    public Character character { get; protected set; }

    public WeaponData currentWeaponData { get; protected set; }
    public GameObject currentWeaponObject { get; protected set; }
    public WeaponData.FireType currentFireType;
    public Transform muzzlePoint { get; protected set; }

    [Range(0.0f, 1.0f)] public float fireCooldown = 0.0f;
    public bool isFiring = false;
    public int currentRounds = 0;
    public float currentReloadTime = 0.0f;

    public bool isHolstered { get; protected set; }

    public bool isEquipped { get { return currentWeaponData != null; } }
    public bool weaponIsPistol { get { if (currentWeaponData == null) return false; return currentWeaponData.weaponType == WeaponData.WeaponType.Pistol; } }
    public TransformData GetCurrentIKData { get { return currentWeaponData.ikData; } }
    public Transform WeaponHandTarget
    {
        get
        {
            if (currentWeaponData == null) return null;
            if (currentWeaponObject == null) return null;
            return currentWeaponObject.transform.Find ( "hand-ik-target" );
        }
    }

    [SerializeField] private WeaponData pistolData;
    [SerializeField] private WeaponData rifleData;

    public System.Action<WeaponData> onWeaponEquiped;
    public System.Action<WeaponData> onWeaponUnequiped;

    public System.Action<WeaponData> onWeaponHolstered;
    public System.Action<WeaponData> onWeaponUnholstered;

    [SerializeField] private TextMeshProUGUI clipText;

    public bool ShouldAim { get; protected set; }
    private float shouldAimCooldown = 1.0f;

    [SerializeField] private float minHitDistance = 2.0f;

    public Vector3 AimPosition = new Vector3 ();
    public RaycastHit aimHit;
    public bool hasAimHit = false;

    public void CheckAimPosition ()
    {
        bool setAimPosition = false;
        bool setHit = false;

        if (character.IsAiming)
        {
            Ray ray = new Ray ( character.cCameraController.cameraTransform.position, character.cCameraController.cameraTransform.forward );
            RaycastHit[] hits;

            hits = Physics.RaycastAll ( ray, currentWeaponData.maxDistance );

            hits = hits.OrderBy ( x => x.distance ).ToArray ();

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].distance < minHitDistance) continue;

                Character character = hits[i].collider.gameObject.GetComponent<Character> ();

                if (character != null)
                {
                    if (!character.IsAI)
                    {
                        continue;
                    }
                }

                AimPosition = hits[i].point;
                setAimPosition = true;

                aimHit = hits[i];
                setHit = true;
                hasAimHit = true;
                break;
            }
        }

        if (!setAimPosition)
        {
            AimPosition = character.cCameraController.cameraTransform.position + (character.cCameraController.cameraTransform.forward * 15);
        }

        if (!setHit)
        {
            hasAimHit = false;
        }

    }

    public void EquipPistol ()
    {
        Equip ( pistolData );
    }

    public void EquipRifle ()
    {
        Equip ( rifleData );
    }

    private int currentBurstCounter = 0;
    private float currentBurstCooldown = 0.0f;

    private void Awake ()
    {
        character = GetComponent<Character> ();
        EquipPistol ();
        SetHolsterState ();
    }

    private void Update ()
    {
        if (character.IsAI) return;

        CheckFireInput ();
        CheckIsFiring ();
        CheckBurstCooldown ();
        CheckReload ();    

        CheckAimPosition ();
        CheckShouldAim ();
        CheckWeaponSwitch ();

        SetWeaponUI ();

        if (Input.GetKeyDown ( KeyCode.H ))
        {
            SetHolsterState ( );
        }
    }

    private void SetWeaponUI ()
    {
        if (currentWeaponData == null || character.currentState != Character.State.Standing)
        {
            clipText.text = "";
        }
        else
        {
            Vector3 position = currentWeaponObject.transform.TransformPoint ( currentWeaponData.clipTextLocalPosition );

            if (isHolstered)
            {
                clipText.text = "[H]";
                position = currentWeaponObject.transform.TransformPoint ( currentWeaponData.clipTextHolsteredLocalPosition );
            }
            else
            {
                if (character.IsAiming)
                {
                    if (currentReloadTime > 0.0f)
                    {
                        clipText.text = "-----";
                    }
                    else
                    {
                        clipText.text = currentRounds.ToString ( "00" ) + " / " + currentWeaponData.clipSize.ToString ( "00" );
                    }
                }
                else
                {
                    clipText.text = "";

                }
            }

            clipText.transform.position = position;
        }

    }

    private void CheckFireInput ()
    {
        if (isHolstered || currentWeaponData == null) { isFiring = false; return; }

        switch (currentFireType)
        {
            case WeaponData.FireType.Single:
                if (Input.GetMouseButtonDown ( 0 ))
                {
                    TryFire ();

                    ShouldAim = true;
                    shouldAimCooldown = 1.0f;
                }
                break;

            case WeaponData.FireType.Burst:                
                if (Input.GetMouseButtonDown ( 0 ))
                {
                    TryFire ();

                    ShouldAim = true;
                    shouldAimCooldown = 1.0f;
                }
                break;

            case WeaponData.FireType.Auto:
                if (Input.GetMouseButton ( 0 ))
                {
                    TryFire ();

                    ShouldAim = true;
                    shouldAimCooldown = 1.0f;
                }
                else
                {
                    isFiring = false;
                }
                break;
        }
    }

    private void TryFire ()
    {
        if (fireCooldown > 0.0f) return;
        if (currentFireType == WeaponData.FireType.Burst && currentBurstCooldown > 0.0f) return;
        if (currentReloadTime > 0.0f) { isFiring = false;  return; }
        if (!character.IsAiming) return;

        isFiring = true;
    }

    private void CheckIsFiring ()
    {
        if (fireCooldown > 0.0f)
        {
            fireCooldown -= Time.deltaTime;

            if (fireCooldown <= 0.0f)
                fireCooldown = 0.0f;

            return;
        }

        if (currentReloadTime > 0.0f) { isFiring = false; return; }

        if (isFiring)
        {
            Fire ();
        }
    }

    private void CheckBurstCooldown ()
    {
        if (currentBurstCooldown > 0.0f)
        {
            currentBurstCooldown -= Time.deltaTime;

            if (currentBurstCooldown <= 0.0f)
                currentBurstCooldown = 0.0f;

            return;
        }
    }

    private void CheckReload ()
    {
        if(currentReloadTime > 0.0f)
        {
            currentReloadTime -= Time.deltaTime;

            if(currentReloadTime <= 0.0f)
            {
                currentReloadTime = 0.0f;
                currentRounds = currentWeaponData.clipSize;
            }
        }
        else
        {
            if (Input.GetKeyDown ( KeyCode.R ))
            {
                Reload ();
            }
        }
    }

    public void Reload ()
    {
        if (currentReloadTime > 0.0f) return;
        if (isHolstered || currentWeaponData == null) { return; }
        currentReloadTime = currentWeaponData.reloadTime;
        SoundEffect.Play ( currentWeaponData.audioClipReload );
    }

    public void Fire ()
    {
        if (!isFiring) return;

        if(currentRounds > 0)
        {
            SoundEffect.Play ( currentWeaponData.audioClipFire );
            character.cIK.AddRecoil ( currentWeaponData.recoilData );
            currentRounds--;
            RayBullet ();

        }
        else
        {
            SoundEffect.Play ( currentWeaponData.audioClipEmptyFire );
            character.cIK.AddRecoil ( currentWeaponData.recoilData );
        }

        fireCooldown = 60.0f / currentWeaponData.fireRate;

        switch (currentFireType)
        {
            case WeaponData.FireType.Single:
                isFiring = false;
                break;

            case WeaponData.FireType.Burst:

                currentBurstCounter++;
                if (currentBurstCounter == 3)
                {
                    isFiring = false;
                    currentBurstCooldown = 60.0f / currentWeaponData.burstDelay;
                    currentBurstCounter = 0;
                }

                break;

            case WeaponData.FireType.Auto:
                if (!Input.GetMouseButton ( 0 ))
                {
                    isFiring = false;
                }
                break;
        }
    }

    [SerializeField] private GameObject ricochetPrefab;

    private void RayBullet ()
    {
        if (hasAimHit)
        {
            float distNrm = Mathf.InverseLerp ( 0.0f, currentWeaponData.maxDistance, aimHit.distance );
            float damage = currentWeaponData.damageByDistanceFalloff.Evaluate ( distNrm ) * currentWeaponData.maxDamage;

            GameObject go = Instantiate ( ricochetPrefab );
            go.transform.position = aimHit.point;
            go.transform.rotation = Quaternion.LookRotation ( -character.cCameraController.cameraTransform.forward );

            GameObject ps = Instantiate ( currentWeaponData.muzzlePrefab );
            ps.transform.SetParent ( muzzlePoint );
            ps.transform.localPosition = Vector3.zero;
            ps.transform.localRotation = Quaternion.identity;
        }
    }

    private void CheckWeaponSwitch ()
    {
        if (Input.GetKeyDown ( KeyCode.Alpha1 ))
        {
            EquipPistol ();
        }
        if (Input.GetKeyDown ( KeyCode.Alpha2 ))
        {
            EquipRifle ();
        }
    }   

    private void CheckShouldAim ()
    {
        if (shouldAimCooldown > 0.0f)
        {
            shouldAimCooldown -= Time.deltaTime;
            if (shouldAimCooldown <= 0.0f)
            {
                shouldAimCooldown = 0.0f;
                ShouldAim = false;
            }
        }
    }

    public void SetHolsterState()
    {
        if (!isEquipped) return;

        isHolstered = !isHolstered;

        if (isHolstered)
        {
            currentWeaponObject.transform.SetParent ( GetComponent<Animator> ().GetBoneTransform ( currentWeaponData.holsterBodyPart ) );
            currentWeaponData.holsterData.SetLocal ( currentWeaponObject.transform );

            onWeaponHolstered?.Invoke ( currentWeaponData );
        }
        else
        {
            currentWeaponObject.transform.SetParent ( GetComponent<Animator> ().GetBoneTransform ( currentWeaponData.activeBodyPart ).Find ( "weapon-holder" ) );
            currentWeaponObject.transform.localPosition = currentWeaponData.offsetPosition;
            currentWeaponObject.transform.localEulerAngles = currentWeaponData.offsetRotation;
            currentWeaponObject.transform.localScale = currentWeaponData.localScale;

            onWeaponUnholstered?.Invoke ( currentWeaponData );
        }
    }

    public void Equip (WeaponData weaponData)
    {
        if (currentWeaponData)
        {
            Unequip ();
        }

        currentWeaponData = weaponData;
        currentWeaponObject = Instantiate ( weaponData.prefab );

        if (isHolstered)
        {
            currentWeaponObject.transform.SetParent ( GetComponent<Animator> ().GetBoneTransform ( currentWeaponData.holsterBodyPart ) );
            currentWeaponData.holsterData.SetLocal ( currentWeaponObject.transform );
        }
        else
        {
            currentWeaponObject.transform.SetParent ( GetComponent<Animator> ().GetBoneTransform ( weaponData.activeBodyPart ).Find ( "weapon-holder" ) );
            currentWeaponObject.transform.localPosition = weaponData.offsetPosition;
            currentWeaponObject.transform.localEulerAngles = weaponData.offsetRotation;
            currentWeaponObject.transform.localScale = weaponData.localScale;
        }

        onWeaponEquiped?.Invoke ( currentWeaponData );

        currentRounds = currentWeaponData.clipSize;
        currentReloadTime = 0.0f;
        currentFireType = currentWeaponData.fireTypes[0];
        muzzlePoint = currentWeaponObject.transform.Find ( "muzzle-point" );
    }

    public void ToggleFireType ()
    {
        int index = currentWeaponData.fireTypes.IndexOf ( currentFireType );
        index++;

        if(index >= currentWeaponData.fireTypes.Count)
        {
            index = 0;
        }

        currentFireType = currentWeaponData.fireTypes[index];
    }

    public void Unequip ()
    {
        WeaponData w = currentWeaponData;
        currentWeaponData = null;

        if (currentWeaponObject)
        {
            currentWeaponObject.transform.SetParent ( null );
            currentWeaponObject.transform.GetChild ( 0 ).gameObject.AddComponent<BoxCollider> ();
            currentWeaponObject.AddComponent<Rigidbody> ();
        }

        onWeaponUnequiped?.Invoke ( w );
    }
}
