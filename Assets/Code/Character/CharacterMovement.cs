using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    #region Values
    [Header("Settings")]
    [SerializeField] private bool orientToMovement;
    public bool OrientToMovement { get => orientToMovement; set => orientToMovement = value; }

    [SerializeField] private float moveSpeed;
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

    [SerializeField] private float rotationSpeed;
    public float RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }

    #endregion
    public virtual void Move()
    {

    }
    public virtual void Move(Vector3 moveVector)
    {

    }
    public virtual void SetDestination(Vector3 moveVector)
    {

    }
}
