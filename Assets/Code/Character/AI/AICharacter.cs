using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private VisionPerception visionPerception;
    public VisionPerception VisionPerception { get => visionPerception; set => visionPerception = value; }
    #endregion

    #region Values

    [SerializeField] private PlayerCharacter playerCharacter;
    public PlayerCharacter PlayerCharacter { get => playerCharacter; set => playerCharacter = value; }

    [SerializeField] private Transform focusPoint;
    public override Transform FocusPoint { get => focusPoint; set => focusPoint = value; }

    [SerializeField] private Transform focusTarget;
    public override Transform FocusTarget { get => focusTarget; set => focusTarget = value; }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        healthComp.DeathAction = OnDeath;
    }

    // Update is called once per frame
    void Update()
    {

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

    public void FindPlayer()
    {
        playerCharacter = FindObjectOfType<PlayerCharacter>();
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
