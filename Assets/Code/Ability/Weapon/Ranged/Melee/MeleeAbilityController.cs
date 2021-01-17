using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MeleeAbilityController : AbilityController
{
    [SerializeField] private Character owner;
    public override Character Owner { get => owner; set => owner = value; }

    [SerializeField] private MeleeAbilityComponent currentAbility;
    public override AbilityComponent CurrentAbility { get => currentAbility as MeleeAbilityComponent; set => currentAbility = (MeleeAbilityComponent)value; }

    public new UnityEvent<MeleeAbilityComponent> onAbilityEquipped;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        onAbilityEquipped = new UnityEvent<MeleeAbilityComponent>();
    }

    public override void EquipAbility(AbilityConfig abilityConfig)
    {
        currentAbility = (MeleeAbilityComponent)abilityConfig.CreateAbilityComponent(transform);
        onAbilityEquipped.Invoke(currentAbility);
    }

    public override void PullTrigger()
    {
        if (currentAbility)
            currentAbility.PullTrigger();
    }
    public override void ReleaseTrigger()
    {
        if (currentAbility)
            currentAbility.ReleaseTrigger();
    }
}
