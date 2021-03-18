// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Eric Policaro
// 
//  Date: 02/15/2016
// ----------------------------------------------------------------------------

using SG.Core.OnGUI;
using SG.Vignettitor.Graph.Drawing;
using SG.Vignettitor.Graph.NodeViews;
using SG.Vignettitor.NodeViews;
using SG.Vignettitor.VignetteData;
using UnityEngine;

namespace SG.Vignettitor.Nodes
{
    [NodeView(typeof(SubgraphNode))]
    public class SubgraphView : VignetteNodeView
    {
        protected override Color DefaultColor
        {
            get
            {
                bool isRunning = Runtime != null && Runtime.CurrentNodeId == Node.NodeID;
                return isRunning ? new Color32(46, 204, 113, 255) : new Color32(211, 84, 0, 255);
            }
        }

        protected SubgraphNode SubgraphNode
        {
            get { return _node ?? (_node = (SubgraphNode) Node); }
        }

        protected VignetteGraph Graph
        {
            get { return SubgraphNode.Graph; }
            set { SubgraphNode.Graph = value; }
        }

        protected override void DrawTitle()
        {
            string label = Graph != null ? Graph.name : "Empty Subgraph";
            GUI.Label(titleRect, label, OnGUIUtils.CenteredBlackTextStyle);
        }

        public override void Draw(Rect rect)
        {
            base.Draw(rect);
            Rect r = new Rect(bodyRect);
            r.height = Mathf.Min(r.height * .6f, 64f);
            r.width = r.height * 1.81f;

            r.y = titleRect.yMax + (bodyRect.height / 2f) - r.height/2f;
            r.x = (titleRect.width / 2f) - (r.width / 2f);
            
            GUI.color = new Color(1f, 1f, 1f, 0.45f);
            GUI.DrawTexture(r, GraphDrawingAssets.SubgraphIcon);
            GUI.color = Color.white;
        }

        public override string GetConnectionLabel(int c)
        {
            if (SubgraphNode.Outlets.Count == 0)
                return "";

            for (int i = 0; i < SubgraphNode.Outlets.Count; i++)
            {
                if (i == c)
                    return SubgraphNode.Outlets[i].OutletName;
            }

            return "";
        }

        private SubgraphNode _node;
    }
}