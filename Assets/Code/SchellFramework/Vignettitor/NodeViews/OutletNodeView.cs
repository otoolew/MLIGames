// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Eric Policaro
// 
//  Date: 02/17/2016
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using SG.Vignettitor.Graph.NodeViews;
using SG.Vignettitor.Nodes;
using SG.Vignettitor.NodeViews;
using UnityEngine;

namespace Scripts.StateMachines.NodeViews
{
    [NodeView(typeof(OutletNode))]
    public class OutletNodeView : VignetteNodeView
    {
        protected override Color DefaultColor
        {
            get { return new Color32(231, 76, 60, 255); }
        }

        public override void Draw(Rect rect)
        {
            base.Draw(rect);
            OutletNode n = (OutletNode)Node;
            DrawRows(string.Format("{0}: {1}\n{2}", n.OutletName, n.Order, n.Output));
        }
    }
}