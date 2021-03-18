// ------------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 5/6/2016 3:30:14 PM
// ------------------------------------------------------------------------------

using JetBrains.Annotations;
using UnityEngine;

namespace SG.Entities
{
    /// <summary>
    /// Interface which defines a segment of an inheritance chain for the Binder system.
    /// 
    /// It's expected to function as a means of providing fallbacks as shortcuts for Binders.
    /// 
    /// The primary use case is inside the Vignette system. Consider a node which plays a sound
    /// by parenting a AudioSource to a specified GameObject, specified by a Binder.
    /// This Vignette is likely going to be executed on a specific GameObject (via a component).
    /// If we wish to play the sound on that exact object, we shouldn't need to specify it again.
    /// 
    /// The Fallback if the Binder is empty is the GameObject executing the current vignette
    /// (specified by the runtime graph).
    /// 
    /// The Fallback if Binder specifies Tag but not Entity is that Tag inside the Entity currently
    /// executing the current vignette.
    /// </summary>
    public interface IBinderSource
    {
        IBinderSource Parent { [CanBeNull] get; }
        GameObject GameObject { [CanBeNull] get; }
        EntityReference EntityReference { [CanBeNull] get; }
    }

    public class BinderSource : IBinderSource
    {
        public static readonly BinderSource Empty = new BinderSource(null, null);

        public IBinderSource Parent { get; private set; }
        public GameObject GameObject { get; private set; }
        public EntityReference EntityReference { get; private set; }

        public BinderSource([CanBeNull] GameObject executor, [CanBeNull] IBinderSource parent = null)
        {
            GameObject = executor;
            Parent = parent;

            if (GameObject == null)
                return;

            // search up hierarchy for EntityReference
            Transform iterTransform = GameObject.transform;
            while (iterTransform)
            {
                EntityReference = iterTransform.GetComponent<EntityReference>();
                if (EntityReference)
                    break;
                iterTransform = iterTransform.parent;
            }
        }
    }
}
