using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Weapon Data", menuName ="Weapon System/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string WeaponName;

    [Header("Ammo Details")]
    public int BulletsInMagazine;
    public int MagazineCapacity;
    public int TotalReserveAmmo;

    [Header("Regular Shot")]
    public ShootType ShootType;
    public int BulletsPerShoot;
    public float FireRate;

    [Header("Burst Shot")]
    public bool BurstAvailable;
    public bool BurstActive;
    public int BurstBulletPerShot =1;
    public float BurstFireRate;
    public float BurstFireDelay =0.1f;

    [Header("Weapon Spread")]
    public float BaseSpread;
    public float MaxSpread;
    public float SpreadIncreaseRate = 0.15f;

    [Header("Weapon Generics")]
    public WeaponType WeaponType;
    [Range(1, 3)]
    public float ReloadSpeed = 1;
    [Range(1, 3)]
    public float EquipSpeed = 1;
    [Range(4, 8)]
    public float GunDistance = 4;
    [Range(4, 8)]
    public float CameraDistance = 6;
}
