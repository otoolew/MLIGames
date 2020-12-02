using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : PoolComponent<Projectile>
{
    [SerializeField] Projectile projectilePrefab;
    public override Projectile PoolablePrefab { get => projectilePrefab; set => projectilePrefab = value; }

    public override Projectile CreatePooledObject()
    {
        Projectile projectile = Instantiate(projectilePrefab);
        return projectile;
    }

}
