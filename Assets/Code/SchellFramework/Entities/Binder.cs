// ------------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 12/15/2015 9:49:49 AM
// ------------------------------------------------------------------------------

using JetBrains.Annotations;
using SG.Core;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SG.Entities
{
    /// <summary>
    /// Class for shared functionality for locating a gameObject in the scene via Entity + Tag references.
    /// </summary>
    [Serializable]
    public class Binder
    {
        [SerializeField]
        private Entity _entity;
        public Entity Entity { get { return _entity; } }

        [SerializeField]
        private Tag _tag;
        public Tag Tag { get { return _tag; } }

        public GameObject Target
        {
            get { return BindTarget(); }
        }

        /// <summary>
        /// Bind with a fallback source if the definition of the binder is incomplete.
        /// Used in Sequences to reduce boilerplate assignment.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [CanBeNull]
        private GameObject BindTarget([NotNull] IBinderSource source)
        {
            if (!_tag && !_entity) // fallback to source game object
            {
                IBinderSource iteratorSource = source;
                do
                {
                    if (iteratorSource.GameObject)
                        return iteratorSource.GameObject;
                    iteratorSource = iteratorSource.Parent;
                } while (iteratorSource != null);
                return null;
            }

            EntityReference entityTarget; // must locate an EntityReference
            if (_entity)
                entityTarget = Services.Locate<IEntityTracker>().Find(_entity);
            else // fallback to source entity reference
            {
                IBinderSource iteratorSource = source;
                do
                {
                    entityTarget = iteratorSource.EntityReference;
                    iteratorSource = iteratorSource.Parent;
                } while (!entityTarget && iteratorSource != null);
            }

            return BindFromEntityReference(entityTarget);
        }

        /// <summary>
        /// Simple bind without a fallback source.
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        private GameObject BindTarget()
        {
            return _entity ? BindFromEntityReference(Services.Locate<IEntityTracker>().Find(_entity)) : null;
        }

        [CanBeNull]
        private GameObject BindFromEntityReference([CanBeNull] EntityReference entityReference)
        {
            if (!entityReference) // No entity found in chain
                return null;

            if (!_tag) // return entity root
                return entityReference.gameObject;

            TagReference tagReference = entityReference.Get(_tag);
            return tagReference ? tagReference.gameObject : null;
        }

        public TComponent GetComponent<TComponent>()
        {
            GameObject bindTarget = BindTarget();
            if (!bindTarget)
                throw new EntityException(string.Concat("Unable to bind: ", this));
            return bindTarget.GetComponent<TComponent>();
        }

        public BinderState Bind([NotNull] IBinderSource source)
        {
            GameObject bindTarget = BindTarget(source);
            if (!bindTarget)
                throw new EntityException(string.Concat("Unable to bind: ", _entity.SafeName(), " -> ", _tag.SafeName()));
            return new BinderState(bindTarget);
        }

        public BinderComponentState AddComponent<TComponent>([NotNull] IBinderSource source, out TComponent component) where TComponent : Component
        {
            GameObject bindTarget = BindTarget(source);
            if (!bindTarget)
                throw new EntityException(string.Concat("Unable to bind: ", _entity.SafeName(), " -> ", _tag.SafeName()));

            component = bindTarget.AddComponent<TComponent>();
            return new BinderComponentState(component, true);
        }

        public BinderComponentState GetComponent<TComponent>([NotNull] IBinderSource source, out TComponent component) where TComponent : Component
        {
            GameObject bindTarget = BindTarget(source);
            if (!bindTarget)
                throw new EntityException(string.Concat("Unable to bind: ", _entity.SafeName(), " -> ", _tag.SafeName()));

            component = bindTarget.GetComponent<TComponent>();
            if (!component)
                throw new EntityException(string.Concat("Unable to bind: ", _entity.SafeName(), " -> ", _tag.SafeName()));
            return new BinderComponentState(component, false);
        }

        /// <summary>
        /// Get or Add a component based upon an optional component validator (for instance, return a script player
        /// unless it is currently playing something).
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="source"></param>
        /// <param name="component"></param>
        /// <param name="componentValidator"></param>
        /// <returns></returns>
        public BinderComponentState GetOrAddComponent<TComponent>([NotNull] IBinderSource source, out TComponent component,
            Func<TComponent, bool> componentValidator = null) where TComponent : Component
        {
            GameObject bindTarget = BindTarget(source);
            if (!bindTarget)
                throw new EntityException(string.Concat("Unable to bind: ", _entity.SafeName(), " -> ", _tag.SafeName()));

            component = null;
            TComponent[] candidateComponents = bindTarget.GetComponents<TComponent>();
            for (int i = 0; i < candidateComponents.Length; i++)
            {
                if (componentValidator != null && !componentValidator.Invoke(candidateComponents[i]))
                    continue;

                component = candidateComponents[i];
                break;
            }
            if (component)
                return new BinderComponentState(component, false); // found a component that was acceptible, use it

            // add a new component since we couldn't find one that worked
            component = bindTarget.AddComponent<TComponent>();
            return new BinderComponentState(component, true);
        }

        public override string ToString()
        {
            return !_tag ? _entity.SafeName() : string.Concat(_entity.SafeName(), " -> ", _tag.name);
        }

        public void Reset(Entity defaultEntity, Tag defaultTag)
        {
            _entity = defaultEntity;
            _tag = defaultTag;
        }
    }

    [Serializable]
    public struct BinderState
    {
        [SerializeField]
        private GameObject _target;
        public GameObject Target { get { return _target; } }

        public BinderState(GameObject target)
        {
            _target = target;
        }
    }

    [Serializable]
    public struct BinderComponentState
    {
        [SerializeField]
        private Component _component;

        [SerializeField]
        private bool _added;

        public BinderComponentState(Component component, bool added)
        {
            _component = component;
            _added = added;
        }

        public void Dispose()
        {
            if (_added)
                Object.Destroy(_component);
        }
    }
}
