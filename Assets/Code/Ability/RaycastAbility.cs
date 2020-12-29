using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastAbility : AbilityComponent
{
    #region Components
    [SerializeField] private LineRenderer lineRenderer;
    public LineRenderer LineRenderer { get => lineRenderer; set => lineRenderer = value; }

    [SerializeField] private Transform firePoint;
    public override Transform FirePoint { get => firePoint; set => firePoint = value; }

    [SerializeField] private Timer cooldownTimer;
    public Timer CooldownTimer { get => cooldownTimer; set => cooldownTimer = value; }

    [Header("Munitions")]
    [SerializeField] private MunitionStorage munitionStorage;
    public MunitionStorage MunitionStorage { get => munitionStorage; set => munitionStorage = value; }
    #endregion


    #region Variables
    [SerializeField] private float modifierValue;
    public float ModifierValue { get => modifierValue; set => modifierValue = value; }

    [SerializeField] private float range;
    public float Range { get => range; set => range = value; }

    [SerializeField] private float fireRate;
    public float FireRate { get => fireRate; set => fireRate = value; }

    [SerializeField] private bool isAutoFire;
    public bool IsAutoFire { get => isAutoFire; set => isAutoFire = value; }

    [SerializeField] private bool isTriggerHeld;
    public bool IsTriggerHeld { get => isTriggerHeld; set => isTriggerHeld = value; }

    [SerializeField] private LayerMask hitLayerMask;
    public LayerMask HitLayerMask { get => hitLayerMask; set => hitLayerMask = value; }

    [SerializeField] private float lineEffectDuration;
    public float LineEffectDuration { get => lineEffectDuration; set => lineEffectDuration = value; }
    #endregion

    private void Awake()
    {
        cooldownTimer = new Timer(fireRate);
    }

    private void Start()
    {

    }

    private void Update()
    {
        cooldownTimer.Tick();
        if (isTriggerHeld && isAutoFire)
        {
            Fire();
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

    public override void Fire()
    {
        if (cooldownTimer.IsFinished)
        {
            if (!MunitionStorage.IsMagazineEmpty)
            {
                MunitionStorage.ConsumeAmmo();
                CastRay();
                PlayMuzzleEffect();
            }

            cooldownTimer.ResetTimer();
        }
    }

    public void ApplyModifierValue(HitCollider hitCollider)
    {
        hitCollider.HealthComp.ApplyHealthChange(ModifierValue);
    }

    public override void Reload()
    {
        munitionStorage.FillMagazine();
    }

    public void CastRay()
    {
        Vector3 hitPosition;
        Ray ray = new Ray
        {
            origin = firePoint.position,
            direction = firePoint.forward,
        };

        if (Physics.Raycast(ray, out RaycastHit raycastHit, range, hitLayerMask))
        {
            hitPosition = raycastHit.point;
            HitCollider hitObject = raycastHit.collider.GetComponent<HitCollider>();
            if (hitObject != null)
            {
                hitObject.HealthComp.ApplyHealthChange(modifierValue);
            }

            PlayImpactEffect(raycastHit.point);
            StartCoroutine(PlayLineEffect(firePoint.position, hitPosition));
        }
        else
        {
            hitPosition = ray.origin + ray.direction * range;
        }

        StartCoroutine(PlayLineEffect(firePoint.position, hitPosition));
    }

    private void PlayMuzzleEffect()
    {
        //Debug.Log("RayCast Ability PlayMuzzleEffect at " + FirePoint);
    }

    private void PlayImpactEffect(Vector3 position)
    {
        //Debug.Log("RayCast Ability Impact at " + position);
    }

    IEnumerator PlayLineEffect(Vector3 startPosition, Vector3 endPosition)
    {
        lineRenderer.enabled = true;

        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);

        
        yield return new WaitForSeconds(LineEffectDuration);
        lineRenderer.enabled = false;
    }
}
