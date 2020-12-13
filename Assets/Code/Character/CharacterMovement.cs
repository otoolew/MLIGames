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
    #endregion


    public void Move(Vector3 moveVector)
    {
        characterController.Move(moveVector * CurrentSpeed * Time.deltaTime);
    }
}
