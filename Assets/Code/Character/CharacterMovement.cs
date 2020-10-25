using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable] public enum MovementState { STANDING, CROUCHING }
public class CharacterMovement : MonoBehaviour
{
    private const string StandAnimKey = "IsStanding";

    #region Components
    [Header("Components")]

    [SerializeField] private CharacterController characterController;
    protected virtual CharacterController CharacterController { get => characterController; set => characterController = value; }

    [SerializeField] private Animator animatorComp;
    protected virtual Animator AnimatorComp { get => animatorComp; set => animatorComp = value; }

    #endregion



    #region Values
    [Header("Variables")]
    public bool OrientToMovement;

    [Header("Current State Values")]
    public MovementState CurrentMoveState;
    public float CurrentSpeed;
    public float BaseMoveSpeed;
    public float RotationSpeed;
    public float DistanceToGround; // distance to ground
    [Header("Standing Values")]
    public float StandSpeedModifier;
    public float StandHeight;
    public Vector3 StandCenter;
    public bool IsStanding;
    [Header("Crouching Values")]
    public float CrouchSpeedModifier;
    public float CrouchHeight;
    public Vector3 CrouchCenter;
    public bool IsCrouching;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        CurrentMoveState = MovementState.STANDING;
        CurrentSpeed = BaseMoveSpeed * StandSpeedModifier;
        characterController.height = StandHeight;
        characterController.center = StandCenter;
        DistanceToGround = 1.0f;
    }
    public void MoveCharacter(Vector3 moveVector)
    {
        characterController.Move(moveVector * CurrentSpeed * Time.deltaTime);

        if (moveVector != Vector3.zero && OrientToMovement)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveVector), RotationSpeed * Time.deltaTime);
        }
    }
    public void Crouch()
    {
        if (CurrentMoveState != MovementState.CROUCHING)
        {
            CurrentSpeed = BaseMoveSpeed * CrouchSpeedModifier;
            characterController.height = CrouchHeight;
            characterController.center = CrouchCenter;
            //Vector3 tmpPosition = transform.position;
            //tmpPosition.y -= DistanceToGround;
            //transform.position -= tmpPosition;
            CurrentMoveState = MovementState.CROUCHING;
            IsCrouching = true;
            IsStanding = false;
            if(AnimatorComp)
                AnimatorComp.SetBool("Crouch", true);
        }
    }
    public void UnCrouch()
    {
        if (CurrentMoveState == MovementState.CROUCHING)
        {
            CurrentSpeed = BaseMoveSpeed * StandSpeedModifier;
            characterController.center = StandCenter;
            characterController.height = StandHeight;
            CurrentMoveState = MovementState.STANDING;
            IsStanding = true;
            IsCrouching = false;
            if (AnimatorComp)
                AnimatorComp.SetBool("Crouch", false);
        }
    }
    public Vector3 ForwardRestPoint()
    {
        return characterController.center * 1.25f + (Vector3.forward * 10);
    }
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(characterController.transform.position + new Vector3(0, characterController.height / 2, 0), new Vector3(characterController.radius * 2, characterController.height, characterController.radius * 2));

        Gizmos.DrawLine(characterController.center * 1.25f, characterController.center * 1.25f + (Vector3.forward * 10));
    }
}
