using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Vector3 transformOffset;
    public Vector3 TransformOffset { get => transformOffset; set => transformOffset = value; }

    [SerializeField] private Vector3 rotationOffset;
    public Vector3 RotationOffset { get => rotationOffset; set => rotationOffset = value; }

    [SerializeField] private Transform cameraTransform;
    public Transform CameraTransform { get => cameraTransform; set => cameraTransform = value; }

    [SerializeField] private Transform followTarget;
    public Transform FollowTarget { get => followTarget; set => followTarget = value; }

    #region Monobehaviour
    private void Start()
    {
        transform.parent = null;
        cameraTransform.position = transformOffset;
        cameraTransform.rotation = Quaternion.Euler(rotationOffset);
    }
    #endregion

    #region Methods
    public void DetachFromParent()
    {
        transform.parent = null;
    }

    private void LateUpdate()
    {
        if (followTarget == null) return;

        transform.position = followTarget.position;
    }
    #endregion

}
