using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class HitboxTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        HealthComponent healthComp = other.GetComponent<HealthComponent>();
        if (healthComp)
        {

        }
    }
}
