using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileAbility : WeaponAbilityComponent
{
    #region Components
    //[SerializeField] private AbilityController abilityController;
    //public override AbilityController AbilityController { get => abilityController; set => abilityController = value; }

    [SerializeField] private Transform firePoint;
    public override Transform FirePoint { get => firePoint; set => firePoint = value; }
    
    [SerializeField] private Timer cooldownTimer;
    public Timer CooldownTimer { get => cooldownTimer; set => cooldownTimer = value; }

    [Header("Munitions")]
    [SerializeField] private MunitionStorage munitionStorage;
    public MunitionStorage MunitionStorage { get => munitionStorage; set => munitionStorage = value; }

    [SerializeField] private ResourcePool<Projectile> munitionResource;
    public ResourcePool<Projectile> MunitionResource { get => munitionResource; set => munitionResource = value; }

    #endregion

    #region Variables
    [SerializeField] private ProjectileData projectileData;
    public ProjectileData ProjectileData { get => projectileData; set => projectileData = value; }

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
        if(isTriggerHeld && isAutoFire)
        {
            Fire();
        }
    }
    #endregion


    public override void Fire()
    {
        if (cooldownTimer.IsFinished)
        {
            if (munitionStorage.ConsumeAmmo())
            {
                Projectile projectile = munitionResource.FetchFromPool();

                projectile.AbilityOriginComp = this;
                projectileData.InitProjectileData(projectile);

                //Debug.Log("Set Up Projectile Configuration");
                projectile.gameObject.SetActive(true);
                projectile.transform.position = FirePoint.position;
                projectile.transform.rotation = FirePoint.rotation;
                projectile.FireOriginPoint = FirePoint.position;
            }

            cooldownTimer.ResetTimer();
        }
    }

    public override void PullTrigger()
    {
        isTriggerHeld = true;
    }

    public override void ReleaseTrigger()
    {
        isTriggerHeld = false;
    }

    public override void Reload()
    {
        munitionStorage.FillMagazine();
    }
}
