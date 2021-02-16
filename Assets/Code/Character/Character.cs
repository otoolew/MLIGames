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
    /// The Characters Animation Component
    /// </summary>
    public abstract Animator AnimatorComp { get; set; }

    /// <summary>
    /// The Characters Movement Component
    /// </summary>
    public abstract CharacterMovement MovementComp { get; set; }
    /// <summary>
    /// The Characters Movement Component
    /// </summary>
    public abstract CharacterRotation RotationComp { get; set; }
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
    public abstract Transform TargetPoint { get; set; }

    /// <summary>
    /// The Location of the Character in the world.
    /// </summary>
    public Vector3 WorldLocation { get => transform.position;}

    /// <summary>
    /// Is the character a valid active player in the scene.
    /// </summary>
    /// <returns></returns>
    public abstract bool IsValid();

    /// <summary>
    /// Called when a character dies.
    /// </summary>
    public abstract void OnDeath();

    #region Editor
    /// <summary>
    /// On Validate is only called in Editor. By performing checks here was can rest assured they will not be null.
    /// Usually what is in the Components region is in here.
    /// </summary>
    protected virtual void OnValidate()
    {
        if (RigidbodyComp == null)
        {
            Debug.LogError("No RigidbodyComp assigned");
        }

        if (AnimatorComp == null)
        {
            //Debug.LogError("No AnimatorComp assigned");
        }

        if (MovementComp == null)
        {
            //Debug.LogError("No MovementComp assigned");
        }

        if (RotationComp == null)
        {
            Debug.LogError(gameObject.name + " has NO RotationComp assigned");
        }

        if (HealthComp == null)
        {
            Debug.LogError("No Health Component assigned");
        }

        if (HealthComp == null)
        {
            Debug.LogError("No Health Component assigned");
        }
    }
    #endregion
}
