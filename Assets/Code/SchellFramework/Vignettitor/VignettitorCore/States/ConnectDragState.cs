//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   09/04/2014
//-----------------------------------------------------------------------------

using SG.Core.OnGUI;
using SG.Vignettitor.Graph;
using SG.Vignettitor.Graph.NodeViews;
using SG.Vignettitor.Graph.States;
using UnityEngine;

namespace SG.Vignettitor.VignettitorCore.States
{
    /// <summary>
    /// State of a graph editor when a user has performed a mouse down on a 
    /// node output pin and is in the process of drawing a connector to an 
    /// existing node or an empty space to create a new node.
    /// </summary>
    public class ConnectDragState : GraphEditorState
    {
        /// <summary>
        /// When dragging this close to the border of the view, it will pan.
        /// </summary>
        public const int AUTO_PAN_BUFFER = 30;

        #region -- Protected Fields -------------------------------------------
        /// <summary>
        /// ID of the node where the connection drag started.
        /// </summary>
        protected int source;

        /// <summary>
        /// Output index of the source node that the connection starts at.
        /// </summary>
        protected int outputIndex;
        
        /// <summary>
        /// The last recorded position of the mouse cursor on the graph.
        /// </summary>
        protected Vector2 lastPosition;
        #endregion -- Protected Fields ----------------------------------------

        /// <summary>
        /// Create a new ConenctDragState to connect nodes or create a new one.
        /// </summary>
        /// <param name="editor">Vignettitor this state can access.</param>
        /// <param name="source">
        /// ID of the node where the connection drag started.
        /// </param>
        /// <param name="outputIndex">
        /// Output index of the source node that the connection starts at.
        /// </param>
        public ConnectDragState(Vignettitor editor, int source, int outputIndex, Vector2 pos)
            : base(editor)
        {
            editor.SelectionManager.Clear();
	        this.source = source;
	        this.outputIndex = outputIndex;
            lastPosition = pos;
        }

        public override void OnDashMouseMove(Vector2 position)
        {
            base.OnDashMouseMove(position);
            lastPosition = position;            
        }

        public override void OnNodeMouseDown(int id, Vector2 position)
        {
            base.OnNodeMouseDown(id, position);
            if (Event.current.button == 1)
                editor.EnterIdleState();
        }

        public override void OnDashMouseDown(Vector2 position)
        {
            base.OnDashMouseDown(position);
            if (Event.current.button == 1)
                editor.EnterIdleState();
        }

        public override void OnNodeMouseUp(int id, Vector2 position)
        {
            base.OnNodeMouseUp(id, position);
            if (Event.current.button == 0)
            {
                if (id == source)
                {
                    int outputCount = editor.ViewStates[id].nodeView.Outputs.Length;
                    if (outputCount >= outputIndex)
                    {
                        NodeView.NodeViewOutput output = editor.ViewStates[id].nodeView.Outputs[outputIndex];
                        if (output.Position.Contains(position))
                        {
                            editor.EnterConnectionEditState(id, outputIndex);
                            Event.current.Use();
                        }
                    }
                }
                else
                {
                    editor.ConnectNodes(source, outputIndex, id);
                    Event.current.Use();
                    editor.EnterIdleState();
                }
                Event.current.Use();
            }
        }

        public override void OnDashMouseUp(Vector2 position)
        {
            base.OnDashMouseUp(position);
            if (Event.current.button == 0)
            {
                Vignettitor vignettitor = editor as Vignettitor;
                if (vignettitor != null) vignettitor.EnterCreateNodeState(source, outputIndex, position);
            }
        }

        public override void Draw()
        {
            if (Event.current.rawType == EventType.MouseUp)
            {
                if (Event.current.button == 0)
                    OnDashMouseUp(lastInputPosition);
            }

            Rect display = editor.Container;
            display.xMax -= GraphEditor.COMMAND_WIDTH;

            if (lastPosition.x > display.xMax - AUTO_PAN_BUFFER)
                editor.Scroll -= new Vector2(1, 0);
            if (lastPosition.x < display.xMin + AUTO_PAN_BUFFER)
                editor.Scroll += new Vector2(1, 0);
            if (lastPosition.y > display.yMax - AUTO_PAN_BUFFER)
                editor.Scroll -= new Vector2(0, 1);
            if (lastPosition.y < display.yMin + AUTO_PAN_BUFFER)
                editor.Scroll += new Vector2(0, 1);
        }

        public override void OverlayDraw()
        {
            base.OverlayDraw();
            if (lastPosition != Vector2.zero)
                OnGUIUtils.DrawLine(editor.GetNodeExit(source, outputIndex), lastPosition, editor.visuals.NewConnectionColor);
        }
    }
}
