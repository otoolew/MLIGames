//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   09/04/2014
//-----------------------------------------------------------------------------

using UnityEngine;

namespace SG.Vignettitor.Graph.States
{
    /// <summary>
    /// State of a graph editor when a connection between two nodes is 
    /// selected and may be edited.
    /// </summary>
    public class ConnectionEditState : GraphEditorState
    {
        #region -- Public Properties ------------------------------------------
        /// <summary> Node that the selected connection starts at. </summary>
        public int Node { get; protected set; }

        /// <summary> Child index that the connection goes to.  </summary>
        public int OutputIndex { get; protected set; }
        #endregion -- Public Properties ---------------------------------------

        /// <summary>
        /// Create a new state for editing connections between nodes.
        /// </summary>
        /// <param name="editor">Graph editor that may be modified.</param>
        /// <param name="node">
        /// ID of the node where the connection starts.
        /// </param>
        /// <param name="outputIndex">
        /// Child index of "node" that the connection goes to.
        /// </param>
        public ConnectionEditState(GraphEditor editor, int node, int outputIndex)
            : base(editor)
        {
            editor.SelectionManager.Clear();
            supportedCommands.Add("Delete");
            supportedCommands.Add("SoftDelete");

            Node = node;
            OutputIndex = outputIndex;
        }

        #region -- Input Overrides --------------------------------------------
        public override void OnDashMouseDown(Vector2 position)
        {
            base.OnDashMouseDown(position);
            if (Event.current.button == 0)
            {
                editor.EnterIdleState();
                editor.state.OnDashMouseDown(position);
            }
        }

        public override void OnNodeMouseDown(int id, Vector2 position)
        {
            base.OnNodeMouseDown(id, position);
            if (Event.current.button == 0)
            {
                editor.EnterIdleState();
                editor.state.OnNodeMouseDown(id, position);
            }
        }

        public override void CommandDraw()
        {
            base.CommandDraw();

            if (GUILayout.Button("Delete Connection") || MatchExecuteCommand("Delete") || MatchExecuteCommand("SoftDelete"))
            {
                editor.DeleteConnection(Node, OutputIndex);
                editor.state = new IdleState(editor);
            }
        }
        #endregion -- Input Overrides -----------------------------------------
    }
}
