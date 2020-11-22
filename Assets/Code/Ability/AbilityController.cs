﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    [SerializeField] private AbilityComponent currentAbility;
    public AbilityComponent CurrentAbility { get => currentAbility; set => currentAbility = value; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

}
