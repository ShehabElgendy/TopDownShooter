using System.Collections.Generic;
using UnityEngine;

public enum AmmoBoxType { SmallBox, BigBox }

[System.Serializable]
public struct AmmoData
{
    public WeaponType weaponType;
    [Range(10, 100)]
    public int minAmount;
    [Range(10, 100)]
    public int maxAmount;
}

public class PickUpAmmo : Interactable
{
    [SerializeField]
    private AmmoBoxType boxType;

    [SerializeField]
    private List<AmmoData> smallBoxAmmo;

    [SerializeField]
    private List<AmmoData> BigBoxAmmo;

    [SerializeField]
    private GameObject[] boxModel;

    private void Start()
    {
        SetupBoxModel();
    }

    public override void Interaction()
    {
        List<AmmoData> currentAmmoList = smallBoxAmmo;

        if(boxType == AmmoBoxType.BigBox)
            currentAmmoList = BigBoxAmmo;

        foreach (AmmoData ammo in currentAmmoList)
        {
            Weapon weapon = weaponController.WeaponInSlots(ammo.weaponType);

            AddBulletsToWeapon(weapon, GetBulletAmount(ammo));
        }

        ObjectPool.Instance.ReturnObject(gameObject);
    }

    private int GetBulletAmount(AmmoData ammoData)
    {
        float min = Mathf.Min(ammoData.minAmount,ammoData.maxAmount);
        float max = Mathf.Max(ammoData.minAmount,ammoData.maxAmount);

        float randomAmmo = Random.RandomRange(min, max);

        return Mathf.RoundToInt(randomAmmo);
    }

    private void AddBulletsToWeapon(Weapon weapon, int amount)
    {
        if (weapon == null) return;

        weapon.TotalReserveAmmo += amount;
    }
    private void SetupBoxModel()
    {
        for (int i = 0; i < boxModel.Length; i++)
        {
            boxModel[i].SetActive(false);

            if (i == (int)boxType)
            {
                boxModel[i].gameObject.SetActive(true);
                UpdateMeshAndMaterial(boxModel[i].GetComponent<MeshRenderer>());
            }
        }
    }
}
