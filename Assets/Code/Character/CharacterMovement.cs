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

    #endregion

    #region Values
    [Header("Variables")]
    public bool RotateToMouse;
    public bool OrientToMovement;
    public float AimFocusOffset;
    [Header("Current State Values")]
    public float CurrentSpeed;
    public float BaseMoveSpeed;
    public float RotationSpeed;
    public float DistanceToGround; // distance to ground
    public LayerMask GroundLayer;
    #endregion

    private void Update()
    {
        if (RotateToMouse)
        {
            MouseLook();
        }
    }

    public void Move(Vector3 moveVector)
    {
        characterController.Move(moveVector * CurrentSpeed * Time.deltaTime);
    }

    public void MouseLook()
    {
        Vector3 playerToMouse = MouseToWorldPoint(Mouse.current.position.ReadValue()) - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(playerToMouse);
        if(lookRotation.eulerAngles != Vector3.zero)
        {
            lookRotation.x = 0f;
            lookRotation.z = 0f;
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, RotationSpeed * Time.deltaTime);
        }

    }

    private Vector3 MouseToWorldPoint(Vector2 mouseScreen)
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseScreen);
        ray.origin += new Vector3(0, AimFocusOffset, 0);
        if (Physics.Raycast(ray, out RaycastHit rayHit, 100.0f, GroundLayer))
        {
            return rayHit.point;
        }
        return transform.position;
    }

}
