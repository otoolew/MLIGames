//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   10/03/2014
//-----------------------------------------------------------------------------

using UnityEngine;

namespace SG.Vignettitor.Graph.States
{
    /// <summary>
    /// State in use whan a graph annotation is being edited. This forwards 
    /// relevent inputs to the annotation manager and manages returning to 
    /// the idle state.
    /// </summary>
    public class EditAnnotationState : GraphEditorState
    {
        public EditAnnotationState(GraphEditor editor)
            : base(editor)
        { }

        #region -- Input Handling ---------------------------------------------
        public override void OnDashMouseDown(Vector2 position)
        {
            base.OnDashMouseDown(position);
            if (Event.current.button == 0)
            {
                editor.AnnotationManager.EndAnnotationEdit();
                editor.EnterIdleState();
                editor.state.OnDashMouseDown(position);
            }
        }

        public override void OnNodeMouseDown(int id, Vector2 position)
        {
            base.OnNodeMouseDown(id, position);
            if (Event.current.button == 0)
            {
                editor.AnnotationManager.EndAnnotationEdit();
                editor.EnterIdleState();
                editor.state.OnDashMouseDown(position);
            }
        }

        public override void OnDashMouseMove(Vector2 position)
        {
            base.OnDashMouseMove(position);
            if (Event.current.type == EventType.MouseDrag)
                editor.AnnotationManager.OnAnnotationScale();
        }

        public override void OnDashMouseUp(Vector2 position)
        {
            base.OnDashMouseUp(position);
            editor.AnnotationManager.CancelAnnotationScale();
        }      

        public override void OnNodeMouseUp(int id, Vector2 position)
        {
            base.OnNodeMouseUp(id, position);
            editor.AnnotationManager.CancelAnnotationScale();
        }
        #endregion -- Input Handling ------------------------------------------
    }
}
