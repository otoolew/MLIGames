// ------------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 3/23/2016 11:31:33 AM
// ------------------------------------------------------------------------------

using SG.Core.OnGUI;
using UnityEngine;

namespace SG.Vignettitor.NodeViews
{
    public abstract class NodeViewStringView : VignetteNodeView
    {
        protected static readonly Color DrawColor =
            new Color(0x96 / 255f, 0xF3 / 255f, 0x8F / 255f);

        protected override Color DefaultColor { get { return DrawColor; } }

        public override void Draw(Rect rect)
        {
            IHasNodeViewString n = Node as IHasNodeViewString;
            base.Draw(rect);
            if (n != null)
                GUILayout.Label(n.ViewString, OnGUIUtils.LeftAlignedWrappedLabel);
        }
    }

    /// <summary>
    /// A simple interface that indicates this node has a single string which should
    /// be displayed for its NodeView (which means its NodeView class should inherit
    /// directly from NodeViewStringView).
    /// </summary>
    public interface IHasNodeViewString
    {
        string ViewString { get; }
    }

    public interface ISimpleNodeEditView
    {
        string[] EditViewPropertyNames { get; }
    }
}
