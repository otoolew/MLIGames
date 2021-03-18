//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   08/28/2015
//-----------------------------------------------------------------------------

using System;

namespace SG.Vignettitor
{
    /// <summary>
    /// Stores version information used within the Vignettitor Module.
    /// </summary>
    public static class VignettitorVersion
    {
        /// <summary>
        /// The current required file version for vignette graphs. Anything 
        /// lower than this will need to be upgraded. This field should be 
        /// updated whenever the data format changes.
        /// </summary>
        public const int SAVE_FORMAT_VERSION = 1;

        /// <summary> 
        /// Version of the vignettitor module available at runtime.
        /// </summary>
        public static readonly Version VIGNETTITOR_VERSION = new Version(2, 3, 1);
    }
}
