//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   09/04/2014
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using SG.Core.OnGUI;
using UnityEngine;

namespace SG.Vignettitor.Graph.States
{
    /// <summary>
    /// Base class for an interaction state of a graph editor. Each state can 
    /// define how input is handled, what shortcuts can be used, and what 
    /// commands are available.
    /// </summary>
    public abstract class GraphEditorState
    {
        #region -- Protected Fields -------------------------------------------
        /// <summary>
        /// The last window command that was recieved (like "Paste" or 
        /// "Select All").
        /// </summary>
        protected string lastCommand = "";

        /// <summary> The graph editor that this sate can act on. </summary>
        protected GraphEditor editor;

        /// <summary>
        /// All window commands that this state cares about and can use.
        /// </summary>
        protected List<string> supportedCommands = new List<string>{"SelectAll", "Paste"};

        /// <summary> Position of mouse or touch input last frame. </summary>
        protected Vector2 lastInputPosition;

        /// <summary>
        /// A popup that can focus on a node by ID. This may be null.
        /// </summary>
        protected GoToNodePopup gotoPopup;

        protected bool panning;
        #endregion -- Protected Fields ----------------------------------------

        /// <summary>
        /// Construct a new graph editor state.
        /// </summary>
        /// <param name="editor">Graph editor to act upon.</param>
        protected GraphEditorState(GraphEditor editor)
        { this.editor = editor; }

