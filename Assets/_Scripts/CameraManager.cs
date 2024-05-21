using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [Header("Camera Distance")]
    [SerializeField]
    private bool canChangeCameraDistance;

    [SerializeField]
    private float distanceChangeRate;

    private float targetCameraDistance;

    private CinemachineVirtualCamera virtualCam;
    private CinemachineFramingTransposer transposer;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);


        virtualCam = GetComponentInChildren<CinemachineVirtualCamera>();
        transposer = virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Update()
    {
        UpdateCameraDistance();
    }

    private void UpdateCameraDistance()
    {
        if(!canChangeCameraDistance) return;

        float currentDistance = transposer.m_CameraDistance;

        if (Mathf.Abs(targetCameraDistance - currentDistance) < 0.01f)
            return;


        transposer.m_CameraDistance =
            Mathf.Lerp(currentDistance, targetCameraDistance, distanceChangeRate * Time.deltaTime);
    }

    public void ChangeCameraDistance(float distance) => targetCameraDistance = distance;
}
