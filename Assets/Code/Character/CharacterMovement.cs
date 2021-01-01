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
    public bool OrientToMovement;
    [Header("Current State Values")]
    public float CurrentSpeed;
    public float BaseMoveSpeed;
    public float RotationSpeed;
    #endregion


    public void Move(Vector3 moveVector)
    {
        characterController.Move(moveVector * CurrentSpeed * Time.deltaTime);

        if (OrientToMovement && moveVector.magnitude > 0.1)
        {
            Vector3 direction = (Vector3.right * moveVector.x) + (Vector3.forward * moveVector.z);
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        }
    }

}
