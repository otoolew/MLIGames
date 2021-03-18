// -----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Joseph Pasek
//
//  Created: 12/10/2015 1:47:20 PM
// -----------------------------------------------------------------------------

using UnityEngine;

namespace SG.Entities
{
    /// <summary>
    /// Component which identifies a GameObject as representing one or more logical Tags.
    /// </summary>
    public class TagReference : MonoBehaviour
    {
        [SerializeField]
        private Tag[] _tags;
        public Tag[] Tags { get { return _tags; } }

        private void Reset()
        {
            _tags = new Tag[1];
        }
    }
}
