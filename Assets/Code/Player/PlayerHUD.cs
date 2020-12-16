using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    // TO DO HAVE SEPARATE WEAPON HUD PREFABS STORED IN GUN INFO
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

    public void AssignLeftAbility(AbilityComponent ability)
    {
        Debug.Log("HUD AssignLeftAbility");
        leftAbilityPanel.AssignAbility(ability);
    }

    public void AssignRightAbility(AbilityComponent ability)
    {
        Debug.Log("HUD AssignRightAbility");
        rightAbilityPanel.AssignAbility(ability);
    }

}
