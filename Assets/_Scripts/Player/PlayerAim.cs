using System;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Player player;

    private PlayerControls controls;

    [Header("Aim Visuals - Laser")]

    [SerializeField]
    private LineRenderer aimLaser;

    [SerializeField, Range(0, 1)]
    private float laserTipDistance;

    [Header("Aim Controls")]
    [SerializeField]
    private Transform aimTransform;

    [SerializeField]
    private bool isAimingPrecisly;

    [SerializeField]
    private bool isLockingToTarget;


    [Header("Camera Controls")]

    [SerializeField, Range(0.5f, 1f)]
    private float minCameraDistance = 1.5f;

    [SerializeField, Range(1f, 3f)]
    private float maxCameraDistance = 4f;

    [SerializeField]
    private Transform cameraTarget;

    [SerializeField, Range(3f, 5f)]
    private float cameraSensetivity = 5f;

    [Space]

    [SerializeField]
    private LayerMask aimLayerMask;


    private RaycastHit lastKnownMouseHit;

    private Vector2 mouseInput;

    private void Start()
    {
        player = GetComponent<Player>();

        AssignInputEvents();
    }

    private void Update()
    {
        UpdateAimPosition();
        UpdateCameraPosition();
        UpdateAimVisuals();
    }

    private void UpdateAimVisuals()
    {
        aimLaser.enabled = player.weapon.WeaponReady();

        if(!aimLaser.enabled) return;

        WeaponModel weaponModel = player.weaponVisuals.CurrentWeaponModel();
        weaponModel.transform.LookAt(aimTransform);
        weaponModel.GunPoint.LookAt(aimTransform);

        Transform gunPoint = player.weapon.GunPoint();
        Vector3 laserDir = player.weapon.BulletDirection();
        float gunDistance = player.weapon.CurrentWeapon().GunDistance;
        Vector3 endPoint = gunPoint.position + laserDir * gunDistance;

        if (Physics.Raycast(gunPoint.position, laserDir, out RaycastHit hit, gunDistance))
            endPoint = hit.point;

        aimLaser.SetPosition(0, gunPoint.position);
        aimLaser.SetPosition(1, endPoint);
        aimLaser.SetPosition(2, endPoint + laserDir * laserTipDistance);
    }

    private void UpdateAimPosition()
    {
        Transform target = Target();


        if (target != null && isLockingToTarget)
        {
            if (target.GetComponent<Renderer>() != null)
                aimTransform.position = target.GetComponent<Renderer>().bounds.center;
            else
                aimTransform.position = target.position;

            return;
        }


        aimTransform.position = GetMouseHitInfo().point;
        if (!isAimingPrecisly)
            aimTransform.position = new Vector3(aimTransform.position.x, aimTransform.position.y + 1, aimTransform.position.z);
    }




    public Transform Target()
    {
        Transform target = null;

        if (GetMouseHitInfo().transform.GetComponent<Target>() != null)
        {
            target = GetMouseHitInfo().transform;
        }

        return target;
    }

    public Transform Aim() => aimTransform;

    public bool CanAimPrecisly() => isAimingPrecisly;

    public RaycastHit GetMouseHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseInput);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        {
            lastKnownMouseHit = hitInfo;
            return hitInfo;
        }

        return lastKnownMouseHit;
    }

    #region Camera Region

    private void UpdateCameraPosition()
    {
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesiredCameraPosition(), cameraSensetivity * Time.deltaTime);
    }


    private Vector3 DesiredCameraPosition()
    {
        float actualMaxCameraDistance = player.movement.moveInput.y < -0.5f ? minCameraDistance : maxCameraDistance;

        Vector3 desiredCameraPos = GetMouseHitInfo().point;
        Vector3 aimDir = (desiredCameraPos - transform.position).normalized;

        float distanceToDesiredPos = Vector3.Distance(transform.position, desiredCameraPos);
        float clampedDistance = Mathf.Clamp(distanceToDesiredPos, minCameraDistance, actualMaxCameraDistance);

        desiredCameraPos = transform.position + aimDir * clampedDistance;
        desiredCameraPos.y = transform.position.y + 1;

        return desiredCameraPos;
    }

    #endregion
    private void AssignInputEvents()
    {
        controls = player.controls;

        controls.Character.Aim.performed += context => mouseInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => mouseInput = Vector2.zero;
    }
}
