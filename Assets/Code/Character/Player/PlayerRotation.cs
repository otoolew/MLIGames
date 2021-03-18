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

    [SerializeField] private Quaternion targetRotation;
    public Quaternion TargetRotation { get => targetRotation; set => targetRotation = value; }

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

    private void Start()
    {
        targetRotation = transform.rotation;
    }
    private void Update()
    {
        if (InputActions != null)
        {
            lookInput = InputActions.Character.Look.ReadValue<Vector2>();
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
    }

    #region PlayerInput Calls
    public override void RotateTo(Vector2 value)
    {
        if (gamepadEnabled)// This is an added check until I figure out how devices activate and deactivate on input
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
        else
        {
            MouseLook();       
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

    #region Mouse
    public void MouseLook()
    {
        if (MouseToWorldPoint(Mouse.current.position.ReadValue(), out Vector3 mousePoint))
        {
            Vector3 playerToMouse = mousePoint - transform.position;

            Quaternion lookRotation = Quaternion.LookRotation(playerToMouse);

            if (lookRotation.eulerAngles != Vector3.zero) // It already shouldn't be...
            {
                lookRotation.x = 0f;
                lookRotation.z = 0f;
                lookRotation.eulerAngles += offset;
                targetRotation = lookRotation;

            }
        }
    }

    private bool MouseToWorldPoint(Vector2 mouseScreen, out Vector3 mousePoint)
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseScreen);
        //ray.origin += offset;
        if (Physics.Raycast(ray, out RaycastHit rayHit, 500.0f, layerMask)) // 1000 can be better
        {
            mousePoint = rayHit.point;
            return true;
        }
        mousePoint = Vector3.zero;
        return false;
    }
    #endregion

}
