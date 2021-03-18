// -----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Joseph Pasek
//
//  Created: 12/10/2015 11:54:18 AM
// -----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using SG.Core;
using UnityEngine;

namespace SG.Entities
{
    /// <summary>
    /// Component which identifies a GameObject as representing a logical Entity.
    /// </summary>
    public class EntityReference : MonoBehaviour
    {
        [SerializeField]
        private Entity _entity;
        public Entity Entity { get { return _entity; } }

        private Dictionary<Tag, List<TagReference>> _tagToRefs;

        private Dictionary<Tag, List<TagReference>> TagToRefs
        {
            get
            {
                if (_tagToRefs != null)
                    return _tagToRefs;
                
                // Build tag mapping for all tagged objects in this Entity.
                TagReference[] tagRefs = GetComponentsInChildren<TagReference>(true);
                _tagToRefs = new Dictionary<Tag, List<TagReference>>(tagRefs.Length);
                for (int i = 0; i < tagRefs.Length; i++)
                {
                    for (int j = 0; j < tagRefs[i].Tags.Length; j++)
                    {
                        Tag t = tagRefs[i].Tags[j];
                        List<TagReference> refList;
                        if (!_tagToRefs.TryGetValue(t, out refList))
                            _tagToRefs.Add(t, new List<TagReference> { tagRefs[i] });
                        else
                            refList.Add(tagRefs[i]);
                    }
                }
                return _tagToRefs;
            }
        }

        private TagMatcher _tagMatcher;

        public TagMatcher TagMatcher
        {
            get { return _tagMatcher ?? (_tagMatcher = new TagMatcher(TagToRefs.Keys.ToArray(), null, null)); }
        }

        // Register on Awake
        private void Awake()
        {
            // Register with the App
            Services.Locate<IEntityTracker>().Register(this);
        }

        // Unregister on Destroy
        private void OnDestroy()
        {
            Services.Locate<IEntityTracker>().Unregister(this);
        }

        /// <summary>
        /// Returns true if this EntityReference has a TagReference Component in its child hierarchy that contains the provided Tag
        /// </summary>
        public bool HasTag(Tag searchTag)
        {
            return TagToRefs.ContainsKey(searchTag);
        }

        public TagReference Get(Tag searchTag)
        {
            List<TagReference> tagList;
            if (TagToRefs.TryGetValue(searchTag, out tagList) && tagList.Count > 0)
                return tagList[0];
            return null;
        }

        public List<TagReference> GetAll(Tag searchTag)
        {
            List<TagReference> tagList;
            return TagToRefs.TryGetValue(searchTag, out tagList) ? tagList : null;
        }
    }
}
