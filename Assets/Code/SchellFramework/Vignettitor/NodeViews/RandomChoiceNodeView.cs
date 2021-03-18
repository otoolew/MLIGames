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
    /// <summary> Node View for a random choice node. </summary>
    [NodeView(typeof(RandomChoiceNode))]
    public class RandomChoiceNodeView : VignetteNodeView
	{
        /// <summary> Icon to display on the node. </summary>
        private readonly BuiltInTexture2D icon = new BuiltInTexture2D("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");

        protected override Color DefaultColor
        { get { return new Color(1f, 0.898f, 0.6f); } }

        public override void Draw(Rect rect)
        {
            base.Draw(rect);
            Rect iconRect = new Rect(bodyRect);
            iconRect.height = Mathf.Min(iconRect.height * .75f, 64.0f);
            iconRect.width = iconRect.height;
            iconRect.y = titleRect.yMax + (bodyRect.height / 2.0f) - (iconRect.height / 2.0f);
            iconRect.x = (titleRect.width / 2.0f) - (iconRect.width / 2.0f);
            GUI.DrawTexture(iconRect, icon);
        }
	}
}
