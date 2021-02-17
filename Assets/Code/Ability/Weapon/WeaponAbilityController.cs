using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponAbilityController : AbilityController
{
    [SerializeField] private Character owner;
    public override Character Owner { get => owner; set => owner = value; }

    [SerializeField] private WeaponAbilityComponent currentAbility;
    public override AbilityComponent CurrentAbility { get => currentAbility as WeaponAbilityComponent; set => currentAbility = (WeaponAbilityComponent)value; }

    public new UnityEvent<WeaponAbilityComponent> onAbilityEquipped;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        onAbilityEquipped = new UnityEvent<WeaponAbilityComponent>();
    }

    public override void EquipAbility(AbilityConfig abilityConfig, string ownerTag)
    {
        currentAbility = (WeaponAbilityComponent)abilityConfig.CreateAbilityComponent(transform);
        currentAbility.OwnerTag = ownerTag;
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
    public void Reload()
    {
        if (currentAbility)
            currentAbility.Reload();
    }
}
