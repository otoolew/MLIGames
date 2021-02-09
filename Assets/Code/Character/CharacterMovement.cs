using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class CharacterMovement : MonoBehaviour
{
    #region Components
    //public abstract Rigidbody RigidbodyComp { get; set; }
    //public abstract CharacterController CharacterController { get; set; }
    #endregion

    #region Values
    [Header("Settings")]
    [SerializeField] private bool orientToMovement;
    public bool OrientToMovement { get => orientToMovement; set => orientToMovement = value; }

    [SerializeField] private float moveSpeed;
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

    [SerializeField] private float rotationSpeed;
    public float RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }

    #endregion

    protected virtual void Start()
    {      
    }
    protected virtual void Update()
    {

    }

    public abstract void Move(Vector3 moveVector);

}
