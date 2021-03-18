using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Projectile", menuName = "Game/Projectile Data")]
public class ProjectileData : ScriptableObject
{
    #region Variables
    [SerializeField] private float modifierValue;
    public float ModifierValue { get => modifierValue; set => modifierValue = value; }

    [SerializeField] private float range;
    public float Range { get => range; set => range = value; }

    [SerializeField] private float maxVelocity;
    public float MaxVelocity { get => maxVelocity; set => maxVelocity = value; }
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
        projectile.ModifierValue = modifierValue;
        projectile.Range = range;
        projectile.MaxVelocity = maxVelocity;
    }
    #endregion
}
