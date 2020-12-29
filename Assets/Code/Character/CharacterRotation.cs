using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    public float RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }

    [SerializeField] private Vector3 offset;
    public Vector3 Offset { get => offset; set => offset = value; }

    [SerializeField] private LayerMask layerMask;
    public LayerMask LayerMask { get => layerMask; set => layerMask = value; }

    public void RotateToDirection(Vector2 value)
    {
        if(value.magnitude > 0.1)
        {
            Vector3 direction = (Vector3.right * value.x) + (Vector3.forward * value.y);
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
            //Debug.Log("Rot Val = " + value);
            //Vector3 relativePos = new Vector3(value.x, 0.0f, value.y) - transform.position;
            //relativePos.x = 0;
            //relativePos.z = 0;
            //Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            //Quaternion rotation = Quaternion.FromToRotation(transform.rotation.eulerAngles, relativePos);
           // transform.rotation = Quaternion.Lerp(transform.rotation, rotation, RotationSpeed * Time.deltaTime);
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
