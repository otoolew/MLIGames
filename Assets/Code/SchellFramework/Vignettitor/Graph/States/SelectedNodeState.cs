//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   09/04/2014
//-----------------------------------------------------------------------------

using SG.Core.OnGUI;
using System.Collections.Generic;
using UnityEngine;

namespace SG.Vignettitor.Graph.States
{
    /// <summary>
    /// Graph editor state when one or more nodes are selected and there are 
    /// a few node related actions that may be performed.
    /// </summary>
    public class SelectedNodeState : GraphEditorState
    {
        /// <summary>
        /// How much time a second click must occur in after entering this
        ///  state to transition to edit mode.
        /// </summary>
        private const float MAX_EDIT_CLICK_DELAY = 0.4f;

        /// <summary> Tracks click or double click on nodes. </summary>
        protected ClickMonitor clickMonitor = new ClickMonitor(0.2f, 0.2f);

        /// <summary> Timestamp when this state was entered. </summary>
        protected float enterTime;

        /// <summary>
        /// Recorded positions of all of the selected nodes. This allows for 
        /// changing positions of multiple nodes at once.
        /// </summary>
        protected List<Vector2> actualPositions = new List<Vector2>();

        /// <summary>
        /// Create a new selected node state that performs actions on the 
        /// selections.
        /// </summary>
        /// <param name="renderer">Graph renderer to affect.</param>
        public SelectedNodeState(GraphEditor renderer)
            : base(renderer)
        {
            enterTime = Time.realtimeSinceStartup;
            supportedCommands.Add("Delete");
            supportedCommands.Add("SoftDelete");
            supportedCommands.Add("FrameSelected");
            GUI.UnfocusWindow();
        }

        /// <summary> Is there one and only one node selected? </summary>
        /// <param name="id">Check if the node is selected by itself.</param>
        /// <returns>True if the node is the only selection.</returns>
        private bool CheckForSoloSelection(int id)
        {
            return editor.SelectionManager.IsSelected(id) && 
                editor.SelectionManager.AllSelected.Count == 1;
        }

        /// <summary>
        /// Respond to a selection change by going back to idle if there are no 
        /// nodes or by updating the internal positions list.
        /// </summary>
        public override void OnSelectionChanged()
        {
            base.OnSelectionChanged();
            if (editor.SelectionManager.AllSelected.Count <= 0)
            {
                editor.EnterIdleState();
            }
            actualPositions.Clear();
            for (int i = 0; i < editor.SelectionManager.AllSelected.Count; i++)
            {
                if (editor.SelectionManager.AllSelected[i] < 0 ||
                    editor.SelectionManager.AllSelected[i] >= editor.ViewStates.Count)
                {
                    //Debug.Log("illegal state index " + editor.SelectionManager.AllSelected[i]);
                    editor.CleanSelection();
                }
                else
                {
                    actualPositions.Add(editor.ViewStates[editor.SelectionManager.AllSelected[i]].Position);
                }
            }
        }

        protected virtual void MoveSelection(Vector2 amount, bool forceSnap)
        {
            for (int i = 0; i < editor.SelectionManager.AllSelected.Count; i++)
            {
                int n = editor.SelectionManager.AllSelected[i];
                actualPositions[i] += amount;
                if (editor.visuals.SnapToGrid || forceSnap)
                {
                    editor.ViewStates[n].Position.x = Core.SgMath.RoundTo(actualPositions[i].x, editor.visuals.GridSpace);
                    editor.ViewStates[n].Position.y = Core.SgMath.RoundTo(actualPositions[i].y, editor.visuals.GridSpace);
                    actualPositions[i] = editor.ViewStates[n].Position;
                }
                else
                {
                    editor.ViewStates[n].Position.x = actualPositions[i].x;
                    editor.ViewStates[n].Position.y = actualPositions[i].y;
                }
            }
        }

