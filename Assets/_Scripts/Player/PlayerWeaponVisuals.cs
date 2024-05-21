using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisuals : MonoBehaviour
{
    [SerializeField]
    private WeaponModel[] weaponModels;

    [SerializeField]
    private BackupWeaponModel[] backupWeaponModels;

    [Header("Rig")]

    [SerializeField]
    private float rigWeightIncreaseRate;

    private bool shouldIncreaseRigWeight;

    private bool shouldIncreaseLeftHandIKWeight;

    [Header("Left Hand IK")]

    [SerializeField]
    private TwoBoneIKConstraint leftHandIK;

    [SerializeField]
    private Transform leftHandIKTarget;

    [SerializeField]
    private float leftHandIKIncreaseRate;

    private static int WEAOPON_EQUIPTYPE_PARAMETER = Animator.StringToHash("WeaponEquipType");
    private static int WEAPON_EQUIP_PARAMETER = Animator.StringToHash("EquipWeapon");
    private static int EQUIPPING_SPEED_PARAMETER = Animator.StringToHash("EquipSpeed");
    private static int RELOAD_PARAMETER = Animator.StringToHash("Reload");
    private static int RELOAD_SPEED_PARAMETER = Animator.StringToHash("ReloadSpeed");
    private static int FIRE_PARAMETER = Animator.StringToHash("Fire");

    private Animator anim;

    private Rig rig;

    private Player player;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();
        player = GetComponentInChildren<Player>();
        weaponModels = GetComponentsInChildren<WeaponModel>(true);
        backupWeaponModels = GetComponentsInChildren<BackupWeaponModel>(true);
    }
    private void Start()
    {

    }

    private void Update()
    {
        UpdateRigWeight();
        UpdateLeftHandIKWeight();
    }

    public void PlayFireAnimation() => anim.SetTrigger(FIRE_PARAMETER);

    public void PlayReloadAnimation()
    {
        float reloadSpeed = player.weapon.CurrentWeapon().ReloadSpeed;

        anim.SetFloat(RELOAD_SPEED_PARAMETER, reloadSpeed);
        anim.SetTrigger(RELOAD_PARAMETER);
        ReduceRigWieght();
    }

    public void PlayWeaponEquipAnimation()
    {
        EquipType equipType = CurrentWeaponModel().EquipType;

        float equipSpeed = player.weapon.CurrentWeapon().EquipSpeed;

        leftHandIK.weight = 0;
        ReduceRigWieght();
        anim.SetTrigger(WEAPON_EQUIP_PARAMETER);
        anim.SetFloat(WEAOPON_EQUIPTYPE_PARAMETER, ((float)equipType));
        anim.SetFloat(EQUIPPING_SPEED_PARAMETER, equipSpeed);
    }


    public void SwitchOnCurrentWeaponModel()
    {
        int animationIndex = (int)CurrentWeaponModel().HoldType;

        SwitchOffWeaponModels();

        SwitchOffBackupWeaponModels();

        if (!player.weapon.HasOnlyOneWeapon())
            SwitchOnBackupWeaponModel();

        SwitchAnimatorLayer(animationIndex);
        CurrentWeaponModel().gameObject.SetActive(true);
        AttachLeftHand();
    }

    public void SwitchOffWeaponModels()
    {
        for (int i = 0; i < weaponModels.Length; i++)
        {
            weaponModels[i].gameObject.SetActive(false);
        }
    }

    private void SwitchOffBackupWeaponModels()
    {
        foreach (BackupWeaponModel backupModel in backupWeaponModels)
        {
            backupModel.Activate(false);
        }
    }

    public void SwitchOnBackupWeaponModel()
    {
        SwitchOffBackupWeaponModels();

        BackupWeaponModel lowHangWeapon = null;
        BackupWeaponModel backHangWeapon = null;
        BackupWeaponModel sideHangWeapon = null;

        foreach (BackupWeaponModel backupModel in backupWeaponModels)
        {
            if (backupModel.WeaponType == player.weapon.CurrentWeapon().WeaponType) continue;

            if (player.weapon.WeaponInSlots(backupModel.WeaponType) != null)
            {
                if (backupModel.HangTypeIs(HangType.LowBackHang))
                    lowHangWeapon = backupModel;

                if (backupModel.HangTypeIs(HangType.BackHang))
                    backHangWeapon = backupModel;

                if (backupModel.HangTypeIs(HangType.SideHang))
                    sideHangWeapon = backupModel;
            }

            lowHangWeapon?.Activate(true);
            backHangWeapon?.Activate(true);
            sideHangWeapon?.Activate(true);
        }
    }

    private void SwitchAnimatorLayer(int layerIndex)
    {
        for (int i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }

        anim.SetLayerWeight(layerIndex, 1);
    }

    public WeaponModel CurrentWeaponModel()
    {
        WeaponModel weaponModel = null;

        WeaponType weaponType = player.weapon.CurrentWeapon().WeaponType;

        for (int i = 0; i < weaponModels.Length; i++)
        {
            if (weaponModels[i].WeaponType == weaponType)
            {
                weaponModel = weaponModels[i];
            }
        }
        return weaponModel;
    }

    #region Animation Rigging Methods

    private void UpdateLeftHandIKWeight()
    {
        if (shouldIncreaseLeftHandIKWeight)
        {
            leftHandIK.weight += leftHandIKIncreaseRate * Time.deltaTime;

            if (leftHandIK.weight >= 1)
                shouldIncreaseLeftHandIKWeight = false;
        }
    }

    private void AttachLeftHand()
    {
        Transform targetTransform = CurrentWeaponModel().HoldPoint;

        leftHandIKTarget.localPosition = targetTransform.localPosition;
        leftHandIKTarget.localRotation = targetTransform.localRotation;
    }

    private void UpdateRigWeight()
    {
        if (shouldIncreaseRigWeight)
        {
            rig.weight += rigWeightIncreaseRate * Time.deltaTime;

            if (rig.weight >= 1)
                shouldIncreaseRigWeight = false;
        }
    }

    private void ReduceRigWieght()
    {
        rig.weight = 0.15f;
    }

    public void MaximizeRigWieght() => shouldIncreaseRigWeight = true;

    public void MaximizeLeftHandIKWieght() => shouldIncreaseLeftHandIKWeight = true;

    #endregion

}
