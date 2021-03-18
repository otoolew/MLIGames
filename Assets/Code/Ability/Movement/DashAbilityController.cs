using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbilityController : AbilityController
{
    [SerializeField] private PlayerCharacter owner;
    public override Character Owner { get => owner as PlayerCharacter; set => owner = (PlayerCharacter)value; }

    [SerializeField] private DashAbilityComponent currentAbility;
    public override AbilityComponent CurrentAbility { get => currentAbility as DashAbilityComponent; set => currentAbility = (DashAbilityComponent)value; }

}
