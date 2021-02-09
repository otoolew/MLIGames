using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRotation : CharacterRotation
{
    #region Components
    [SerializeField] private PlayerControls inputActions;
    public PlayerControls InputActions { get => inputActions; set => inputActions = value; }

    [SerializeField] private VisionPerception visionComp;
    public VisionPerception VisionComp { get => visionComp; set => visionComp = value; }
    #endregion

    [SerializeField] private bool gamepadEnabled;
    public bool GamepadEnabled { get => gamepadEnabled; set => gamepadEnabled = value; }

    [SerializeField] private Vector2 lookInput;
    public Vector2 LookInput { get => lookInput; set => lookInput = value; }

    [SerializeField] private bool autoAim;
    public bool AutoAim { get => autoAim; set => autoAim = value; }

    [SerializeField] private bool isAiming;
    public bool IsAiming { get => isAiming; set => isAiming = value; }

    [SerializeField] private Transform rotationFocus;
    public Transform RotationFocus { get => rotationFocus; set => rotationFocus = value; }

    [SerializeField] private Vector3 offset;
    public Vector3 Offset { get => offset; set => offset = value; }

    [SerializeField] private LayerMask layerMask;
    public LayerMask LayerMask { get => layerMask; set => layerMask = value; }

    private void Update()
    {
        if (InputActions != null)
        {
            lookInput = InputActions.Character.Look.ReadValue<Vector2>();
        }

        //if (RotationFocus)
        //{
        //    RotateTo(RotationFocus);
        //}

        //if (isAiming)
        //{
        //    CastRay();
        //}
    }

    #region PlayerInput Calls
    public override void RotateTo(Vector2 value)
    {
        if (gamepadEnabled)
        {
            if (Gamepad.current.enabled)
            {
                Vector3 direction = (Vector3.right * value.x) + (Vector3.forward * value.y);
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                if (targetRotation.eulerAngles != Vector3.zero)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
                }
            }
            return;
        }
        
        if (Mouse.current.enabled)
        {
            Vector3 playerToMouse = MouseToWorldPoint(value) - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(playerToMouse);
            if (lookRotation.eulerAngles != Vector3.zero)
            {
                lookRotation.x = 0f;
                lookRotation.z = 0f;
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, RotationSpeed * Time.deltaTime);
            }
        }
    }

    public override void OnLook(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            IsAiming = true;
        }
        if (callbackContext.performed)
        {
            RotateTo(callbackContext.ReadValue<Vector2>());
        }
        if (callbackContext.canceled)
        {
            IsAiming = false;
        }
    }
    #endregion

    public void CastRay()
    {
        Ray ray = new Ray
        {
            origin = transform.position,
            direction = transform.forward,
        };
        ray.origin += new Vector3(0, 1, 0);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, visionComp.Radius, visionComp.DetectionLayer))
        {
            Character hitObject = raycastHit.collider.GetComponent<Character>();
            if (hitObject != null)
            {
                rotationFocus = hitObject.FocusPoint;
            }
        }
    }

    public void Aim()
    {
        RotationFocus = null;
        for (int i = 0; i < visionComp.VisableTargetList.Count; i++)
        {
            float bestAngle = 90;
            Vector3 dir = visionComp.VisableTargetList[i].transform.position - transform.position;
            float aimAngle = Vector3.Angle(transform.forward, dir);
            if (aimAngle < bestAngle)
            {
                bestAngle = aimAngle;
                Debug.Log("Aim Angle " + aimAngle + " " + visionComp.VisableTargetList[i].name);
                RotationFocus = visionComp.VisableTargetList[i].transform;
            }

            //Vector3 forward = transform.TransformDirection(Vector3.up).normalized;
            //float deviation = Vector3.Dot(forward, dir.normalized);
            //Debug.Log("Input Deviation From Character Direction " + deviation);
            //ChangeDirection(previousDeviation, deviation);
        }
    }
    #region Mouse
    public void MouseLook()
    {
        Vector3 playerToMouse = MouseToWorldPoint(Mouse.current.position.ReadValue()) - transform.position;
        Debug.Log(Mouse.current.position.ReadValue());
        Debug.Log(MouseToWorldPoint(Mouse.current.position.ReadValue()));
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
        if (Physics.Raycast(ray, out RaycastHit rayHit, 1000.0f, layerMask))
        {
            return rayHit.point;
        }
        return transform.position;
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin = transform.position + new Vector3(0, 1, 0);
        Gizmos.DrawRay(origin, Vector3.forward * visionComp.Radius);
    }
}
