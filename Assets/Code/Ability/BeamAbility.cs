using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BeamAbility : AbilityComponent
{
    #region Components

    [SerializeField] private Timer damageTickTimer;
    public Timer DamageTickTimer { get => damageTickTimer; set => damageTickTimer = value; }

    [SerializeField] private LineRenderer lineRenderer;
    public LineRenderer LineRenderer { get => lineRenderer; set => lineRenderer = value; }

    [SerializeField] private Transform firePoint;
    public override Transform FirePoint { get => firePoint; set => firePoint = value; }

    #endregion

    #region Values
    [SerializeField] private float modifierValue;
    public float ModifierValue { get => modifierValue; set => modifierValue = value; }

    [SerializeField] private float range;
    public float Range { get => range; set => range = value; }

    [SerializeField] private float damageTickRate;
    /// <summary>
    /// Every amount of time in seconds apply damage.
    /// </summary>
    public float DamageTickRate { get => damageTickRate; set => damageTickRate = value; }

    [SerializeField] private LayerMask hitLayerMask;
    public LayerMask HitLayerMask { get => hitLayerMask; set => hitLayerMask = value; }
    #endregion

    [SerializeField] private bool isTriggerdown;
    public bool IsTriggerDown { get => isTriggerdown; set => isTriggerdown = value; }

    //[SerializeField] private Ray ray;
    //public Ray Ray { get => ray; set => ray = value; }

    public Vector3 MuzzleLocation 
    {
        get
        {
            return firePoint.transform.localPosition;
        } 
    }
    private void Awake()
    {
        damageTickTimer = new Timer(damageTickRate);
    }
    private void Start()
    {

    }

    private void Update()
    {
        if (IsTriggerDown)
        {
            HitCollider hitObject = CastRay();

            if (hitObject != null)
            {
                damageTickTimer.Tick();
                if (damageTickTimer.IsFinished)
                {
                    ApplyModifierValue(hitObject);
                    damageTickTimer.ResetTimer();
                }
            }
            else
            {
                damageTickTimer.ResetTimer();
            }
        }
    }

    public override void PullTrigger()
    {
        isTriggerdown = true;
        lineRenderer.enabled = true;
    }

    public override void ReleaseTrigger()
    {
        isTriggerdown = false;
        lineRenderer.enabled = false;
    }

    public HitCollider CastRay()
    {
        lineRenderer.SetPosition(0, firePoint.transform.position);

        Ray ray = new Ray
        {
            origin = firePoint.position,
            direction = firePoint.forward,
        };

        if (Physics.Raycast(ray, out RaycastHit raycastHit, range, hitLayerMask))
        {
            lineRenderer.SetPosition(1, raycastHit.point);
            HitCollider hitObject = raycastHit.collider.GetComponent<HitCollider>();
            if(hitObject != null)
            {
                return hitObject;
            }
        }
        else
        {
            Vector3 hitPosition = ray.origin + ray.direction * range;
            lineRenderer.SetPosition(1, hitPosition);
        }
        return null;
    }

    public override void Fire()
    {
        Debug.Log("Nothing to do here...");
    }

    public void ApplyModifierValue(HitCollider hitCollider)
    {
        hitCollider.HealthComp.ApplyHealthChange(ModifierValue);
    }
}
