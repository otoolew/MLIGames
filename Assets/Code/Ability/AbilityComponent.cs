using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityComponent : MonoBehaviour
{
    public abstract string OwnerTag { get; set; }
    public abstract void PullTrigger();
    public abstract void ReleaseTrigger();
    public abstract void Fire();
}
