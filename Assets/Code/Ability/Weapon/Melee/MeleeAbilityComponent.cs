using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAbilityComponent : WeaponAbilityComponent
{
    #region Components

    [SerializeField] private Transform firePoint;
    public override Transform FirePoint { get => firePoint; set => firePoint = value; }
    #endregion

    #region Variables
    [SerializeField] private bool isTriggerHeld;
    public bool IsTriggerHeld { get => isTriggerHeld; set => isTriggerHeld = value; }

    [SerializeField] private bool isAutoFire;
    public bool IsAutoFire { get => isAutoFire; set => isAutoFire = value; }

    [SerializeField] private float fireRate;
    public float FireRate { get => fireRate; set => fireRate = value; }

    public bool IsSwinging;
    private float swingStartTime;
    public float SwingTime = 1.0f;

    #endregion

    #region Monobehaviour
    private void Awake()
    {

    }
    private void OnEnable()
    {

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (IsSwinging)
        {
            float fracComplete = (Time.time - swingStartTime) / SwingTime;

            firePoint.localRotation = Quaternion.Slerp(Quaternion.Euler(new Vector3(0, -45, 0)), Quaternion.Euler(new Vector3(0, 45, 0)), fracComplete);
            if (fracComplete >= 1)
            {
                StopSwing();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Melee Trigger " + other.name);
        HitCollider hitCollider = other.GetComponent<HitCollider>();
        if (hitCollider)
        {
            //Debug.Log(string.Format("{0} Hit {1}", this.name, hitCollider.transform.root.name));
            hitCollider.HealthComp.ApplyHealthChange(-10);
        }
    }
    #endregion
    private void StartSwing()
    {
        swingStartTime = Time.time;
        firePoint.localRotation = Quaternion.Euler(new Vector3(0, -45, 0));
        IsSwinging = true;
    }
    private void StopSwing()
    {
        IsSwinging = false;
    }
    public override void Fire()
    {
        if(!IsSwinging)
            StartSwing();
    }

    public override void PullTrigger()
    {
        Fire();
        isTriggerHeld = true;
    }

    public override void ReleaseTrigger()
    {
        isTriggerHeld = false;
    }

    public override void Reload()
    {
        Debug.Log("Nothing to do here!");
    }
}
