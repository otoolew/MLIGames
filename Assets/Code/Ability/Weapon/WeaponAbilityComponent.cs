using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAbilityComponent : AbilityComponent
{
    public abstract Transform FirePoint { get; set; }
    public abstract void Reload();
}
