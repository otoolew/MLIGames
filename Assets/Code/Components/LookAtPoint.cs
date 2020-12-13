using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookAtPoint : MonoBehaviour
{
    [SerializeField] private bool lookAtMouse;
    public bool LookAtMouse { get => lookAtMouse; set => lookAtMouse = value; }

    [SerializeField] private float rotationSpeed;
    public float RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }

    [SerializeField] private Vector3 offset;
    public Vector3 Offset { get => offset; set => offset = value; }

    [SerializeField] private LayerMask layerMask;
    public LayerMask LayerMask { get => layerMask; set => layerMask = value; }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (LookAtMouse)
        {
            MouseLook();
        }
    }
    public void MouseLook()
    {
        Vector3 playerToMouse = MouseToWorldPoint(Mouse.current.position.ReadValue()) - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(playerToMouse);
        if (lookRotation.eulerAngles != Vector3.zero)
        {
            lookRotation.x = 0f;
            lookRotation.z = 0f;
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, RotationSpeed * Time.deltaTime);
        }

    }
    private Vector3 MouseToWorldPoint(Vector2 mouseScreen)
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseScreen);
        ray.origin += offset;
        if (Physics.Raycast(ray, out RaycastHit rayHit, 100.0f, layerMask))
        {
            return rayHit.point;
        }
        return transform.position;
    }
}
