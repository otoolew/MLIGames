//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   09/04/2014
//-----------------------------------------------------------------------------

using SG.Core.OnGUI;
using UnityEngine;

namespace SG.Vignettitor.Graph.States
{
    /// <summary>
    /// State of a graph editor when the user is viewing content or relocating 
    /// nodes. Most meaningful interactions will transition to another state.
    /// </summary>
    public class IdleState : GraphEditorState
    {
        /// <summary> Used to track click or double click on nodes. </summary>
        private readonly ClickMonitor clickMonitor = new ClickMonitor(0.2f, 1.0f);

        public IdleState(GraphEditor renderer)
            : base(renderer)
        { }

        public override void CommandDraw()
        {
            base.CommandDraw();
            if (GUILayout.Button("New Annotation"))
            {
                Annotation note = editor.AnnotationManager.
                    CreateAnnotation(editor.GetFocalPoint());
                editor.AnnotationManager.BeginAnnotationEdit(note);
            }
        }

        #region -- Input Overrides --------------------------------------------
        public override void OnNodeMouseUp(int id, Vector2 position)
        {
            base.OnNodeMouseUp(id, position);
            // if a node is clicked once, select it.
            if (clickMonitor.Up(position) == 1)
            {
                editor.SelectionManager.Clear();
                editor.SelectionManager.AddToSelection(id);
            }
        }

        public override void OnNodeMouseDown(int id, Vector2 position)
        {
            base.OnNodeMouseDown(id, position);
            clickMonitor.Down(position);
        }
        #endregion -- Input Overrides -----------------------------------------
    }
}
