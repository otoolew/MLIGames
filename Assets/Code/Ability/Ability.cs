using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public abstract AbilityComponent AbilityComp { get;}
    public abstract void Tick();
    public abstract void EquipAbility(AbilityController controller);
    //public abstract Transform FirePoint { get; set; }
    public abstract void PullTrigger();
    public abstract void ReleaseTrigger();
    public abstract void Fire();
    public abstract void Reload();

}
