//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   08/21/2015
//-----------------------------------------------------------------------------

using SG.Core.OnGUI;
using SG.Vignettitor.Graph.Drawing;
using SG.Vignettitor.Graph.Layout;
using SG.Vignettitor.Graph.NodeViews;
using SG.Vignettitor.Nodes;
using UnityEngine;

namespace SG.Vignettitor.NodeViews
{
    /// <summary>
    /// Draws a vignettitor representation of a jump node, which includes a 
    /// rendering of the node it will jump to.
    /// </summary>
    [NodeView(typeof(JumpNode))]
    public class JumpNodeView : VignetteNodeView, ILayoutPriority
    {
        /// <summary>
        /// Should this jump node display it's output connection?
        /// </summary>
        private bool showConnection;

        /// <summary>
        /// Where to draw the toggle output display button.
        /// </summary>
        private Rect toggleRect;

        private readonly ClickMonitor clickMonitor = new ClickMonitor(0.2f, 0.2f);

        #region -- VignetteNodeView Overrides ---------------------------------
        protected override Color DefaultColor
        { get { return new Color32(242, 163, 44, 255); } }

        protected override void DrawFixedOutputs(int[] children)
        {
            Rect o = new Rect(outputRect.x,
                outputRect.center.y - outputRect.width / 2.0f,
                outputRect.width, outputRect.width);

            Rect r = new Rect(o);
            r.height = o.width;
            Outputs = new NodeViewOutput[1];
            Outputs[0] = new NodeViewOutput
            {
                Position = r,
                State = showConnection ?
                NodeViewOutput.NodeViewOutputState.Set : NodeViewOutput.NodeViewOutputState.Hidden
            };
            Rect dot = new Rect(r);
            dot.width *= 0.85f;
            dot.height *= 0.85f;
            dot.x += r.width - dot.width;

            if (children.Length != 1 || children[0] == -1)
            {
                Outputs[0].State = NodeViewOutput.NodeViewOutputState.Empty;
                DrawOutput(dot, GraphDrawingAssets.MagentaDotTexture);
            }
            else
            {
                DrawOutput(dot, GraphDrawingAssets.GrayDotTexture);
            }
            r.y += r.height;
        }

        protected override void StoreRects(Rect rect)
        {
            base.StoreRects(rect);
            toggleRect = new Rect(titleRect);
            toggleRect.y = rect.height - toggleRect.height;
            toggleRect.x = idRect.xMax + 2;
            toggleRect.width -= toggleRect.x;
        }

        /// <summary>
        /// Draw the jump node and attempt to draw a preview of the destination
        /// node inside.
        /// </summary>
        /// <param name="rect"></param>
        public override void Draw(Rect rect)
        {
            base.Draw(rect);

            showConnection = GUI.Toggle(toggleRect, showConnection, "Show Connection");

            if (Node.Children != null &&
                Node.Children.Length > 0 &&
                Node.Children[0] != null)
            {
                SG.Vignettitor.VignettitorCore.Vignettitor v = graphEditor as SG.Vignettitor.VignettitorCore.Vignettitor;
                int index = v.GetIndex(Node.Children[0]);

                Matrix4x4 mat = GUI.matrix;
                GUIUtility.ScaleAroundPivot(new Vector2(0.7f, 0.7f), rect.center);

                // Prevent some expensive inception stuff.
                if (Node.Children[0] is JumpNode)
                {
                    JumpNodeView j = v.GetViewState(index).nodeView as JumpNodeView;
                    j.SimpleDraw(rect);
                }
                else
                {
                    v.GetViewState(index).nodeView.Draw(rect);
                }

                if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                {
                    clickMonitor.Down(Event.current.mousePosition);
                    //Event.current.Use();
                }

                if (Event.current.type == EventType.MouseUp && rect.Contains(Event.current.mousePosition))
                {
                    int clicks = clickMonitor.Up(Event.current.mousePosition);
                    if (clicks == 2)
                    {
                        v.SelectionManager.Clear();
                        v.SelectionManager.AddToSelection(index);
                        v.AnimatedFocusOnSelection();
                        Event.current.Use();
                    }
                }

                GUI.matrix = mat;
            }
        }
        #endregion -- VignetteNodeView Overrides ------------------------------

        /// <summary>
        /// Used to call the base draw function to prevent drawing the 
        /// destination node preview. This avoids jump nodes rendering jump 
        /// nodes inside of jump nodes inside of jump nodes....
        /// 
        /// It also prevents an infinit recursion if twp jump nodes point to 
        /// each other.
        /// </summary>
        /// <param name="rect">Rect to draw node in.</param>
        private void SimpleDraw(Rect rect)
        {
            base.Draw(rect);
        }

        int ILayoutPriority.GetLayoutPriority()
        {
            // These should be laid out last so they will not appear like a 
            // direct parent.
            return -1000;
        }
    }
}
