using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityController : MonoBehaviour
{
    [SerializeField] private AbilityComponent currentAbility;
    public AbilityComponent CurrentAbility { get => currentAbility; set => currentAbility = value; }

    public UnityEvent<AbilityComponent> onAbilityEquipped;
    // Start is called before the first frame update
    void Start()
    {
        onAbilityEquipped = new UnityEvent<AbilityComponent>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void EquipAbility(AbilityConfig abilityConfig)
    {
        currentAbility = abilityConfig.CreateAbilityComponent(transform);
        onAbilityEquipped.Invoke(currentAbility);
    }

    public void PullTrigger()
    {
        if (currentAbility)
            currentAbility.PullTrigger();
    }
    public void ReleaseTrigger()
    {
        if (currentAbility)
            currentAbility.ReleaseTrigger();
    }
    public void Reload()
    {
        if (currentAbility)
            currentAbility.Reload();
    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawLine(currentAbility.FirePoint.position, currentAbility.FirePoint.position + (currentAbility.FirePoint.forward * 10));
    //}
}
