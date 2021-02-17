using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newProjectileAbilityConfig", menuName = "Game/Ability Config/Projectile Ability Config")]
public class ProjectileAbilityConfig : AbilityConfig
{
    [SerializeField] private ProjectileAbility projectileAbilityPrefab;
    public override AbilityComponent AbilityPrefab { get => projectileAbilityPrefab as ProjectileAbility; set => projectileAbilityPrefab = value as ProjectileAbility; }

    [SerializeField] private ProjectileData projectileData;
    public ProjectileData ProjectileData { get => projectileData; set => projectileData = value; }

    public override AbilityComponent CreateAbilityComponent(Transform parent)
    {
        ProjectileAbility projectileAbility = Instantiate(projectileAbilityPrefab, parent);
        projectileAbility.ProjectileData = Instantiate(ProjectileData);
        return projectileAbility as ProjectileAbility;
    }
}
