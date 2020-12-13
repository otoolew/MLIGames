using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    // TO DO HAVE SEPARATE WEAPON HUD PREFABS STORED IN GUN INFO
    //[SerializeField] private CannonPanel leftCannonPanel;
    //public CannonPanel LeftCannonPanel { get => leftCannonPanel; set => leftCannonPanel = value; }

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
    public void AssignLeftAbilityUI(AbilityComponent ability)
    {
        
    }
    public void AssignRightAbility(AbilityComponent ability)
    {
        rightAbilityPanel.AssignAbility(ability);
    }

}
