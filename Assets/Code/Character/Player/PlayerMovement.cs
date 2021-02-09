using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CharacterMovement
{
    #region Components
    [Header("Components")]
    [SerializeField] private PlayerControls inputActions;
    public PlayerControls InputActions { get => inputActions; set => inputActions = value; }

    [SerializeField] private Rigidbody rigidbodyComp;
    public Rigidbody RigidbodyComp { get => rigidbodyComp; set => rigidbodyComp = value; }

    [SerializeField] private CharacterController characterController;
    public CharacterController CharacterController { get => characterController; set => characterController = value; }

    [SerializeField] private Timer dashCooldownTimer;
    public Timer DashCooldownTimer { get => dashCooldownTimer; set => dashCooldownTimer = value; }

    [SerializeField] private Timer dashActiveTimer;
    public Timer DashActiveTimer { get => dashActiveTimer; set => dashActiveTimer = value; }

    #endregion
    #region Values

    [Header("Player")]
    [SerializeField] private Vector2 moveInput;
    public Vector2 MoveInput { get => moveInput; set => moveInput = value; }

    public bool IsDashing;
    public float DashMultiplier;
    public float DashCooldown;
    public float DashActiveTime;
    public Vector3 DashDirection;

    #endregion

    protected override void Start()
    {
        dashCooldownTimer = new Timer(DashCooldown);
        dashActiveTimer = new Timer(DashActiveTime);
    }
    protected override void Update()
    {
        if (InputActions != null)
        {
            moveInput = InputActions.Character.Move.ReadValue<Vector2>();
        }

        Move(new Vector3(moveInput.x, 0.0f, moveInput.y));

        if (IsDashing)
        {
            dashActiveTimer.Tick();
            Dash();
            if (dashActiveTimer.IsFinished)
            {
                IsDashing = false;
                dashCooldownTimer.ResetTimer();
            }
        }
        else
        {
            dashCooldownTimer.Tick();
        }
    }
    public override void Move(Vector3 moveVector)
    {
        //Vector3 moveNormal = new Vector3(moveVector.x, 0.0f, moveVector.y).normalized;
        //transform.position += moveVector * Time.deltaTime * MoveSpeed;

        characterController.Move(moveVector * MoveSpeed * Time.deltaTime);
        DashDirection = moveVector;
        if (OrientToMovement && moveVector.magnitude > 0.1)
        {
            Vector3 direction = (Vector3.right * moveVector.x) + (Vector3.forward * moveVector.z);
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        }
    }

    public void StartDash()
    {
        if (IsDashing)
            return;

        if (dashCooldownTimer.IsFinished)
        {
            IsDashing = true;
            dashActiveTimer.ResetTimer();
            //DashDirection = transform.rotation.eulerAngles;
        }
    }
    public void Dash()
    {
        characterController.Move(DashDirection * (MoveSpeed * DashMultiplier) * Time.deltaTime);
        //transform.position += DashDirection * (MoveSpeed * DashMultiplier) * Time.deltaTime;
    }
}
