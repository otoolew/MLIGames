using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityPanel : MonoBehaviour
{
    #region Components
    [SerializeField] private AbilityComponent abilityComp;
    public AbilityComponent AbilityComp { get => abilityComp; set => abilityComp = value; }

    [SerializeField] private TMP_Text ammoCountText;
    public TMP_Text AmmoCountText { get => ammoCountText; set => ammoCountText = value; }
    #endregion

    private void ChangeAmmoCountText(string value)
    {
        ammoCountText.text = value;
    }

    public void AssignAbility(AbilityComponent ability)
    {
        if (ability.GetType() == typeof(RaycastAbility))
        {
            RaycastAbility abilityComp = (RaycastAbility)ability;
            abilityComp.MunitionStorage.onAmmoCountChange.AddListener(ChangeAmmoCountText);
            ammoCountText.text = abilityComp.MunitionStorage.GetMunitionsDisplay();
            return;
        }

        if (ability.GetType() == typeof(ProjectileAbility))
        {
            ProjectileAbility abilityComp = (ProjectileAbility)ability;
            abilityComp.MunitionStorage.onAmmoCountChange.AddListener(ChangeAmmoCountText);
            ammoCountText.text = abilityComp.MunitionStorage.GetMunitionsDisplay();
            return;
        }
    }
}