        #region -- Window Commands --------------------------------------------
        /// <summary>
        /// Checks if a command was the last recieved command and clears it.
        /// </summary>
        /// <param name="input">Command to check on.</param>
        /// <returns>True if the command was the last one recieved.</returns>
        public bool MatchExecuteCommand(string input)
        {
            if (string.IsNullOrEmpty(lastCommand)) 
                return false;
            if (input == lastCommand)
            {
                lastCommand = string.Empty;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if this state can handle a certain command (like "Paste").
        /// </summary>
        /// <param name="command">Command to look for.</param>
        /// <returns>True if this state uses the command.</returns>
        public bool OnValidateCommand(string command)
        {
            return supportedCommands.Contains(command);
        }

        /// <summary>
        /// Records the command so that it may be processed next update.
        /// </summary>
        /// <param name="command">Window command that was executed.</param>
        public void OnExecuteCommand(string command)
        {
            lastCommand = command;
        }
        #endregion -- Window Commands -----------------------------------------

        public int DragButton
        { get { return (editor.ReadOnly ? 0 : 2); } }

        /// <summary>
        /// Defines any per frame operations that a state performs.
        /// </summary>
        public virtual void Update(){ }
        
        /// <summary>
        /// Handles a selection change when this is the current state.
        /// </summary>
        public virtual void OnSelectionChanged(){ }

        #region -- Drawing ----------------------------------------------------
        /// <summary>
        /// Defines any additional drawing within the graph that a state does.
        /// /// </summary>
        public virtual void Draw()
        {
            if (Event.current.rawType == EventType.MouseUp)
            {
                if (Event.current.button == DragButton)
                    panning = false;

                if (Event.current.button != 2)
                    if (!editor.ReadOnly)
                        editor.SelectionManager.EndMarqueeSelect(lastInputPosition);
            }
        }

        /// <summary>
        /// Defines any additional drawing to do in the command bar (generally 
        /// on the right side of the window.)
        /// </summary>
        public virtual void CommandDraw() { }

        /// <summary>
        /// Defines any drawing that should happen on top of the nodes and 
        /// connections in the graph.
        /// </summary>
        public virtual void OverlayDraw()
        {
            int border = (int)(editor.visuals.SelectionBorderWidth * editor.Zoom);
            RectOffset offset = new RectOffset(border, border, border, border);

            for (int i = 0; i < editor.SelectionManager.AllSelected.Count; i++)
                OnGUIUtils.DrawBox(offset.Add(editor.GetNodeRect(editor.SelectionManager.AllSelected[i])), "", editor.visuals.SelectedColor, Color.clear);

            for (int i = 0; i < editor.SelectionManager.PotentialSelections.Count; i++)
                OnGUIUtils.DrawBox(offset.Add(editor.GetNodeRect(editor.SelectionManager.PotentialSelections[i])), "", editor.visuals.MarqueeBorderColor, Color.clear);

            if (editor.SelectionManager.MarqueeActive)
                OnGUIUtils.DrawBox(editor.SelectionManager.MarqueeRect, "", editor.visuals.MarqueeColor, editor.visuals.MarqueeBorderColor);

            if (MatchExecuteCommand("SelectAll"))
                editor.SelectionManager.SelectAll();

            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.G && Event.current.control)
            {
                gotoPopup = new GoToNodePopup(editor.Container.center, editor);
            }

            if (gotoPopup != null)
                gotoPopup = gotoPopup.Draw() ? null : gotoPopup;

            if (panning)
                editor.DrawAdapter.AddCursorRect(new Rect(-int.MaxValue/2, -int.MaxValue/2, int.MaxValue, int.MaxValue), DrawAdapter.MouseCursor.Pan);
        }
        #endregion -- Drawing -------------------------------------------------

        #region -- Input Handling ---------------------------------------------      

        /// <summary> Handle a mouse (or touch) down within a node. </summary>
        /// <param name="id">ID of the node where it happened.</param>
        /// <param name="position">Position within the node. </param>
        public virtual void OnNodeMouseDown(int id, Vector2 position)
        {
            if (Event.current.button == 2)
            {
                Vector2 screenMousePos = editor.GraphToScreenPoint(
                    editor.ViewStates[id].Position + (position/editor.Zoom));
                lastInputPosition = screenMousePos;
            }            
            if (Event.current.button == DragButton)
                panning = true;
        }

        /// <summary> Handle a mouse (or touch) up within a node. </summary>
        /// <param name="id">ID of the node where it happened.</param>
        /// <param name="position">Position within the node. </param>
        public virtual void OnNodeMouseUp(int id, Vector2 position)
        {            
            if (Event.current.button == DragButton)
                panning = false;
        }

        /// <summary> Handle a mouse (or touch) move within a node. </summary>
        /// <param name="id">ID of the node where it happened.</param>
        /// <param name="position">New position within the node. </param>
        public virtual void OnNodeMouseMove(int id, Vector2 position)
        {
            if (Event.current.button == 2)
            {
                Vector2 screenMousePos = editor.GraphToScreenPoint(
                    editor.ViewStates[id].Position + (position / editor.Zoom));
                editor.Scroll += screenMousePos - lastInputPosition;
                lastInputPosition = screenMousePos;
            }
        }

        /// <summary> 
        /// Handle a mouse (or touch) move that occured outside of any nodes.
        ///  </summary>
        /// <param name="position">
        /// New position of the mouse in screen (window) coordinates.
        /// </param>
        public virtual void OnDashMouseMove(Vector2 position)
        {            
            if (Event.current.button == DragButton)
                editor.Scroll += position - lastInputPosition;
            else if (!editor.ReadOnly)
                editor.SelectionManager.Update(position);
            lastInputPosition = position;
        }

        /// <summary> 
        /// Handle a mouse (or touch) up that occured outside of any nodes.
        ///  </summary>
        /// <param name="position">
        /// Position of the mouse in screen (window) coordinates.
        /// </param>
        public virtual void OnDashMouseUp(Vector2 position)
        {
            if (Event.current.button != 2)
                if (!editor.ReadOnly)
                    editor.SelectionManager.EndMarqueeSelect(position);

            if (Event.current.button == DragButton)
                panning = false;
        }

        /// <summary> 
        /// Handle a mouse (or touch) down that occured outside of any nodes.
        ///  </summary>
        /// <param name="position">
        /// Position of the mouse in screen (window) coordinates.
        /// </param>
        public virtual void OnDashMouseDown(Vector2 position)
        {            
            if (Event.current.button == DragButton)
            {
                lastInputPosition = position;
                panning = true;
            }
            else if (!editor.ReadOnly)
                editor.SelectionManager.BeginMarqueeSelect(position, Event.current.shift);
        }
        #endregion -- Input Handling ------------------------------------------
    }
}
