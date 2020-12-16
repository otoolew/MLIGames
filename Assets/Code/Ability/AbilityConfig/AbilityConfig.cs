using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityConfig : ScriptableObject
{
    public abstract AbilityComponent AbilityPrefab { get; set; }
    public abstract AbilityComponent CreateAbilityComponent(Transform parent);
}
