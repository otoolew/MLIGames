// ----------------------------------------------------------------------------
//  Copyright © 2017 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   01/27/2017
// ----------------------------------------------------------------------------

using System;
using UnityEngine;

namespace SG.Core.Inspector
{
    /// <summary>
    /// When used on a field in the inspector, the field may be specified to 
    /// not be editable when in runtime, edit time, or both
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
        /// <summary>
        /// Options for the play state of the game in the editor.
        /// </summary>
        public enum PlayMode
        {
            /// <summary>  Any mode. </summary>
            Both,

            /// <summary>
            /// Only when the game is not running in the editor.
            /// </summary>
            Edit,

            /// <summary>
            /// Only when the game is running in the editor.
            /// </summary>
            Runtime
        }

        /// <summary>
        /// In what modes should the filed be locked into read-only?
        /// </summary>
        public PlayMode WhenToLock { get; private set; }

        /// <summary>
        /// When used on a field in the inspector, the field may be specified to 
        /// not be editable when in runtime, edit time, or both.
        /// </summary>
        /// <param name="whenToLock">
        /// In what modes should the filed be locked into read-only?
        /// </param>
        public ReadOnlyAttribute(PlayMode whenToLock)
        {
            WhenToLock = whenToLock;
        }
    }
}