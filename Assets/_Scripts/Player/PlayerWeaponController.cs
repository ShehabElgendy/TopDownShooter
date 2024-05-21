using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;

    [SerializeField]
    private WeaponData defaultWeaponData;

    [SerializeField]
    private Weapon currentWeapon;

    [Header("Bullet Details")]
    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private float bulletSpeed;

    [SerializeField]
    private Transform weaponHolder;

    [SerializeField]
    private Transform aim;

    [Header("Inventory")]
    [SerializeField]
    private int maxSlots = 2;

    [SerializeField]
    private List<Weapon> weaponSlots;

    [SerializeField]
    private GameObject weaponPickupPrefab;

    private bool weaponReady;

    private bool isShooting;

    private const float REFERENCE_BULLET_SPEED = 20f;


    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();

        Invoke(nameof(EquipStartingWeapon), 0.1f);
    }

    private void Update()
    {
        if (isShooting)
        {
            Shoot();
        }
    }

    private void EquipStartingWeapon()
    {
        weaponSlots[0] = new Weapon(defaultWeaponData);
        EquipWeapon(0);
    }


    #region Slots Managemt - Pickup - Equip - Ready - Drop
    private void EquipWeapon(int index)
    {
        if (index >= weaponSlots.Count) return;

        SetWeaponReady(false);
        currentWeapon = weaponSlots[index];
        player.weaponVisuals.PlayWeaponEquipAnimation();

        CameraManager.Instance.ChangeCameraDistance(currentWeapon.CameraDistance);
    }

    public void PickupWeapon(Weapon newWeapon)
    {
        if (WeaponInSlots(newWeapon.WeaponType) != null)
        {
            WeaponInSlots(newWeapon.WeaponType).TotalReserveAmmo += newWeapon.BulletsInMagazine;
            return;
        }



        if (weaponSlots.Count >= maxSlots && newWeapon.WeaponType != currentWeapon.WeaponType)
        {
            int weaponIndex = weaponSlots.IndexOf(currentWeapon);
            player.weaponVisuals.SwitchOffWeaponModels();
            weaponSlots[weaponIndex] = newWeapon;
            CreateWeaponOnGround();
            EquipWeapon(weaponIndex);
            return;
        }

        weaponSlots.Add(newWeapon);
        player.weaponVisuals.SwitchOnBackupWeaponModel();
    }

    private void DropWeapon()
    {
        if (HasOnlyOneWeapon()) return;

        CreateWeaponOnGround();
        weaponSlots.Remove(currentWeapon);
        EquipWeapon(0);
    }

    private void CreateWeaponOnGround()
    {
        GameObject droppedWapon = ObjectPool.Instance.GetObject(weaponPickupPrefab);
        droppedWapon.GetComponent<PickUpWeapon>()?.SetupPickUpWeapon(currentWeapon, transform);
    }

    public void SetWeaponReady(bool ready) => weaponReady = ready;

    public bool WeaponReady() => weaponReady;

    #endregion

    private IEnumerator BurstFire()
    {
        SetWeaponReady(false);
        for (int i = 1; i <= currentWeapon.BulletPerShoot; i++)
        {
            FireSingleBullet();
            yield return new WaitForSeconds(currentWeapon.BurstFireDelay);

            if (i >= currentWeapon.BulletPerShoot)
            {
                SetWeaponReady(true);
            }
        }
    }

    private void Shoot()
    {
        if (!WeaponReady()) return;
        if (!currentWeapon.CanShoot()) return;

        if (currentWeapon.ShootType == ShootType.Single)
            isShooting = false;

        player.weaponVisuals.PlayFireAnimation();

        if (currentWeapon.BurstActivated())
        {
            StartCoroutine(BurstFire());
            return;
        }

        FireSingleBullet();
    }

    private void FireSingleBullet()
    {
        currentWeapon.BulletsInMagazine--;

        GameObject newBullet = ObjectPool.Instance.GetObject(bulletPrefab);

        newBullet.transform.position = GunPoint().position;
        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        bulletScript.BulletSetup(currentWeapon.GunDistance);

        Vector3 bulletsDir = currentWeapon.ApplySpread(BulletDirection());

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.GetComponent<Rigidbody>().velocity = bulletsDir * bulletSpeed;
    }

    private void Reload()
    {
        SetWeaponReady(false);
        player.weaponVisuals.PlayReloadAnimation();
    }

    public Vector3 BulletDirection()
    {
        Transform aim = player.aim.Aim();

        Vector3 direction = (aim.position - GunPoint().position).normalized;

        if (!player.aim.CanAimPrecisly() && player.aim.Target() == null)
            direction.y = 0;

        return direction;
    }

    public bool HasOnlyOneWeapon() => weaponSlots.Count <= 1;

    public Weapon WeaponInSlots(WeaponType weaponType)
    {
        foreach (Weapon weapon in weaponSlots)
        {
            if (weapon.WeaponType == weaponType)
                return weapon;
        }

        return null;
    }

    public Weapon CurrentWeapon() => currentWeapon;

    public Transform GunPoint() => player.weaponVisuals.CurrentWeaponModel().GunPoint;

    #region Input Events
    private void AssignInputEvents()
    {
        PlayerControls controls = player.controls;

        controls.Character.Fire.performed += context => isShooting = true;
        controls.Character.Fire.canceled += context => isShooting = false
        ;
        controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += context => EquipWeapon(1);
        controls.Character.EquipSlot3.performed += context => EquipWeapon(2);
        controls.Character.EquipSlot4.performed += context => EquipWeapon(3);
        controls.Character.EquipSlot5.performed += context => EquipWeapon(4);

        controls.Character.DropCurrentWeapon.performed += context => DropWeapon();
        controls.Character.ToggleWeaponMode.performed += context => currentWeapon.ToggleBurst();

        controls.Character.Reload.performed += context =>
        {
            if (currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }

        };
    }
    #endregion
}












