using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPanel : MonoBehaviour
{
    // TO DO HAVE SEPARATE WEAPON HUD PREFABS STORED IN GUN INFO
    [SerializeField] private HealthPanel healthPanel;
    public HealthPanel HealthPanel { get => healthPanel; set => healthPanel = value; }

    [SerializeField] private AbilityPanel leftAbilityPanel;
    public AbilityPanel LeftAbilityPanel { get => leftAbilityPanel; set => leftAbilityPanel = value; }

    [SerializeField] private AbilityPanel rightAbilityPanel;
    public AbilityPanel RightAbilityPanel { get => rightAbilityPanel; set => rightAbilityPanel = value; }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void AssignCharacterUIElements(PlayerCharacter playerCharacter)
    {
        AssignHealthComponent(playerCharacter.HealthComp);
        AssignLeftAbility(playerCharacter.LeftAbilityController.CurrentAbility);
        AssignRightAbility(playerCharacter.RightAbilityController.CurrentAbility);
    }
    public void AssignHealthComponent(HealthComponent healthComp)
    {
        if (healthComp)
        {
            healthPanel.RegisterHealthComponent(healthComp);
        }
    }
    public void AssignLeftAbility(AbilityComponent ability)
    {
        if (ability)
        {
            leftAbilityPanel.AssignAbility(ability);
        }
    }

    public void AssignRightAbility(AbilityComponent ability)
    {
        if (ability)
        {
            rightAbilityPanel.AssignAbility(ability);
        }
    }
}
