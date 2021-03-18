//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   09/04/2014
//-----------------------------------------------------------------------------

using System;
using UnityEngine;

namespace SG.Vignettitor.Graph.States
{
    /// <summary>
    /// Pans the view to focus on an area of the graph and zooms out if necessary 
    /// to fit the area. This state will automatically exit, returning to the 
    /// Return state, once the focus animation completes.
    /// </summary>
    public class AnimatePanState : GraphEditorState
    {
        #region -- Constants --------------------------------------------------
        /// <summary>
        /// Duration of the pan and zoom transition to the focus.
        /// </summary>
        private const float ANIM_DURATION = 0.3f;

        /// <summary>
        /// Percent extra to zoom out to keep buffer on the edge of the screen.
        /// </summary>
        private const float ZOOM_BUFFER = 0.08f;

        /// <summary> Used to check if ranges are nearly equal. </summary>
        private const float EPSILON = 0.00001f;
        #endregion -- Constants -----------------------------------------------

        #region -- Private Variables ------------------------------------------
        /// <summary>State to return to once this is completed.</summary>
        private readonly GraphEditorState returnState;

        /// <summary>Focal point when the animation started.</summary>
        private readonly Vector2 startFocus;

        /// <summary>Focal point to end on.</summary>
        private readonly Vector2 targetFocus;

        /// <summary>Zoom when the animation started.</summary>
        private readonly float startZoom;

        /// <summary>Zoom to end on.</summary>
        private readonly float targetZoom;

        /// <summary>Last timeStamp of the update call.</summary>
        private float lastTime;

        /// <summary>Total time tha animation has been playing.</summary>
        private float totalTime;
        #endregion -- Private Variables ---------------------------------------

        /// <summary>
        /// Triggers a state to focus a graph editor on a given rectangle.
        /// </summary>
        /// <param name="editor">The graph editor to operate on.</param>
        /// <param name="focus">
        /// The rectangle that is the new target to fit on the screen.
        /// </param>
        /// <param name="returnState">
        /// State to transition to once the focus animation is complete.
        /// </param>
        public AnimatePanState(GraphEditor editor, Rect focus, GraphEditorState returnState)
            : base(editor)
        {
            this.returnState = returnState;
            startZoom = editor.Zoom;
            targetZoom = startZoom;

            Rect displayedRect = editor.GetDisplayedRect(false);
            float comWidth = editor.GetCommandRect().width;

            startFocus = editor.ScreenToGraphPoint( new Vector2(
                    (editor.Container.width - comWidth) / 2.0f,
                    editor.Container.height / 2.0f));

            // If the displayed width is less than the goal width
            if (displayedRect.width < focus.width)
            {
                targetZoom = Mathf.Min(targetZoom,
                    (editor.Container.width - comWidth) / focus.width);
            }
            if (displayedRect.height < focus.height)
            {
                targetZoom = Mathf.Min(targetZoom,
                    editor.Container.height / focus.height);
            }
            if (Math.Abs(targetZoom - startZoom) > EPSILON)
            {
                // Leave some buffer on the sides of the destination view.
                targetZoom -= targetZoom * ZOOM_BUFFER;
            }

            targetFocus = focus.center;

            totalTime = 0;
            lastTime = Time.realtimeSinceStartup;
        }

        public override void Draw()
        {
            base.Draw();
            editor.DrawAdapter.AddCursorRect(new Rect(-int.MaxValue / 2, -int.MaxValue / 2, int.MaxValue, int.MaxValue), DrawAdapter.MouseCursor.Orbit);
        }

        /// <summary>
        /// Update viewport to animate towards the target position and zoom.
        /// </summary>
        public override void Update()
        {
            float time = Time.realtimeSinceStartup;
            float dt = time - lastTime;

            Vector2 path = targetFocus - startFocus;
            float mag = path.magnitude;
            Vector2 norm = path.normalized;
            float perc = totalTime / ANIM_DURATION;
            perc = Mathf.SmoothStep(0.0f, 1.0f, perc);
            editor.FocusOnPoint(startFocus + norm * mag * (perc), false);
            editor.Zoom = startZoom - (startZoom - targetZoom) * perc;

            totalTime += dt;
            lastTime = time;

            if (totalTime >= ANIM_DURATION)
            {
                editor.Zoom = targetZoom;
                editor.FocusOnPoint(targetFocus, false);
                if (returnState == null)
                    editor.EnterIdleState();
                else
                    editor.EnterState(returnState);
            }
        }

        #region -- Ignored Inputs ---------------------------------------------
        public override void OnDashMouseDown(Vector2 position) { }
        public override void OnDashMouseMove(Vector2 position) { }
        public override void OnDashMouseUp(Vector2 position) { }
        public override void OnNodeMouseDown(int id, Vector2 position) { }
        public override void OnNodeMouseMove(int id, Vector2 position) { }
        public override void OnNodeMouseUp(int id, Vector2 position) { }
        #endregion -- Ignored Inputs ------------------------------------------
    }
}
