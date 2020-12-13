using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProjectileAbilityPanel : AbilityPanel
{
    #region Components
    [SerializeField] private ProjectileAbility abilityComp;
    public override AbilityComponent AbilityComp { get => abilityComp as ProjectileAbility; set => abilityComp = value as ProjectileAbility; }

    public override Ability Ability { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    [SerializeField] private TMP_Text ammoCountText;
    public TMP_Text AmmoCountText { get => ammoCountText; set => ammoCountText = value; }

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void ChangeAmmoCountText(string value)
    {
        ammoCountText.text = value;
    }

    public override void AssignAbility(AbilityComponent ability)
    {
        if(ability.GetType() == typeof(ProjectileAbility))
        {
            ProjectileAbility abilityComp = (ProjectileAbility)ability;
            abilityComp.MunitionStorage.onAmmoCountChange.AddListener(ChangeAmmoCountText);
            ammoCountText.text = abilityComp.MunitionStorage.GetMunitionsDisplay();
        }       
    }
}
