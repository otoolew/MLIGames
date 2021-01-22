using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerCharacter : Character
{
    [SerializeField] private PlayerController playerController;
    public PlayerController PlayerController { get => playerController; set => playerController = value; }

    [SerializeField] private PlayerMovement movementComp;
    public override CharacterMovement MovementComp { get => movementComp as PlayerMovement; set => movementComp = (PlayerMovement)value; }

    [SerializeField] private HealthComponent healthComp;
    public override HealthComponent HealthComp { get => healthComp; set => healthComp = value; }
    
    [SerializeField] private CharacterRotation rotationComp;
    public CharacterRotation RotationComp { get => rotationComp; set => rotationComp = value; }

    [SerializeField] private WeaponAbilityController leftAbilityController;
    public WeaponAbilityController LeftAbilityController { get => leftAbilityController; set => leftAbilityController = value; }

    [SerializeField] private WeaponAbilityController rightAbilityController;
    public WeaponAbilityController RightAbilityController { get => rightAbilityController; set => rightAbilityController = value; }

    [SerializeField] private DashAbilityController dashAbilityController;
    public DashAbilityController DashAbilityController { get => dashAbilityController; set => dashAbilityController = value; }


    [Header("Ability Configs")]

    [SerializeField] private MeleeAbilityConfig meleeAbilityConfigConfig;
    public MeleeAbilityConfig MeleeAbilityConfigConfig { get => meleeAbilityConfigConfig; set => meleeAbilityConfigConfig = value; }

    [SerializeField] private RaycastAbilityConfig raycastAbilityConfig;
    public RaycastAbilityConfig RaycastAbilityConfig { get => raycastAbilityConfig; set => raycastAbilityConfig = value; }

    [SerializeField] private ProjectileAbilityConfig projectileAbilityConfig;
    public ProjectileAbilityConfig ProjectileAbilityConfig { get => projectileAbilityConfig; set => projectileAbilityConfig = value; }

    public UnityEvent onUseInteractable;

    //public UnityAction WeaponActionTrigger;

    #region Monobehaviour
    private void Awake()
    {
        //inputActions = new PlayerControls();
        onUseInteractable = new UnityEvent();
        SetUpAbility(LeftAbilityController, meleeAbilityConfigConfig);
        SetUpAbility(RightAbilityController, raycastAbilityConfig);
        SetUpAbility(DashAbilityController);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        onUseInteractable.RemoveAllListeners();
        if(playerController)
            playerController.ReleaseCharacter();
    }
    #endregion

    #region PlayerCharacter
    public void SetUpPlayerUI(PlayerUI playerUI)
    {
        //Debug.Log("Setup Player UI");
        playerUI.CharacterPanel.gameObject.SetActive(true);
        playerUI.CharacterPanel.AssignCharacterUIElements(this);
    }
    private void SetUpAbility(AbilityController abilityController, AbilityConfig abilityConfig)
    {
        abilityController.Owner = this;
        abilityController.EquipAbility(abilityConfig);
    }
    private void SetUpAbility(DashAbilityController abilityController)
    {
        abilityController.Owner = this;
    }
    public void Move(Vector2 moveInput)
    {     
        movementComp.Move(new Vector3(moveInput.x, 0.0f, moveInput.y));                        
    }
    public void Look(Vector2 lookInput)
    {
        if (GameManager.Instance.PlayerOptions.ControllerType == ControllerType.GAMEPAD)
        {          
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
    #endregion

    #region PlayerInput Calls
    public void OnDashPullTrigger(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if (movementComp)
                movementComp.StartDash();
        }

        if (callbackContext.canceled)
        {
        }
    }
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
    public void OnUseInteraction(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            onUseInteractable.Invoke();
        }
    }
    #endregion
}
