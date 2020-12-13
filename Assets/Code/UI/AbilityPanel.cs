using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbilityPanel : MonoBehaviour
{
    #region Components
    public abstract Ability Ability { get; set; }
    public abstract AbilityComponent AbilityComp { get; set; }
    public abstract void AssignAbility(AbilityComponent ability);
    #endregion
}
