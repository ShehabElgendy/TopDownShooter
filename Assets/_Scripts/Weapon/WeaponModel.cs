using UnityEngine;
public enum EquipType { SideEquip, BackEquip }
public enum HoldType {None, CommonHold, LowHold, HighHold }

public class WeaponModel : MonoBehaviour
{
    public WeaponType WeaponType;
    public EquipType EquipType;
    public HoldType HoldType;

    public Transform GunPoint;
    public Transform HoldPoint;
}
