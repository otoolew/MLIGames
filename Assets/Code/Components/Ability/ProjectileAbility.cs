using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAbility : AbilityComponent
{
    #region Components
    [SerializeField] private Timer cooldownTimer;
    public Timer CooldownTimer { get => cooldownTimer; set => cooldownTimer = value; }

    [SerializeField] private Transform firePoint;
    public override Transform FirePoint { get => firePoint; set => firePoint = value; }
    #endregion

    #region Variables
    [SerializeField] private ProjectileData projectileData;
    public ProjectileData ProjectileData { get => projectileData; set => projectileData = value; }
    #endregion

    #region Monobehaviour
    // Start is called before the first frame update
    void Start()
    {
        cooldownTimer = new Timer(0.25f);
    }

    // Update is called once per frame
    void Update()
    {
        cooldownTimer.Tick();
    }
    #endregion

    public override void Fire()
    {
        //Debug.Log("ProjectileAbility -> Fire");
        Projectile projectile = GameAssetManager.Instance.ProjectilePool.FetchFromPool();
        projectile.AbilityOriginComp = this;
        projectileData.InitProjectileData(projectile);

        //Debug.Log("Set Up Projectile Configuration");
        projectile.gameObject.SetActive(true);
        projectile.transform.position = FirePoint.position;
        projectile.transform.rotation = FirePoint.rotation;
        projectile.FireOriginPoint = FirePoint.position;
    }

    public override void PullTrigger()
    {
        //Debug.Log("ProjectileAbility -> PullTrigger");
        if (cooldownTimer.IsFinished)
        {
            Fire();
            cooldownTimer.ResetTimer();
        }
        else
        {
           //Debug.Log("ProjectileAbility -> PullTrigger -> Cooldown not ready!");
        }

    }

    public override void ReleaseTrigger()
    {
        //Debug.Log("ProjectileAbility -> ReleaseTrigger");
    }

}