        public override void Draw()
        {
            base.Draw();

            // Selected Node nudging with keyboard arrows.
            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.RightArrow)
                    MoveSelection(new Vector2(Event.current.shift ? editor.visuals.SmallNudgeDistance : editor.visuals.NormalNudgeDistance,0.0f), Event.current.control);
                else if (Event.current.keyCode == KeyCode.LeftArrow)
                    MoveSelection(new Vector2(Event.current.shift ? -editor.visuals.SmallNudgeDistance : -editor.visuals.NormalNudgeDistance, 0.0f), Event.current.control);
                else if (Event.current.keyCode == KeyCode.DownArrow)
                    MoveSelection(new Vector2(0.0f, Event.current.shift ? editor.visuals.SmallNudgeDistance : editor.visuals.NormalNudgeDistance), Event.current.control);
                else if (Event.current.keyCode == KeyCode.UpArrow)
                    MoveSelection(new Vector2(0.0f, Event.current.shift ? -editor.visuals.SmallNudgeDistance : -editor.visuals.NormalNudgeDistance), Event.current.control);
            }
        }

        public override void CommandDraw()
        {
            base.CommandDraw();

            if (GUILayout.Button("Delete Nodes") || MatchExecuteCommand("Delete") || MatchExecuteCommand("SoftDelete"))
            {
                editor.EndOfFrameActions += () =>
                {
                    editor.DeleteNode(editor.SelectionManager.AllSelected.ToArray());
                    editor.EnterIdleState();
                };
            }

            if (GUILayout.Button("Focus on Node") || MatchExecuteCommand("FrameSelected"))
            {
                editor.AnimatedFocusOnSelection();
            }
            if (GUILayout.Button("New Annotation"))
            {
                Annotation note = editor.AnnotationManager.
                    CreateAnnotation(editor.GetFocalPoint());
                Rect bounds = editor.GetBounds(editor.SelectionManager.AllSelected.ToArray());
                bounds = editor.visuals.BoundAnnotationDefaulOffset.Add(bounds);
                
                note.Position = bounds;
                editor.AnnotationManager.BeginAnnotationEdit(note);
                editor.AnnotationManager.BindOpenAnnotationToSelection();
            }
        }

        #region -- Input Overrides --------------------------------------------
        public override void OnDashMouseDown(Vector2 position)
        {
            base.OnDashMouseDown(position);

            // Do not change state if shift is selected since it may add new 
            // nodes to the selection.
            if (!Event.current.shift)
            {
                // Clicking on the dash (without shift) clears selection.
                if (Event.current.button == 0)
                {
                    editor.SelectionManager.Clear();
                    editor.state.OnDashMouseDown(position);
                }
            }
        }

        public override void OnNodeMouseDown(int id, Vector2 position)
        {
            base.OnNodeMouseDown(id, position);
            if (Event.current.button == 0)
            {
                if (Event.current.shift)
                {
                    // Shift adds new nodes to selection (or removes selected)
                    editor.SelectionManager.ToggleSelected(id);
                }
                else
                {
                    clickMonitor.Down(position);

                    // If a non-selected node is clicked, clear the selection. 
                    // The selection of the new node may occur in mouse up.
                    if (!editor.SelectionManager.IsSelected(id))
                    {
                        editor.SelectionManager.Clear();
                        editor.state.OnNodeMouseDown(id, position);
                    }
                }
            }
        }

        public override void OnNodeMouseMove(int id, Vector2 position)
        {
            base.OnNodeMouseMove(id, position);

            if (Event.current.button != 0) return;

            // If the user is dragging a selection, do not do anything
            if (editor.SelectionManager.MarqueeActive) return;

            // Adjust the positions of all selected nodes based on mouse 
            // movement and snapping configuration.
            for (int i = 0; i < editor.SelectionManager.AllSelected.Count; i++)
            {
                int n = editor.SelectionManager.AllSelected[i];
                actualPositions[i] += Event.current.delta / editor.Zoom;
                if (editor.visuals.SnapToGrid && id != n)
                {
                    editor.ViewStates[n].Position.x = SG.Core.SgMath.RoundTo(actualPositions[i].x, editor.visuals.GridSpace);
                    editor.ViewStates[n].Position.y = SG.Core.SgMath.RoundTo(actualPositions[i].y, editor.visuals.GridSpace);
                }
                else if (id != n)
                {
                    editor.ViewStates[n].Position.x = actualPositions[i].x;
                    editor.ViewStates[n].Position.y = actualPositions[i].y;
                }
            }
        }

        public override void OnNodeMouseUp(int id, Vector2 position)
        {
            base.OnNodeMouseUp(id, position);

            int clicks = clickMonitor.Up(position);

            // If the clicked node is not selected, select it.
            if (!editor.SelectionManager.IsSelected(id) && clicks == 1)
            {
                editor.SelectionManager.Clear();
                editor.SelectionManager.AddToSelection(id);
            }
            // If the selected node is clicked again right after entering the 
            // selected state, go right into the edit state.
            else if (Time.realtimeSinceStartup - enterTime <= MAX_EDIT_CLICK_DELAY && CheckForSoloSelection(id))
            {
                editor.EnterEditState();
            }
            // If it was a double click, enter edit mode.
            else if (clicks == 2)
            {
                editor.EnterEditState();
                OnDashMouseUp(position);
            }
        }
        #endregion -- Input Overrides -----------------------------------------
    }
}
