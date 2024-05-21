using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpWeapon : Interactable
{
    [SerializeField]
    private WeaponData weaponData;

    [SerializeField]
    private BackupWeaponModel[] models;

    private PlayerWeaponController weaponController;

    private void Start()
    {
        UpdateGameObject();
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
        weaponController.PickupWeapon(weaponData);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (weaponController == null)
            weaponController = other.GetComponent<PlayerWeaponController>();
    }
}
