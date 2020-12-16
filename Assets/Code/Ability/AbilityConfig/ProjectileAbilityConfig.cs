using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newProjectileAbilityConfig", menuName = "Game/Ability Config/Projectile Ability Config")]
public class ProjectileAbilityConfig : AbilityConfig
{
    [SerializeField] private ProjectileAbility projectileAbilityPrefab;
    public override AbilityComponent AbilityPrefab { get => projectileAbilityPrefab as ProjectileAbility; set => projectileAbilityPrefab = value as ProjectileAbility; }

    public override AbilityComponent CreateAbilityComponent(Transform parent)
    {
        throw new System.NotImplementedException();
    }
}
