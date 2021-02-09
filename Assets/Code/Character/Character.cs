using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    /// <summary>
    /// The Characters Movement Component
    /// </summary>
    public abstract Rigidbody RigidbodyComp { get; set; }

    /// <summary>
    /// The Characters Movement Component
    /// </summary>
    public abstract CharacterMovement MovementComp { get; set; }
    /// <summary>
    /// The Characters Rotation Component
    /// </summary>
    public abstract CharacterRotation CharacterRotation { get; set; }
    /// <summary>
    /// The Characters Health Component
    /// </summary>
    public abstract HealthComponent HealthComp { get; set; }
    /// <summary>
    /// The Transform a character is focused on
    /// </summary>
    public abstract Transform FocusTarget { get; set; }
    /// <summary>
    /// The Transform other characters is focused on
    /// </summary>
    public abstract Transform FocusPoint { get; set; }

    public Vector3 WorldLocation { get => transform.position;}
    public Quaternion Rotation { get { return gameObject.transform.rotation; }}
    public abstract bool IsValid();
    public Quaternion GetRotation() { return gameObject.transform.rotation; }

    private void OnDrawGizmos()
    {
        
    }
}
