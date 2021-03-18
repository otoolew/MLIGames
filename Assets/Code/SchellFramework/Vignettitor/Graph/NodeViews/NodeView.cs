//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   9/04/2014
//-----------------------------------------------------------------------------

using SG.Core.OnGUI;
using SG.Vignettitor.NodeViews;
using UnityEngine;

namespace SG.Vignettitor.Graph.NodeViews
{
    /// <summary>
    /// Used for drawing a node on a graph, a node view defines what a node 
    /// will render like and retains visual state for each graph element.
    /// </summary>
    public class NodeView
    {
        public const float ROW_HEIGHT = 20.0f;
        /// <summary>
        /// Specifies how a node view output should be drawn and interacted 
        /// with.
        /// </summary>
        public class NodeViewOutput
        {
            /// <summary>
            /// State that defines how a node output may be interacted with.
            /// </summary>
            public enum NodeViewOutputState
            {
                Add,
                Empty,
                Set,
                Hidden
            }

            /// <summary>Position to draw a node output.</summary>
            public Rect Position;

            /// <summary>
            /// Defines how an output is drawn and what happens when creating 
            /// from it.
            /// </summary>
            public NodeViewOutputState State;
        }

        #region -- Constants --------------------------------------------------
        /// <summary>
        /// Scale of the output pins compared to the width of a node.
        /// </summary>
        public const float PIN_RATIO = 0.066f;
        #endregion -- Constants -----------------------------------------------

        #region -- Public Fields ----------------------------------------------
        /// <summary>The object that this node view represents.</summary>
        public object Target;

        /// <summary>
        /// The Graph Editor that this node view will be rendered in.
        /// </summary>
        public GraphEditor graphEditor;

        /// <summary>The outputs for this node view.</summary>
        public NodeViewOutput[] Outputs = new NodeViewOutput[0];

        /// <summary>
        /// Should this node view draw labels on empty connections when there 
        /// is only one output?
        /// </summary>
        /// <returns>True to always draw a label.</returns>
        public virtual bool DrawSingleLabel() { return false; }
        #endregion -- Public Fields -------------------------------------------

        #region -- Protected Fields -------------------------------------------
        /// <summary> Top bar of the node where the title is drawn. </summary>
        protected Rect titleRect;

        /// <summary> Right side with the output pins. </summary>
        protected Rect outputRect;

        /// <summary> Left side of the view.. </summary>
        protected Rect inputRect;

        /// <summary> Area where most of the view should be drawn. </summary>
        protected Rect bodyRect;

        /// <summary> Area for the node ID label. </summary>
        protected Rect idRect;

        /// <summary>Text to display as the title for this node.</summary>
        protected string displayName = "---";

        protected BuiltInTexture2D nodeTexture;

        /// <summary>
        /// The next row available to draw content in this node view when 
        /// using the DrawRows functionality.
        /// </summary>
        protected Rect NextRow;  

        /// <summary> Height of text, used by the title. </summary>
        private static float rowHeight = -1;
        protected static float RowHeight
        {
            get
            {
                if (rowHeight < 0)
                    rowHeight = GUI.skin.label.CalcHeight(new GUIContent("W"), 500);
                return rowHeight;
            }
        }

        /// <summary>
        /// Color to draw this node as in the default draw method.
        /// </summary>
        protected virtual Color DefaultColor
        { get { return Color.clear; } }
        #endregion -- Protected Fields ----------------------------------------

        #region -- Protected Methods ------------------------------------------
        /// <summary>
        /// Gets the next row where content can be drawn if using the DrawRows
        /// functionality.
        /// </summary>
        /// <returns>A rectangle to draw in.</returns>
        protected Rect GetNextRow()
        {
            Rect result = new Rect(NextRow);
            NextRow.y += NextRow.height;
            return result;
        }

        /// <summary>
        /// Store all of the sub recs of the node for drawing different 
        /// regions. These are all cached so they do not need to be 
        /// calculated again.
        /// </summary>
        /// <param name="rect">Rect of the node.</param>
        protected virtual void StoreRects(Rect rect)
        {
            titleRect = new Rect(rect) {height = RowHeight};

            outputRect = new Rect(rect);
            outputRect.width = rect.width * PIN_RATIO;
            outputRect.x = rect.width - outputRect.width;

            inputRect = new Rect(rect);
            inputRect.width = outputRect.width;

            idRect = new Rect(inputRect);
            idRect.y = idRect.yMax - titleRect.height - 3;
            idRect.height = titleRect.height;
            idRect.width *= 2;
            idRect.x += 3;

            bodyRect = new Rect(
                inputRect.xMax, titleRect.yMax,
                rect.width - inputRect.width - outputRect.width,
                rect.height - titleRect.height - 2);
        }

