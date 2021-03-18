//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   09/04/2014
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using JetBrains.Annotations;
using SG.Core.OnGUI;
using SG.Vignettitor.NodeViews;
using SG.Vignettitor.Graph;
using SG.Vignettitor.Graph.Drawing;
using SG.Vignettitor.Graph.NodeViews;
using SG.Vignettitor.VignetteData;
using UnityEngine;

namespace SG.Vignettitor.NodeViews
{
    /// <summary>
    /// A node view specifically for vignette nodes. This adds more output 
    /// drawing capability.
    /// </summary>
    public class VignetteNodeView : NodeView
    {
        /// <summary>
        /// Field mappings that use a NodeViewFieldAttribute to indicate that 
        /// they should be drawn automatically.
        /// </summary>
        private List<NodeViewFieldAttribute.FieldAttributePair> autoFields;

        /// <summary> The vignette node that this view draws. </summary>
        public VignetteNode Node
        {
            get { return Target as VignetteNode; }
        }

        public virtual bool DrawNodeViewFields
        {
            get { return true; }
        }

        public override void Initialize(string displayName, object target)
        {
            base.Initialize(displayName, target);
            autoFields = NodeViewFieldAttribute.GetFields<NodeViewFieldAttribute>(target);
        }

        /// <summary>
        /// Returns the active runtime graph in the editor.
        /// </summary>
        [CanBeNull]
        protected VignetteRuntimeGraph Runtime
        {
            get
            {
                VignettitorCore.Vignettitor editor = graphEditor as VignettitorCore.Vignettitor;
                return editor == null ? null : editor.ActiveRuntime;
            }
        }

        /// <summary>
        /// Returns the Runtime instance of this node based on the active runtime in the editor.
        /// </summary>
        [CanBeNull]
        protected VignetteRuntimeNode RuntimeNode
        {
            get { return Runtime == null ? null : Runtime[Node.NodeID]; }
        }
        
        public override void Draw(Rect rect)
        {
            base.Draw(rect);
            OnGUIUtils.DrawBox(idRect, Node.NodeID.ToString(), Color.white, Color.clear);

            if (DrawNodeViewFields)
                DrawFields(rect);
        }

        public virtual void DrawFields(Rect rect)
        {
            if(Event.current.type != EventType.Repaint)
                return;
            float alpha = 1.0f - GetBackgroundIconAlpha();
            if (alpha <= Mathf.Epsilon)
                return;
            Color c = GUI.color;
            GUI.color = new Color(1.0f, 1.0f, 1.0f, alpha);
            Rect cursor = new Rect(rect);
            cursor.height = ROW_HEIGHT;
            cursor.y += ROW_HEIGHT;
            for (int i = 0; i < autoFields.Count; i++)
            {
                object value = autoFields[i].Field.GetValue(Node);
                string display = value == null ? 
                    (autoFields[i].Field.FieldType == typeof(string) ? "" : "null") : 
                    value.ToString();

                if (autoFields[i].Attribute.DrawFieldName)
                {
                    cursor.width = 80;
                    GUI.Label(cursor, autoFields[i].Field.Name, OnGUIUtils.LeftAlignedWrappedLabel);
                    cursor.x = cursor.xMax;
                    cursor.xMax = rect.xMax;
                }
                GUI.Label(cursor, display, OnGUIUtils.LeftAlignedWrappedLabel);
                cursor.y = cursor.yMax;
            }
            GUI.color = c;
        }

        #region -- Outputs ----------------------------------------------------
        /// <summary>
        /// Draw outputs for a node that has variable outputs. This draws an 
        /// output for each connection as well as an "add connection" output.
        /// Any outputs that point to an invalid child will display as magenta.
        /// </summary>
        /// <param name="children">Indices in the graph of the children.</param>
        protected virtual void DrawVariableOutputs(int[] children)
        {
            int count = Node.Children.Length + 1;
            float height = outputRect.width * count;

            Rect o = new Rect(outputRect.x,
                outputRect.center.y - height / 2.0f,
                outputRect.width, height);

            if (count > 1)
                o.y += o.width / 2.0f;

            Rect r = new Rect(o);
            r.height = o.width;
            Outputs = new NodeViewOutput[count];
            for (int i = 0; i < count; i++)
            {
                Outputs[i] = new NodeViewOutput
                {
                    Position = r,
                    State = NodeViewOutput.NodeViewOutputState.Set
                };
                Rect dot = new Rect(r);
                dot.width *= 0.85f;
                dot.height *= 0.85f;
                dot.x += r.width - dot.width;

                if (i == count - 1)
                {
                    Outputs[i].State = NodeViewOutput.NodeViewOutputState.Add;
                    DrawOutput(dot, GraphDrawingAssets.GrayPlusDotTexture);
                }
                else if (children[i] == -1)
                {
                    DrawOutput(dot, GraphDrawingAssets.MagentaDotTexture);
                }
                else
                {
                    DrawOutput(dot, GraphDrawingAssets.GrayDotTexture);
                }
                r.y += r.height;
            }
        }

        protected void DrawOutput(Rect r, Texture t)
        {
            GUI.DrawTexture(r, t);
            graphEditor.DrawAdapter.AddCursorRect(r, DrawAdapter.MouseCursor.Link);
        }

        /// <summary>
        /// Draw outputs for a node that a fixed number of outputs.
        /// </summary>
        /// <param name="children">Indices in the graph of the children.</param>
        protected virtual void DrawFixedOutputs(int[] children)
        {
            int count = Node.OutputRule.Value;
            if (Node.OutputRule.Rule == OutputRule.RuleType.Passthrough)
                count = 1;
            float height = outputRect.width * count;

            Rect o = new Rect(outputRect.x,
                outputRect.center.y - height / 2.0f,
                outputRect.width, height);

            if (count > 1)
                o.y += o.width / 2.0f;

            Rect r = new Rect(o);
            r.height = o.width;
            Outputs = new NodeViewOutput[count];
            for (int i = 0; i < count; i++)
            {
                Outputs[i] = new NodeViewOutput
                {
                    Position = r,
                    State = NodeViewOutput.NodeViewOutputState.Set
                };
                Rect dot = new Rect(r);
                dot.width *= 0.85f;
                dot.height *= 0.85f;
                dot.x += r.width - dot.width;

                if (i >= children.Length || children[i] == -1)
                {
                    Outputs[i].State = NodeViewOutput.NodeViewOutputState.Empty;
                    if (Node.OutputRule.Rule == OutputRule.RuleType.Passthrough)
                        DrawOutput(dot, GraphDrawingAssets.GrayDotTexture);
                    else
                        DrawOutput(dot, GraphDrawingAssets.MagentaDotTexture);
                }
                else
                {
                    DrawOutput(dot, GraphDrawingAssets.GrayDotTexture);
                }
                r.y += r.height;
            }
        }

        /// <summary>
        /// Draw outputs for a node depending on the nodes output rule.
        /// </summary>
        /// <param name="children">Indices in the graph of the children.</param>
        public override void DrawOutputs(int[] children)
        {
            base.DrawOutputs(children);
            if (Node.OutputRule.Rule == OutputRule.RuleType.Variable)
                DrawVariableOutputs(children);
            else
                DrawFixedOutputs(children);
        }
        #endregion -- Outputs -------------------------------------------------
    }
}
