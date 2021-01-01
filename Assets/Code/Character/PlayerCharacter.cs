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

    [SerializeField] private CharacterRotation rotationComp;
    public CharacterRotation RotationComp { get => rotationComp; set => rotationComp = value; }

    [SerializeField] private AbilityController leftAbilityController;
    public AbilityController LeftAbilityController { get => leftAbilityController; set => leftAbilityController = value; }

    [SerializeField] private AbilityController rightAbilityController;
    public AbilityController RightAbilityController { get => rightAbilityController; set => rightAbilityController = value; }

    [Header("Ability Configs")]
    [SerializeField] private RaycastAbilityConfig raycastAbilityConfig;
    public RaycastAbilityConfig RaycastAbilityConfig { get => raycastAbilityConfig; set => raycastAbilityConfig = value; }

    [SerializeField] private ProjectileAbilityConfig projectileAbilityConfig;
    public ProjectileAbilityConfig ProjectileAbilityConfig { get => projectileAbilityConfig; set => projectileAbilityConfig = value; }

    public UnityEvent onUseInteractable;

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
        Look();
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

        SetUpPlayerInput(playerController);


        SetUpAbility(LeftAbilityController, raycastAbilityConfig);
        SetUpAbility(RightAbilityController, raycastAbilityConfig);

        SetUpPlayerHUD(playerController);
       
    }

    public void ReleaseCharacter(PlayerController playerController)
    {
        Debug.Log(playerController.name + " Released Character " + gameObject.name);
        playerController.InputActions.Character.Disable();
        PlayerController = null;
    }

    private void SetUpPlayerInput(PlayerController playerController)
    {
        playerController.InputActions.Character.Enable();

        if (playerController.GamepadEnabled)
        {
            playerController.InputActions.Character.Look.Enable();
            playerController.InputActions.Character.Look.started += OnLook;
        }     
        else
        {
            playerController.InputActions.Character.Look.Disable();
        }   
  

        playerController.InputActions.Character.Left_PullTrigger.started += OnLeftPullTrigger;
        playerController.InputActions.Character.Left_PullTrigger.canceled += OnLeftPullTrigger;

        playerController.InputActions.Character.Right_PullTrigger.started += OnRightPullTrigger;
        playerController.InputActions.Character.Right_PullTrigger.canceled += OnRightPullTrigger;

        playerController.InputActions.Character.Left_Reload.started += OnLeftReload;
        playerController.InputActions.Character.Right_Reload.started += OnRightReload;

        playerController.InputActions.Character.UseInteraction.started += OnUseInteraction;


    }
    private void SetUpPlayerHUD(PlayerController playerController)
    {
        playerController.PlayerHUD.gameObject.SetActive(true);

        playerController.PlayerHUD.AssignLeftAbility(leftAbilityController.CurrentAbility);
        playerController.PlayerHUD.AssignRightAbility(rightAbilityController.CurrentAbility);
    }

    private void SetUpAbility(AbilityController abilityController, AbilityConfig abilityConfig)
    {
        abilityController.EquipAbility(abilityConfig);
    }

    public void Move()
    {
        if (playerController)
        {
            Vector2 moveInput = playerController.InputActions.Character.Move.ReadValue<Vector2>();
            movementComp.Move(new Vector3(moveInput.x, 0.0f, moveInput.y));
        }                  
    }
    public void Look()
    {
        if (playerController)
        {
            if (playerController.GamepadEnabled)
            {
                Vector2 lookInput = playerController.InputActions.Character.Look.ReadValue<Vector2>();
                rotationComp.RotateToDirection(lookInput);
                if (movementComp)
                {
                    if (lookInput.magnitude > 0.1f)
                    {
                        movementComp.OrientToMovement = false;
                    }
                    else
                    {
                        movementComp.OrientToMovement = true;
                    }
                }
            }
            else
            {
                rotationComp.MouseLook();
            }     
        }
    }

    private void UseInteraction()
    {
        Debug.Log("Use Button Pressed");
        onUseInteractable.Invoke();
    }

    #endregion

    #region PlayerInput Calls

    ///// <summary>
    ///// Called when [Move].
    ///// </summary>
    //public void OnMove(InputAction.CallbackContext callbackContext)
    //{
    //    Vector2 inputVector = callbackContext.ReadValue<Vector2>();
    //    movementComp.Move(new Vector3(inputVector.x, 0.0f, inputVector.y));
    //}
    /// <summary>
    /// Called when [Left_PullTrigger].
    /// </summary>
    public void OnLook(InputAction.CallbackContext callbackContext)
    {

    }
    /// <summary>
    /// Called when [Left_PullTrigger].
    /// </summary>
    public void OnLeftPullTrigger(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if (leftAbilityController)
                leftAbilityController.PullTrigger();
        }

        if (callbackContext.canceled)
        {
            if (leftAbilityController)
                leftAbilityController.ReleaseTrigger();
        }       
    }
    public void OnLeftReload(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if (leftAbilityController)
                leftAbilityController.Reload();
        }
    }
    /// <summary>
    /// Called when [Right_PullTrigger].
    /// </summary>
    public void OnRightPullTrigger(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if (rightAbilityController)
                rightAbilityController.PullTrigger();
        }

        if (callbackContext.canceled)
        {
            if (rightAbilityController)
                rightAbilityController.ReleaseTrigger();
        }
    }
    public void OnRightReload(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if (rightAbilityController)
                rightAbilityController.Reload();
        }

    }

    private void OnUseInteraction(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            UseInteraction();
        }
    }
    #endregion
}
