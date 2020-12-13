using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFactory : MonoBehaviour
{
    [SerializeField] private ProjectileAbility projectileAbilityPrefab;
    public ProjectileAbility ProjectileAbilityPrefab { get => projectileAbilityPrefab; set => projectileAbilityPrefab = value; }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void EquipToAbilityController(AbilityController abilityController)
    {
        ProjectileAbility projectileAbility = GameObject.Instantiate(projectileAbilityPrefab, abilityController.transform);
        abilityController.EquipAbility(projectileAbility);
    }
    public bool CreateProjectileWeapon()
    {

        return true;
    }
}
