using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newRaycastAbilityConfig", menuName = "Game/Ability Config/Raycast Ability Config")]
public class RaycastAbilityConfig : AbilityConfig
{
    [SerializeField] private RaycastAbility raycastAbilityPrefab;
    public override AbilityComponent AbilityPrefab { get => raycastAbilityPrefab as RaycastAbility; set => raycastAbilityPrefab = value as RaycastAbility; }
    
    [SerializeField] private float modifierValue;
    public float ModifierValue { get => modifierValue; set => modifierValue = value; }
    
    [SerializeField] private float fireRate;
    public float FireRate { get => fireRate; set => fireRate = value; }

    [SerializeField] private float range;
    public float Range { get => range; set => range = value; }

    public override AbilityComponent CreateAbilityComponent(Transform parent)
    {
        RaycastAbility raycastAbility = Instantiate(raycastAbilityPrefab, parent);
        raycastAbility.OwnerTag = parent.tag;
        raycastAbility.ModifierValue = modifierValue;
        raycastAbility.FireRate = fireRate;
        raycastAbility.Range = range;
        return raycastAbility as RaycastAbility;
    }
}
