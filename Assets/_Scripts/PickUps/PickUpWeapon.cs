using UnityEngine;

public class PickUpWeapon : Interactable
{
    [SerializeField]
    private WeaponData weaponData;

    [SerializeField]
    private Weapon weapon;

    [SerializeField]
    private BackupWeaponModel[] models;

    private bool isOldWeapon;

    private void Start()
    {
        if (!isOldWeapon)
            weapon = new Weapon(weaponData);

        SetupGameObject();
    }

    public void SetupPickUpWeapon(Weapon weapon, Transform transform)
    {
        isOldWeapon = true;
        this.weapon = weapon;
        weaponData = weapon.WeaponData;

        this.transform.position = transform.position + new Vector3(0, 0.5f, 0);
    }

    [ContextMenu("Update Item Model")]
    public void SetupGameObject()
    {
        gameObject.name = "Pickup_Weapon - " + weaponData.WeaponType.ToString();
        SetupWeaponModel();
    }

    private void SetupWeaponModel()
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
}
