using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    #region Components
    [Header("Components")]
    [SerializeField] private CharacterController characterController;
    public virtual CharacterController CharacterController { get => characterController; set => characterController = value; }

    [SerializeField] private Timer dashCooldownTimer;
    public Timer DashCooldownTimer { get => dashCooldownTimer; set => dashCooldownTimer = value; }

    [SerializeField] private Timer dashActiveTimer;
    public Timer DashActiveTimer { get => dashActiveTimer; set => dashActiveTimer = value; }

    #endregion
    #region Values
    [Header("Variables")]
    public bool OrientToMovement;
    [Header("Current State Values")]
    public Vector2 MoveInput;
    public float CurrentSpeed;
    public float BaseMoveSpeed;
    public float RotationSpeed;
    public bool IsDashing;
    public float DashMultiplier;
    public float DashCooldown;
    public float DashActiveTime;
    public Vector3 DashDirection;

    #endregion

    private void Start()
    {
        dashCooldownTimer = new Timer(DashCooldown);
        dashActiveTimer = new Timer(DashActiveTime);
    }
    private void Update()
    {

        if (IsDashing)
        {
            dashActiveTimer.Tick();
            characterController.Move(DashDirection * (CurrentSpeed * DashMultiplier) * Time.deltaTime);
            if (dashActiveTimer.IsFinished)
            {
                IsDashing = false;
                dashCooldownTimer.ResetTimer();
            }
        }
        else
        {
            dashCooldownTimer.Tick();
            Move(MoveInput);
        }

        //if (isTriggerHeld && isAutoFire)
        //{
        //    Fire();
        //}
    }

    public void Move(Vector2 moveVector)
    {
        Move(new Vector3(moveVector.x, 0.0f, moveVector.y));
    }

    public void Move(Vector3 moveVector)
    {
        characterController.Move(moveVector * CurrentSpeed * Time.deltaTime);
        DashDirection = moveVector;
        if (OrientToMovement && moveVector.magnitude > 0.1)
        {
            Vector3 direction = (Vector3.right * moveVector.x) + (Vector3.forward * moveVector.z);
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        }
    }

    public void Dash()
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
}
