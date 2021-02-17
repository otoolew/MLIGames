using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newProjectileAbilityConfig", menuName = "Game/Ability Config/Projectile Ability Config")]
public class ProjectileAbilityConfig : AbilityConfig
{
    [SerializeField] private ProjectileAbility projectileAbilityPrefab;
    public override AbilityComponent AbilityPrefab { get => projectileAbilityPrefab as ProjectileAbility; set => projectileAbilityPrefab = value as ProjectileAbility; }

    [SerializeField] private float modifierValue;
    public float ModifierValue { get => modifierValue; set => modifierValue = value; }

    [SerializeField] private float fireRate;
    public float FireRate { get => fireRate; set => fireRate = value; }

    [SerializeField] private float range;
    public float Range { get => range; set => range = value; }

    public override AbilityComponent CreateAbilityComponent(Transform parent)
    {
        ProjectileAbility projectileAbility = Instantiate(projectileAbilityPrefab, parent);
        projectileAbility.ModifierValue = modifierValue;
        projectileAbility.FireRate = fireRate;
        projectileAbility.Range = range;
        return projectileAbility as ProjectileAbility;
    }
}
