using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastAbility : AbilityComponent
{
    [SerializeField] private float fireRate;
    public float FireRate { get => fireRate; set => fireRate = value; }

    [SerializeField] private float range;
    public float Range { get => range; set => range = value; }

    [SerializeField] private LineRenderer lineRenderer;
    public LineRenderer LineRenderer { get => lineRenderer; set => lineRenderer = value; }

    [SerializeField] private Transform firePoint;
    public override Transform FirePoint { get => firePoint; set => firePoint = value; }

    public override void PullTrigger()
    {

        throw new System.NotImplementedException();
    }

    public override void ReleaseTrigger()
    {
        throw new System.NotImplementedException();
    }
    public override void Fire()
    {
        throw new System.NotImplementedException();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Reload()
    {
        throw new System.NotImplementedException();
    }
}
