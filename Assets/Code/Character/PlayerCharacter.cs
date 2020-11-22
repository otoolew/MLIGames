using RootMotion.FinalIK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    public PlayerController PlayerController { get => playerController; set => playerController = value; }

    [SerializeField] private CharacterMovement movementComp;
    public CharacterMovement MovementComp { get => movementComp; set => movementComp = value; }

    [SerializeField] private AbilityController abilityController;
    public AbilityController AbilityController { get => abilityController; set => abilityController = value; }

    public UnityEvent onUseInteractable;

    [SerializeField] private Transform abilityFirePoint;
    public Transform AbilityFirePoint { get => abilityFirePoint; set => abilityFirePoint = value; }

    //public UnityAction WeaponActionTrigger;

    #region Monobehaviour
    void Start()
    {
        onUseInteractable = new UnityEvent();
        //WeaponActionTrigger = new UnityAction(()=>FireWeapon());
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void UsedButtonPressed()
    {
        Debug.Log("Use Button Pressed");
        onUseInteractable.Invoke();
    }

    private void OnDestroy()
    {
        onUseInteractable.RemoveAllListeners();
    }
    #endregion

    #region PlayerCharacter

    public void PossessCharacter(PlayerController playerController)
    {
        Debug.Log(playerController.name + " Possessed Character " + gameObject.name);
        PlayerController = playerController;
        playerController.InputActions.Character.Enable();
        playerController.InputActions.Character.PullTrigger.started += OnPullTrigger;
        playerController.InputActions.Character.PullTrigger.canceled += OnPullTrigger;
    }

    public void ReleaseCharacter(PlayerController playerController)
    {
        Debug.Log(playerController.name + " Released Character " + gameObject.name);
        playerController.InputActions.Character.Disable();
        PlayerController = null;
    }

    public void Move()
    {
        if (playerController)
        {
            Vector2 moveInput = playerController.InputActions.Character.Move.ReadValue<Vector2>();
            movementComp.Move(new Vector3(moveInput.x, 0.0f, moveInput.y));
        }                  
    }

    #endregion

    #region PlayerInput Calls

    /// <summary>
    /// Called when [Move].
    /// </summary>
    public void OnMove(InputAction.CallbackContext callbackContext)
    {
        Vector2 inputVector = callbackContext.ReadValue<Vector2>();
        movementComp.Move(new Vector3(inputVector.x, 0.0f, inputVector.y));
    }
    /// <summary>
    /// Called when [PullTrigger].
    /// </summary>
    public void OnPullTrigger(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if(abilityController)
                abilityController.PullTrigger();
        }

        if (callbackContext.canceled)
        {
            if(abilityController)
                abilityController.ReleaseTrigger();
        }       
    }

    #endregion
}