//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerWeaponController : MonoBehaviour
//{
//    private Player player;

//    [SerializeField]
//    private GameObject bulletPrefab;

//    [SerializeField]
//    private float bulletSpeed;

//    [SerializeField]
//    private Transform gunPoint;

//    [SerializeField]
//    private Transform weaponHolder;

//    [SerializeField]
//    private Transform aim;

//    [SerializeField]
//    private float fireRate = 0.2f;

//    private float nextFireTime;

//    private const float REFERENCE_BULLET_SPEED = 20f;

//    private static int FIRE_PARAMETER = Animator.StringToHash("Fire");

//    bool isFiring = false;

//    private void Start()
//    {
//        player = GetComponent<Player>();

//        player.controls.Character.Fire.performed += context => StartFiring();
//        player.controls.Character.Fire.canceled += context => StopFiring();
//    }

//    private void Update()
//    {
//        if (isFiring && Time.time >= nextFireTime)
//        {
//            Shoot();
//            nextFireTime = Time.time + fireRate; // Add this line
//        }
//    }

//    private void Shoot()
//    {
//        GameObject newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));

//        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

//        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;

//        rbNewBullet.GetComponent<Rigidbody>().velocity = BulletDirection() * bulletSpeed;

//        Destroy(newBullet, 10f);

//        GetComponentInChildren<Animator>().SetTrigger(FIRE_PARAMETER);
//    }

//    public Vector3 BulletDirection()
//    {
//        Transform aim = player.aim.Aim();

//        Vector3 direction = (aim.position - gunPoint.position).normalized;

//        if (!player.aim.CanAimPrecisly() && player.aim.Target() == null)
//            direction.y = 0;

//        //weaponHolder.LookAt(aim);
//        //gunPoint.LookAt(aim);

//        return direction;
//    }

//    public Transform GunPoint() => gunPoint;


//    private void StartFiring()
//    {
//        isFiring = true;
//    }

//    private void StopFiring()
//    {
//        isFiring = false;
//    }
//}