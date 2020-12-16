using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFactory : MonoBehaviour
{

    [SerializeField] private RaycastAbilityConfig raycastAbilityConfig;
    public RaycastAbilityConfig RaycastAbilityConfig { get => raycastAbilityConfig; set => raycastAbilityConfig = value; }

    [SerializeField] private ProjectileAbilityConfig projectileAbilityConfig;
    public ProjectileAbilityConfig ProjectileAbilityConfig { get => projectileAbilityConfig; set => projectileAbilityConfig = value; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EquipRaycastAbility(AbilityController abilityController)
    {
        //ProjectileAbility projectileAbility = GameObject.Instantiate(projectileAbilityPrefab, abilityController.transform);
        //abilityController.EquipAbility(projectileAbility);
    }

    public void EquipProjectileAbility(AbilityController abilityController)
    {
        //ProjectileAbility projectileAbility = GameObject.Instantiate(projectileAbilityPrefab, abilityController.transform);
        //abilityController.EquipAbility(projectileAbility);
    }
}
