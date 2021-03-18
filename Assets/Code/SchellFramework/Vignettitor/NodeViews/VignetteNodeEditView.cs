//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   09/04/2014
//-----------------------------------------------------------------------------

using SG.Vignettitor.Graph.NodeViews;
using SG.Vignettitor.VignetteData;

namespace SG.Vignettitor.NodeViews
{
    /// <summary> A NodeEditView specifically for vignette nodes. </summary>
    public abstract class VignetteNodeEditView : NodeEditView
    {
        /// <summary> The vignette node that this view draws. </summary>
        public VignetteNode Node { get { return Target as VignetteNode; } }
    }
}
