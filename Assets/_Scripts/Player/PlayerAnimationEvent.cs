using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private PlayerWeaponVisuals visualController;
    private PlayerWeaponController weaponController;

    private void Awake()
    {
        visualController = GetComponentInParent<PlayerWeaponVisuals>();
        weaponController = GetComponentInParent<PlayerWeaponController>();
    }

    public void ReloadIsOver()
    {
        visualController.MaximizeRigWieght();
        weaponController.CurrentWeapon().RefillBullets();
        weaponController.SetWeaponReady(true);
    }

    public void ReturnRig()
    {
        visualController.MaximizeRigWieght();
        visualController.MaximizeLeftHandIKWieght();
    }

    public void WeaponEquipOver()
    {
        weaponController.SetWeaponReady(true);
    }

    public void SwitchOnWeaponModel()
    {
        visualController.SwitchOnCurrentWeaponModel();
    }
}
