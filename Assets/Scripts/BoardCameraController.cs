using UnityEngine;
using Cinemachine;

public class BoardCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private Transform cameraRig;   
    [SerializeField] private float rotationSmooth = 4f;
    [SerializeField] private float followSmooth = 20f;

    private Transform target;
    private float targetRotationY;


    void Update()
    {
        if (target == null || cameraRig == null) return;

        cameraRig.position = Vector3.Lerp(
            cameraRig.position,
            target.position,
            followSmooth * Time.deltaTime
        );

        Quaternion desiredRot = Quaternion.Euler(45f, targetRotationY, 0f);
        cameraRig.rotation = Quaternion.Lerp(
            cameraRig.rotation,
            desiredRot,
            rotationSmooth * Time.deltaTime
        );
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        if (vcam != null)
        {
            vcam.Follow = cameraRig;
            vcam.LookAt = target;
        }
    }

    public void SetBoxRotation(float camRotationY)
    {
        targetRotationY = camRotationY;
    }

    public Transform GetActualTarget() { return target; }
}