using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerCharacter : Character
{
    #region Character

    #region RigidbodyComp
    [SerializeField] private Rigidbody rigidbodyComp;
    public override Rigidbody RigidbodyComp { get => rigidbodyComp; set => rigidbodyComp = value; }
    #endregion

    [SerializeField] private Animator animatorComp;
    public override Animator AnimatorComp { get => animatorComp; set => animatorComp = value; }

    [SerializeField] private PlayerController playerController;
    public PlayerController PlayerController { get => playerController; set => playerController = value; }

    [SerializeField] private PlayerMovement movementComp;
    public override CharacterMovement MovementComp { get => movementComp as PlayerMovement; set => movementComp = (PlayerMovement)value; }

    [SerializeField] private PlayerRotation rotationComp;
    public override CharacterRotation RotationComp { get => rotationComp as PlayerRotation; set => rotationComp = (PlayerRotation)value; }

    [SerializeField] private HealthComponent healthComp;
    public override HealthComponent HealthComp { get => healthComp; set => healthComp = value; }

    [SerializeField] private Transform focusPoint;
    public override Transform TargetPoint { get => focusPoint; set => focusPoint = value; }
    #endregion

    #region Ability
    [Header("Ability Controllers")]
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
    #endregion

    #region Values
    [SerializeField] private Transform focusTarget;
    public override Transform FocusTarget { get => focusTarget; set => focusTarget = value; }

    #endregion

    public UnityEvent onUseInteractable;

    #region Monobehaviour
    private void Awake()
    {
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


    #endregion

    #region Character
    public override void OnDeath()
    {
        throw new NotImplementedException();
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
            {
                leftAbilityController.PullTrigger();
            }

        }
        if (callbackContext.performed)
        {
            if (leftAbilityController)
            {

            }
        }

        if (callbackContext.canceled)
        {
            if (leftAbilityController)
            {
                leftAbilityController.ReleaseTrigger();
            }
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
            {
                rightAbilityController.PullTrigger();
            }
        }
        if (callbackContext.performed)
        {
            if (rightAbilityController)
            {

            }
        }
        if (callbackContext.canceled)
        {
            if (rightAbilityController)
            {
                rightAbilityController.ReleaseTrigger();
            }
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

    public override bool IsValid()
    {
        if (healthComp.IsDead)
        {
            return false;
        }
        return true;
    }



    #endregion

    #region Editor
    protected override void OnValidate()
    {
        base.OnValidate();
        
    }
    #endregion
}
