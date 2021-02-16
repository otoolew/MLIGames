using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AICharacter : Character
{
    #region Components

    [SerializeField] private Rigidbody rigidbodyComp;
    public override Rigidbody RigidbodyComp { get => rigidbodyComp; set => rigidbodyComp = value; }

    [SerializeField] private Animator animatorComp;
    public override Animator AnimatorComp { get => animatorComp; set => animatorComp = value; }

    [SerializeField] private AIMovement movementComp;
    public override CharacterMovement MovementComp { get => movementComp as AIMovement; set => movementComp = (AIMovement)value; }

    [SerializeField] private CharacterRotation rotationComp;
    public override CharacterRotation RotationComp { get => rotationComp; set => rotationComp = value; }

    [SerializeField] private HealthComponent healthComp;
    public override HealthComponent HealthComp { get => healthComp; set => healthComp = value; }

    #region Ability
    [Header("Ability Controllers")]
    
    [Header("Ability Configs")]
    [SerializeField] private RaycastAbilityConfig raycastAbilityConfig;
    public RaycastAbilityConfig RaycastAbilityConfig { get => raycastAbilityConfig; set => raycastAbilityConfig = value; }

    [SerializeField] private WeaponAbilityController abilityController;
    public WeaponAbilityController AbilityController { get => abilityController; set => abilityController = value; }
    #endregion

    #endregion

    #region Values
    [SerializeField] private Transform focusPoint;
    public override Transform TargetPoint { get => focusPoint; set => focusPoint = value; }

    [SerializeField] private Transform focusTarget;
    public override Transform FocusTarget { get => focusTarget; set => focusTarget = value; }
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        SetUpAbility(abilityController, raycastAbilityConfig);
        healthComp.DeathAction += OnDeath;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetUpAbility(AbilityController abilityController, AbilityConfig abilityConfig)
    {
        abilityController.Owner = this;
        abilityController.EquipAbility(abilityConfig);
    }

    public override bool IsValid()
    {
        if(HealthComp != null)
        {
            if (HealthComp.IsDead)
            {
                return false;
            }           
        }
        return true;
    }

    #region Character
    public override void OnDeath()
    {
        Debug.LogWarning("TODO: Implement Character Death");
    }

    #endregion

    #region Editor
    /// <summary>
    /// On Validate is only called in Editor. By performing checks here was can rest assured they will not be null.
    /// </summary>
    protected override void OnValidate()
    {
        base.OnValidate();
    }
    #endregion



}
