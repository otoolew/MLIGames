// -----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Joseph Pasek
//
//  Created: 12/10/2015 11:58:38 AM
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SG.Core;
using UnityEngine.Assertions;

namespace SG.Entities
{
    /// <summary>
    /// Service class which maintains awareness of all EntityReference components in the scene.
    /// </summary>
    [ServiceOnDemand(ServiceType = typeof(IEntityTracker))]
    public class EntityTracker : IEntityTracker
    {
        private readonly Dictionary<Entity, List<EntityReference>> _entityToRef =
            new Dictionary<Entity, List<EntityReference>>();

        /// <summary>
        /// Registers the EntityReference with the tracker.
        /// </summary>
        public void Register(EntityReference reference)
        {
            List<EntityReference> refList;
            if (!_entityToRef.TryGetValue(reference.Entity, out refList))
                _entityToRef.Add(reference.Entity, new List<EntityReference> { reference });
            else
                refList.Add(reference);
        }

        /// <summary>
        /// Unregisters the EntityReference from the tracker.
        /// </summary>
        public void Unregister(EntityReference reference)
        {
            List<EntityReference> refList;
            if (!_entityToRef.TryGetValue(reference.Entity, out refList))
                throw new EntityException(string.Concat("Attempted to unregister an EntityReference that was not registered: ", reference.Entity.SafeName()));
            
            refList.Remove(reference);
            if (refList.Count < 1)
                _entityToRef.Remove(reference.Entity);
        }

        /// <summary>
        /// Gets an EntityReference with the provided Entity
        /// </summary>
        public EntityReference Find(Entity entity)
        {
            List<EntityReference> found;
            if (!_entityToRef.TryGetValue(entity, out found))
                throw new EntityException(string.Concat("Could not find EntityReference for: ", entity.SafeName()));
            return found[0];
        }

        /// <summary>
        /// Gets all EntityReference Components with the provided Entity
        /// </summary>
        public List<EntityReference> FindAll(Entity entity)
        {
            List<EntityReference> found;
            if (!_entityToRef.TryGetValue(entity, out found))
                throw new EntityException(string.Concat("Could not find EntityReference for: ", entity.SafeName()));
            return found;
        }

        /// <summary>
        /// Locates an EntityReference that has all of the supplied Tags.
        /// </summary>
        /// <param name="entity">The Entity</param>
        /// <param name="tags">The Tags an EntityReference must have in order to be accepted.</param>
        /// <returns>An EntityReference that has all of the provided Tags.</returns>
        public EntityReference FindWithTags(Entity entity, params Tag[] tags)
        {
            TagMatcher matcher = new TagMatcher(tags, null, null);

            List<EntityReference> refs = FindAll(entity);
            for (int i = 0; i < refs.Count; i++)
            {
                if (matcher.Score(refs[i].TagMatcher) >= 0)
                    return refs[i];
            }
            throw new EntityException("Could not find EntityReference for Entity '{0}' matching {1}", entity.SafeName(), matcher);
        }

        /// <summary>
        /// Checks if an EntityReference with the provided Entity is registered.
        /// </summary>
        public bool Has(Entity entity)
        {
            return _entityToRef.ContainsKey(entity);
        }

        /// <summary>
        /// Checks if anything with the provided Entity & Tag is registered.
        /// </summary>
        public bool HasWithTag(Entity entity, Tag tag)
        {
            Assert.IsNotNull(tag, "Tag must not be null. Use Has(Entity) if you don't care about tags.");

            TagMatcher matcher = new TagMatcher(tag);

            List<EntityReference> refs = FindAll(entity);
            for (int i = 0; i < refs.Count; i++)
            {
                if (matcher.Score(refs[i].TagMatcher) >= 0)
                    return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Service contract for IEntityTracker;
    /// </summary>
    public interface IEntityTracker : IService
    {
        /// <summary>
        /// Registers the EntityReference with the tracker.
        /// </summary>
        void Register([NotNull] EntityReference entityReference);

        /// <summary>
        /// Unregisters the EntityReference from the tracker.
        /// </summary>
        void Unregister([NotNull] EntityReference entityReference);

        /// <summary>
        /// Gets an EntityReference with the provided Entity
        /// </summary>
        EntityReference Find(Entity entity);
        
        /// <summary>
        /// Gets all EntityReference Components with the provided Entity
        /// </summary>
        List<EntityReference> FindAll([NotNull] Entity entity);

        /// <summary>
        /// Locates an EntityReference that has all of the supplied Tags.
        /// </summary>
        /// <param name="entity">The Entity</param>
        /// <param name="tags">The Tags an EntityReference must have in order to be accepted.</param>
        /// <returns>An EntityReference that has all of the provided Tags.</returns>
        EntityReference FindWithTags([NotNull] Entity entity, params Tag[] tags);

        /// <summary>
        /// Checks if an EntityReference with the provided Entity is registered.
        /// </summary>
        bool Has([NotNull] Entity entity);

        /// <summary>
        /// Checks if anything with the provided Entity & Tag is registered.
        /// </summary>
        bool HasWithTag([NotNull] Entity entity, [NotNull] Tag tag);
    }

    public class EntityException : Exception
    {
        [StringFormatMethod("msg")]
        public EntityException(string msg, params object[] args) :
            base(string.Format(msg, args)) { }
    }
}
