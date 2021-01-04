using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AbilityController : MonoBehaviour
{
    public abstract Character Owner { get; set; }
    public abstract AbilityComponent CurrentAbility { get; set; }
    public UnityEvent<AbilityComponent> onAbilityEquipped { get; set; }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        onAbilityEquipped = new UnityEvent<AbilityComponent>();
    }

    public virtual void EquipAbility(AbilityConfig abilityConfig)
    {
        CurrentAbility = abilityConfig.CreateAbilityComponent(transform);
        onAbilityEquipped.Invoke(CurrentAbility);
    }

    public virtual void PullTrigger()
    {
        if (CurrentAbility)
            CurrentAbility.PullTrigger();
    }

    public virtual void ReleaseTrigger()
    {
        if (CurrentAbility)
            CurrentAbility.ReleaseTrigger();
    }
}
