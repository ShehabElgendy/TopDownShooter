using UnityEngine;

public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    ShotGun,
    Rifle
}

public enum ShootType
{
    Single,
    Auto
}

[System.Serializable]
public class Weapon
{
    public WeaponType WeaponType;

    [Space]

    #region Regular Mode Variables
    public ShootType ShootType;
    public int BulletPerShoot { get; private set; }
    private float DefaultFireRate;
    public float FireRate = 1;
    private float lastShotTime;
    #endregion

    [Header("Ammo Details")]
    public int BulletsInMagazine;
    public int MagazineCapacity;
    public int TotalReserveAmmo;

    #region Weapon Generics Variables
    public float ReloadSpeed { get; private set; }
    public float EquipSpeed { get; private set; }
    public float GunDistance { get; private set; }
    public float CameraDistance { get; private set; }
    #endregion

    #region Burst Mode Variables
    public float BurstFireDelay { get; private set; }
    public bool BurstActive;
    private bool BurstAvailable;
    private int BurstBulletPerShot;
    private float BurstFireRate;
    #endregion

    #region Weapon Spread Variables
    [Header("Spread")]
    private float BaseSpread = 1;
    private float MaxSpread = 3;
    private float SpreadIncreaseRate = 0.15f;
    #endregion

    private float currentSpread = 2;
    private float lastSpreadUpdateTime;
    private float spreadCoolDown = 1;

    public WeaponData WeaponData { get; private set; }

    public Weapon(WeaponData weaponData)
    {
        BulletsInMagazine = weaponData.BulletsInMagazine;
        MagazineCapacity = weaponData.MagazineCapacity;
        TotalReserveAmmo = weaponData.TotalReserveAmmo;

        FireRate = weaponData.FireRate;
        WeaponType = weaponData.WeaponType;

        BulletPerShoot = weaponData.BurstBulletPerShot;
        ShootType = weaponData.ShootType;

        BurstAvailable = weaponData.BurstAvailable;
        BurstBulletPerShot = weaponData.BurstBulletPerShot;
        BurstFireRate = weaponData.BurstFireRate;
        BurstFireDelay = weaponData.BurstFireDelay;

        BaseSpread = weaponData.BaseSpread;
        MaxSpread = weaponData.MaxSpread;
        SpreadIncreaseRate = weaponData.SpreadIncreaseRate;

        ReloadSpeed = weaponData.ReloadSpeed;
        EquipSpeed = weaponData.EquipSpeed;
        GunDistance = weaponData.GunDistance;
        CameraDistance = weaponData.CameraDistance;

        DefaultFireRate = FireRate;

        this.WeaponData = weaponData;
    }


    #region Burst Methods

    public bool BurstActivated()
    {
        if (WeaponType == WeaponType.ShotGun)
        {
            BurstFireDelay = 0;
            return true;
        }
        return BurstActive;
    }
    public void ToggleBurst()
    {
        if (!BurstAvailable) return;

        BurstActive = !BurstActive;

        if (BurstActive)
        {
            BulletPerShoot = BurstBulletPerShot;
            FireRate = BurstFireRate;
        }
        else
        {
            BulletPerShoot = 1;
            FireRate = DefaultFireRate;
        }
    }

    #endregion

    #region Spread Methods
    public Vector3 ApplySpread(Vector3 originalDir)
    {
        UpdateSpread();

        float randmoizedValue = Random.Range(-currentSpread, currentSpread);

        Quaternion spreadRotation = Quaternion.Euler(randmoizedValue, randmoizedValue, randmoizedValue);

        return spreadRotation * originalDir;
    }

    private void UpdateSpread()
    {
        if (Time.time > lastSpreadUpdateTime + spreadCoolDown)
            currentSpread = BaseSpread;
        else
            IncreaseSpread();

        lastSpreadUpdateTime = Time.time;
    }

    private void IncreaseSpread()
    {
        currentSpread = Mathf.Clamp(currentSpread + SpreadIncreaseRate, BaseSpread, MaxSpread);
    }
    #endregion
    public bool CanShoot() => HaveEnoughBullets() && ReadyToFire();

    private bool ReadyToFire()
    {
        if (Time.time > lastShotTime + 1 / FireRate)
        {
            lastShotTime = Time.time;
            return true;
        }

        return false;
    }

    #region Reload Methods

    public bool CanReload()
    {
        if (BulletsInMagazine == MagazineCapacity) return false;
        if (TotalReserveAmmo > 0) return true;

        return false;
    }

    public void RefillBullets()
    {
        //To add bullets in magazine to the total ammount of bullets in reserve
        //TotalReserveAmmo += BulletsInMagazine;


        //To drop the magazine and replace with the bullets to reload
        int bulletsToReload = MagazineCapacity;

        if (bulletsToReload > TotalReserveAmmo)
            bulletsToReload = TotalReserveAmmo;

        TotalReserveAmmo -= bulletsToReload;
        BulletsInMagazine = bulletsToReload;

        if (TotalReserveAmmo < 0)
            TotalReserveAmmo = 0;

    }

    private bool HaveEnoughBullets() => BulletsInMagazine > 0;

    #endregion
}
