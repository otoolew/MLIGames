using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public Quaternion MoveDirection { get { return transform.rotation; }}
    public abstract CharacterMovement MovementComp { get; set; }
    public abstract HealthComponent HealthComp { get; set; }

}
