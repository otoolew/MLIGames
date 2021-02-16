using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Transform CameraToFace { get; set; }

    private void Start()
    {
        CameraToFace = UnityEngine.Camera.main.transform;
    }
    private void Update()
    {
        Vector3 direction = CameraToFace.transform.forward;
        transform.forward = -direction;
    }

}
