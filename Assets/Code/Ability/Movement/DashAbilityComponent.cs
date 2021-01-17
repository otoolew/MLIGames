using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbilityComponent : AbilityComponent
{
    #region Components
    [SerializeField] private DashAbilityController abilityController;
    public DashAbilityController AbilityController { get => abilityController; set => abilityController = value; }

    [SerializeField] private Timer cooldownTimer;
    public Timer CooldownTimer { get => cooldownTimer; set => cooldownTimer = value; }
    #endregion

    #region Variables
    [SerializeField] private bool isTriggerHeld;
    public bool IsTriggerHeld { get => isTriggerHeld; set => isTriggerHeld = value; }

    [SerializeField] private bool isAutoFire;
    public bool IsAutoFire { get => isAutoFire; set => isAutoFire = value; }

    [SerializeField] private float fireRate;
    public float FireRate { get => fireRate; set => fireRate = value; }

    #endregion

    #region Monobehaviour
    private void Awake()
    {

    }
    private void OnEnable()
    {
        cooldownTimer = new Timer(fireRate);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        cooldownTimer.Tick();
        if (isTriggerHeld && isAutoFire)
        {
            Fire();
        }
    }
    #endregion


    public override void Fire()
    {
        if (cooldownTimer.IsFinished)
        {
            cooldownTimer.ResetTimer();
            PlayerCharacter playerCharacter = (PlayerCharacter)abilityController.Owner;
            if (playerCharacter)
            {
                PlayerMovement playerMovement = (PlayerMovement)playerCharacter.MovementComp;
                if(playerMovement)
                    playerMovement.StartDash();
            }
        }
    }

    public override void PullTrigger()
    {
        isTriggerHeld = true;
        Fire();
    }

    public override void ReleaseTrigger()
    {
        isTriggerHeld = false;
    }
}
