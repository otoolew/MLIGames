//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   08/21/2015
//-----------------------------------------------------------------------------

using SG.Core.OnGUI;
using SG.Vignettitor.Graph.NodeViews;
using SG.Vignettitor.NodeViews;
using UnityEngine;

namespace SG.Vignettitor.Nodes
{
    /// <summary>
    /// NodeView for a Log node. Shows the message that will be output.
    /// </summary>
    [NodeView(typeof(LogNode))]
    public class LogNodeView : VignetteNodeView
    {
        protected override Color DefaultColor
        { get { return new Color32(124, 124, 124, 255); } }
    }
}
