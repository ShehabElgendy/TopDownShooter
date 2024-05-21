using UnityEngine;

public class PickUpWeapon : Interactable
{
    [SerializeField]
    private WeaponData weaponData;

    [SerializeField]
    private Weapon weapon;

    [SerializeField]
    private BackupWeaponModel[] models;

    private PlayerWeaponController weaponController;

    private bool isOldWeapon;

    private void Start()
    {
        if (!isOldWeapon)
            weapon = new Weapon(weaponData);

        UpdateGameObject();
    }

    public void SetupPickUpWeapon(Weapon weapon, Transform transform)
    {
        isOldWeapon = true;
        this.weapon = weapon;
        weaponData = weapon.WeaponData;

        this.transform.position = transform.position + new Vector3(0, 0.5f, 0);
    }

    [ContextMenu("Update Item Model")]
    public void UpdateGameObject()
    {
        gameObject.name = "Pickup_Weapon - " + weaponData.WeaponType.ToString();
        UpdateItemModel();
    }

    public void UpdateItemModel()
    {
        foreach (BackupWeaponModel model in models)
        {
            model.gameObject.SetActive(false);

            if (model.WeaponType == weaponData.WeaponType)
            {
                model.gameObject.SetActive(true);
                UpdateMeshAndMaterial(model.GetComponent<MeshRenderer>());
            }
        }
    }

    public override void Interaction()
    {
        weaponController.PickupWeapon(weapon);
        ObjectPool.Instance.ReturnObject(gameObject);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (weaponController == null)
            weaponController = other.GetComponent<PlayerWeaponController>();
    }
}
