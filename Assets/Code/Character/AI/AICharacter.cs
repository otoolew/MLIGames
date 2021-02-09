using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacter : Character
{
    #region Components
    [SerializeField] private Rigidbody rigidbodyComp;
    public override Rigidbody RigidbodyComp { get => rigidbodyComp; set => rigidbodyComp = value; }

    [SerializeField] private AIMovement movementComp;
    public override CharacterMovement MovementComp { get => movementComp as AIMovement; set => movementComp = (AIMovement)value; }

    [SerializeField] private CharacterRotation characterRotation;
    public override CharacterRotation CharacterRotation { get => characterRotation; set => characterRotation = value; }

    [SerializeField] private HealthComponent healthComp;
    public override HealthComponent HealthComp { get => healthComp; set => healthComp = value; }

    [SerializeField] private Transform focusPoint;
    public override Transform FocusPoint { get => focusPoint; set => focusPoint = value; }
    #endregion

    #region Values
    [SerializeField] private PlayerCharacter playerCharacter;
    public PlayerCharacter PlayerCharacter { get => playerCharacter; set => playerCharacter = value; }

    [SerializeField] private Transform focusTarget;
    public override Transform FocusTarget { get => focusTarget; set => focusTarget = value; }

    #endregion
    // Start is called before the first frame update
    void Start()
    {

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
}
