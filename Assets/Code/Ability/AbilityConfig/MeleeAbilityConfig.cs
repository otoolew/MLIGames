using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newMeleeAbilityConfig", menuName = "Game/Ability Config/Melee Ability Config")]
public class MeleeAbilityConfig : AbilityConfig
{
    [SerializeField] private MeleeAbilityComponent meleeAbilityPrefab;
    public override AbilityComponent AbilityPrefab { get => meleeAbilityPrefab; set => meleeAbilityPrefab = (MeleeAbilityComponent)value; }

    public override AbilityComponent CreateAbilityComponent(Transform parent)
    {
        MeleeAbilityComponent meleeAbility = Instantiate(meleeAbilityPrefab, parent);

        return meleeAbility as MeleeAbilityComponent;
    }
}
