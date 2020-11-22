using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Collider))]
public class HitCollider : MonoBehaviour
{
    [SerializeField] private Collider colliderComp;
    public Collider ColliderComp { get => colliderComp; set => colliderComp = value; }

    [SerializeField] private HealthComponent healthComp;
    public HealthComponent HealthComp { get => healthComp; set => healthComp = value; }

}
