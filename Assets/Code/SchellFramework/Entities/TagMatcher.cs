// ------------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 11/6/2015 12:30:47 PM
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SG.Core;
using UnityEngine.Assertions;

namespace SG.Entities
{
    /// <summary>
    /// Utility class for matching Tags and scoring various Tag set intersections.
    /// 
    /// Supports checking based on three tag sets:
    /// - required tags - without these, the score of the match is always -1
    /// - forbid tags - if any of these are present, the score of the match is always -1
    /// - optional tags - if these are present, they add to the score of the match
    /// 
    /// The "score" of a match is calculated as # of required tags + # of optional tags which are present.
    /// This is to cause "more difficult" matches to be scored highly.
    /// </summary>
    public class TagMatcher : IEquatable<TagMatcher>
    {
        private readonly HashSet<Tag> _requiredTagSet;
        private readonly HashSet<Tag> _forbidTagSet;
        private readonly HashSet<Tag> _optionalTagSet;

        private readonly HashSet<Tag> _tempTagSet;

        public TagMatcher(Tag requiredTag)
        {
            _requiredTagSet = new HashSet<Tag> { requiredTag };
        }

        public TagMatcher(Tag[] requiredTags, Tag[] optionalTags, Tag[] forbidTags)
        {
            bool hasRequired = requiredTags.HasAny();
            bool hasForbid = forbidTags.HasAny();
            _requiredTagSet = hasRequired ? new HashSet<Tag>(requiredTags) : null;
            _forbidTagSet = hasForbid ? new HashSet<Tag>(forbidTags) : null;
            if (hasRequired && hasForbid)
                Assert.IsFalse(_requiredTagSet.Overlaps(_forbidTagSet),
                    "Required & Forbid Tags overlap, this is bad logic.");

            if (!optionalTags.HasAny())
                return;

            _optionalTagSet = new HashSet<Tag>(optionalTags);
            _tempTagSet = PrepareOptionalTagSet();
        }

        public TagMatcher(TagMatcher initial, Tag[] addRequiredTags, Tag[] addForbidTags, Tag[] addOptionalTags)
        {
            _requiredTagSet = CombineToNew(initial._requiredTagSet, addRequiredTags);
            _forbidTagSet = CombineToNew(initial._forbidTagSet, addForbidTags);
            if (_requiredTagSet != null && _forbidTagSet != null)
                Assert.IsFalse(_requiredTagSet.Overlaps(_forbidTagSet),
                    "Required & Forbid Tags overlap, this is bad logic.");

            _optionalTagSet = CombineToNew(initial._optionalTagSet, addOptionalTags);
            _tempTagSet = PrepareOptionalTagSet();
        }

        private HashSet<Tag> PrepareOptionalTagSet()
        {
            if (_optionalTagSet == null)
                return null;

            if (_requiredTagSet != null)
                _optionalTagSet.ExceptWith(_requiredTagSet); // Remove any required tags from the optional set
            if (_forbidTagSet != null)
                _optionalTagSet.ExceptWith(_forbidTagSet);
            return new HashSet<Tag>(_optionalTagSet);
        }

        private static HashSet<Tag> CombineToNew(HashSet<Tag> tagSet, Tag[] tagArray)
        {
            bool hasSet = tagSet != null;
            bool hasArray = tagArray.HasAny();
            if (!hasSet && !hasArray)
                return null;
            HashSet<Tag> newSet = hasSet ? new HashSet<Tag>(tagSet) : new HashSet<Tag>(tagArray);
            if (hasSet && hasArray)
                newSet.UnionWith(tagArray);
            return newSet;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((TagMatcher)obj);
        }

        public bool Equals(TagMatcher other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return HashSetEquals(_requiredTagSet, other._requiredTagSet)
                && HashSetEquals(_forbidTagSet, other._forbidTagSet)
                && HashSetEquals(_optionalTagSet, other._optionalTagSet);
        }

        private static bool HashSetEquals(HashSet<Tag> tags1, HashSet<Tag> tags2)
        {
            if (tags1 == null)
                return tags2 == null || tags2.Count == 0;
            return tags2 == null ? tags1.Count == 0 : tags1.SetEquals(tags2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_requiredTagSet != null ? _requiredTagSet.GetHashCode() : 0) * 397)
                    ^ (_optionalTagSet != null ? _optionalTagSet.GetHashCode() : 0);
            }
        }

        public static bool operator ==(TagMatcher matcher1, TagMatcher matcher2)
        {
            if ((object)matcher1 == null || (object)matcher2 == null)
                return Equals(matcher1, matcher2);
            return matcher1.Equals(matcher2);
        }

        public static bool operator !=(TagMatcher matcher1, TagMatcher matcher2)
        {
            if ((object)matcher1 == null || (object)matcher2 == null)
                return !Equals(matcher1, matcher2);
            return !matcher1.Equals(matcher2);
        }

        /// <summary>
        /// Return the Tag Score of the given tagMatcher evaluated against us
        /// Score is -1 if the tags do not meet the required/forbid tags
        /// Score is otherwise equal to the number of required tags + the number of optional tags matched
        /// </summary>
        public int Score(TagMatcher otherMatcher)
        {
            int count;
            if (_requiredTagSet == null)
                count = 0;
            else if (_requiredTagSet.IsSubsetOf(otherMatcher._requiredTagSet))
                count = _requiredTagSet.Count;
            else
                return -1;

            if (_forbidTagSet != null && _forbidTagSet.Overlaps(otherMatcher._requiredTagSet))
                return -1;

            if (_optionalTagSet == null)
                return count;

            // Reset the temp tag set so we can do our comparison
            _tempTagSet.UnionWith(_optionalTagSet);
            _tempTagSet.IntersectWith(otherMatcher._requiredTagSet);
            return count + _tempTagSet.Count;
        }

        private static StringBuilder _sharedStringBuilder;

        public override string ToString()
        {
            if (_sharedStringBuilder == null)
                _sharedStringBuilder = new StringBuilder(30);
            _sharedStringBuilder.Length = 0;
            _sharedStringBuilder.Append("Tags ");
            AppendTags(_requiredTagSet, "Req: ");
            _sharedStringBuilder.Append(" ");
            AppendTags(_optionalTagSet, "Opt: ");
            return _sharedStringBuilder.ToString();
        }

        private static void AppendTags(HashSet<Tag> tagSet, string prepend)
        {
            if (tagSet == null || tagSet.Count <= 0)
                return;

            bool first = true;
            HashSet<Tag>.Enumerator tagSetEnumerator = tagSet.GetEnumerator();
            while (tagSetEnumerator.MoveNext())
            {
                _sharedStringBuilder.Append(first ? prepend : ", ");
                first = false;
                _sharedStringBuilder.Append(tagSetEnumerator.Current.name);
            }
        }
    }
}
