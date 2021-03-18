using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    public float RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }
    public Quaternion CurrentRotation { get { return transform.rotation; } }

    public virtual void OnLook(InputAction.CallbackContext callbackContext)
    {

    }

    public virtual void RotateTo(Vector2 value)
    {
        Vector3 direction = (Vector3.right * value.x) + (Vector3.forward * value.y);
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        if (targetRotation.eulerAngles != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        }
    }

    public void RotateTo(Transform value)
    {
        Vector3 direction = (Vector3.right * value.position.x) + (Vector3.forward * value.position.y);
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        if(targetRotation.eulerAngles != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        }
    }
    public void RotateTo(Vector3 value)
    {
        Vector3 direction = (Vector3.right * value.x) + (Vector3.forward * value.y);
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        if (targetRotation.eulerAngles != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        }
    }
    public bool IsPerpendicular(Vector3 targetDirection, float bias = 0.01f)
    {
        float dot = Vector3.Dot(transform.rotation.eulerAngles.normalized, targetDirection.normalized);
        Debug.Log("DOT PRODUCT " + dot);
        
        if (dot > 1f - bias)
        {
            return true;
        }
        return false;
    }

    public bool IsParallel(Vector3 targetDirection, float bias = 0.01f)
    {
        if (Vector3.Dot(transform.rotation.eulerAngles.normalized, targetDirection.normalized) > 1f - bias)
        {
            return true;
        }
        return false;
    }

    public void IsRotationParalel(Vector2 value)
    {
        if (value.magnitude > 0.1)
        {
            Vector3 direction = (Vector3.right * value.x) + (Vector3.forward * value.y);
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        }
    }

    static float Wrap180(float angle)
    {
        angle %= 360;
        if (angle < -180)
        {
            angle += 360;
        }
        else if (angle > 180)
        {
            angle -= 360;
        }
        return angle;
    }
}
