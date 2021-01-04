using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Projectile", menuName = "Game/Projectile Data")]
public class ProjectileData : ScriptableObject
{
    #region Variables
    [SerializeField] private float fireRate;
    public float FireRate { get => fireRate; set => fireRate = value; }

    [SerializeField] private float range;
    public float Range { get => range; set => range = value; }

    [SerializeField] private float moveSpeed;
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    #endregion

    #region Monobehaviour
    private void Awake()
    {
        
    }
    private void OnEnable()
    {
        
    }
    #endregion

    #region Methods
    public void InitProjectileData(Projectile projectile)
    {
        projectile.MoveSpeed = moveSpeed;
        projectile.Range = range;
    }
    #endregion
}