        /// <summary>
        /// Draws each string argument in a new row of the node view, advancing
        /// the "NextRow" rect when it is done.
        /// </summary>
        /// <param name="fields">Strings to draw in teh node view.</param>
        protected void DrawRows(params string[] fields)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                GUI.Label(NextRow, fields[i], OnGUIUtils.BlackTextStyle);
                NextRow.y += NextRow.height;
            }
        }
        
        protected virtual float GetBackgroundIconAlpha()
        {
            if (nodeTexture != null && graphEditor.visuals.DrawNodeIcons)
            {
                float scale = Mathf.InverseLerp(graphEditor.visuals.MinZoom, graphEditor.visuals.MaxZoom,
                    graphEditor.Zoom);
                scale = graphEditor.visuals.IconFadeCurve.Evaluate(scale);
                return scale;
            }
            return 0.0f;
        }

        /// <summary>
        /// Draw the node title in the title area.
        /// </summary>
        protected virtual void DrawTitle()
        {
            float alpha = 1.0f - GetBackgroundIconAlpha();
            if (alpha >= Mathf.Epsilon)
            {
                Color c = GUI.color;
                GUI.color = new Color(1.0f, 1.0f, 1.0f, alpha);
                GUI.Label(titleRect, displayName, OnGUIUtils.CenteredBlackTextStyle);
                GUI.color = c;
            }
        }
        #endregion -- Protected Methods ---------------------------------------

        #region -- Public Methods ---------------------------------------------
        /// <summary>Create a new NodeView.</summary>
        /// <param name="displayName">
        /// Name to display as the title of the node.
        /// </param>
        /// <param name="target">Object to represent.</param>
        public virtual void Initialize(string displayName, object target)
        {
            this.displayName = displayName;
            nodeTexture = NodeIconAttribute.GetNodeIcon(target.GetType());
            Target = target;
        }

        public virtual void OnWindowFocusChange(bool focus) { }

        public virtual void OnSceneHierarchyChange() { }

        public virtual void OnSelectionChange(Object[] objects) { }

        /// <summary>
        /// Draw the view at the specified location.
        /// </summary>
        /// <param name="rect">Where to draw.</param>
        public virtual void Draw(Rect rect)
        {
            if (DefaultColor != Color.clear)
                OnGUIUtils.DrawBox(rect, "", DefaultColor, Color.black);

            if (Event.current.type == EventType.Repaint)
                StoreRects(rect);

            DrawBackgroundIcon(rect);

            DrawTitle();
            NextRow = new Rect(bodyRect);
            NextRow.height = RowHeight;
        }

        protected virtual void DrawBackgroundIcon(Rect rect)
        {
            if (nodeTexture != null && graphEditor.visuals.DrawNodeIcons)
            {
                Texture t = nodeTexture;
                Rect iconRect = new Rect(rect);
                
                float border = (10 * graphEditor.Zoom) + 2;
                iconRect.xMin += border; iconRect.xMax -= border;
                iconRect.yMin += border; iconRect.yMax -= border;
                iconRect.width = t.width / (float)t.height * iconRect.height;
                iconRect.center = rect.center;

                Matrix4x4 m = GUI.matrix;

                //GUIUtility.ScaleAroundPivot(new Vector2(1.5f, 1.5f), iconRect.center);
                Color c = GUI.color;
                GUI.color = new Color(1.0f, 1.0f, 1.0f, GetBackgroundIconAlpha());
                GUI.DrawTexture(iconRect, t);
                GUI.matrix = m;
                GUI.color = c;
            }
        }

        /// <summary>
        /// Draw outputs of this node, if supported.
        /// </summary>
        /// <param name="children">Children to draw outputs for.</param>
        public virtual void DrawOutputs(int[] children)
        { }

        /// <summary>
        /// Get the label text for a connection between nodes.
        /// </summary>
        /// <param name="c">ID of the output to get a label for.</param>
        /// <returns>LAbel to display on the graph.</returns>
        public virtual string GetConnectionLabel(int c)
        { return DefaultConnectionLabel(c); }

        /// <summary>
        /// Default label for outputs is the index, but it may be overriden.
        /// </summary>
        /// <param name="c">Output index.</param>
        /// <returns>Default label for the connection.</returns>
        public static string DefaultConnectionLabel(int c)
        { return c.ToString(); }
        #endregion -- Public Methods ------------------------------------------
    }
}
