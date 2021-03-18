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
    /// State where a node may be edited directly in the graph if there is an 
    /// available NodeEditView for the node type.
    /// </summary>
    public class EditNodeState : GraphEditorState
    {
        /// <summary> Node Selection Manager for active editor. </summary>
        protected NodeSelectionManager selection;
        
        /// <summary>
        /// Create an edit node state thatcan display a node edit view for a 
        /// selection if available.
        /// </summary>
        /// <param name="editor">Graph Editor to act on.</param>
        public EditNodeState(GraphEditor editor)
            : base(editor)
        {
            selection = editor.SelectionManager;
            supportedCommands.Add("Delete");
            supportedCommands.Add("SoftDelete");
            supportedCommands.Add("FrameSelected");
            supportedCommands.Add("SelectAll");
        }

        public override void CommandDraw()
        {
            base.CommandDraw();

            if (GUILayout.Button("Focus on Node") || MatchExecuteCommand("FrameSelected"))
            {
                editor.AnimatedFocusOnSelection();
            }
        }

        public override void OnSelectionChanged()
        {
            base.OnSelectionChanged();
            if (editor.SelectionManager.AllSelected.Count <= 0)
            {
                editor.EnterIdleState();
            }
        }

        #region -- Input Overrides --------------------------------------------
        public override void OnDashMouseDown(Vector2 position)
        {
            base.OnDashMouseDown(position);
           // If a left click happens outside of the node, go back to idle.
            if (Event.current.button == 0)
            {
                editor.ResetFocus();
                editor.SelectionManager.Clear();
                editor.state.OnDashMouseDown(position);
            }
        }

        public override void OnNodeMouseDown(int id, Vector2 position)
        {
            base.OnNodeMouseDown(id, position);            
            
            // If a left click happens on another node, go to idle.
            if (Event.current.button == 0)
            {
                if (!selection.AllSelected.Contains(id))
                {
                    editor.ResetFocus();
                    editor.SelectionManager.Clear();
                    editor.state.OnNodeMouseDown(id, position);
                }
            }
        }

        public override void OnNodeMouseUp(int id, Vector2 position)
        {
            base.OnNodeMouseUp(id, position);
            OnDashMouseUp(position);            
        }
        #endregion -- Input Overrides -----------------------------------------
    }
}
