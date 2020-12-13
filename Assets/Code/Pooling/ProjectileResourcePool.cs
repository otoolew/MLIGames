using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Game/Resource Pool",fileName = "newProjectileResourcePool")]
public class ProjectileResourcePool : ResourcePool<Projectile>
{
    [SerializeField] private Projectile projectilePrefab;
    protected override Projectile PoolablePrefab { get => projectilePrefab; set => projectilePrefab = value; }

    [SerializeField] private Stack<Projectile> projectilePoolStack;
    protected override Stack<Projectile> PoolStack {get => projectilePoolStack;}

    private void OnEnable()
    {
        Debug.Log("ProjectileResourcePool Enabled");
        projectilePoolStack = new Stack<Projectile>();
    }

    public override Projectile CreatePooledObject()
    {
        Projectile projectile = Instantiate(projectilePrefab);
        return projectile;
    }

}
