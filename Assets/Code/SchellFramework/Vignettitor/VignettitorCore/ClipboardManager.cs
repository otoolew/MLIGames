//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   10/18/2015
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace SG.Vignettitor.VignettitorCore
{
    /// <summary>
    /// Keeps track of clipboards for each available type of vignette graph. 
    /// This lets graphs of the same type share clipboard data but prevents 
    /// pasting illegal nodes from a different graph type.
    /// </summary>
    public class ClipboardManager : UtilityManager<IVignetteClipboard>
    {
        /// <summary> Create a new clipboard manager. </summary>
        /// <param name="clipboardType">
        /// The type of the clipboard to use for all graph types. This type 
        /// must implement IVignetteClipboard.
        /// </param>
        public ClipboardManager(Type clipboardType) : base(clipboardType)
        {
        }

        /// <summary>
        /// Get the clipboard for use with the given graph type.
        /// </summary>
        /// <param name="graphType">
        /// A type that must derive from VignetteGraph.
        /// </param>
        /// <returns>
        /// IVignetteClipboard of the type passed in to the constructor.
        /// </returns>
        public IVignetteClipboard GetClipboard(Type graphType)
        {
            return GetInstance(graphType);
        }
    }
}
