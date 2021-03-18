//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   05/20/2014
//-----------------------------------------------------------------------------

using SG.Vignettitor.VignetteData;

namespace SG.Vignettitor.Editor.Search
{
    /// <summary>
    /// An individual result from a vignette search.
    /// </summary>
    public class VignetteSearchResult
    {
        /// <summary> The graph indicated in the search. </summary>
        public VignetteGraph vignette;

        /// <summary>
        /// The node indicated in the search. This may be null if the search 
        /// type is only identifying the graph and not nodes.
        /// </summary>
        public VignetteNode node;

        public VignetteSearchResult(VignetteGraph vignette, VignetteNode node)
        {
            this.vignette = vignette;
            this.node = node;
        }
    }
}
