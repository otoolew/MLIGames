//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   03/05/2015
//-----------------------------------------------------------------------------

using SG.Core.OnGUI;
using SG.Vignettitor.Graph;
using SG.Vignettitor.Graph.States;
using SG.Vignettitor.NodeViews;
using SG.Vignettitor.VignetteData;
using SG.Vignettitor.VignettitorCore;
using SG.Vignettitor.VignettitorCore.States;
using UnityEngine;

namespace SG.Vignettitor.Runtime
{
    /// <summary>
    /// The RuntimeVignettitor class is a vignettitor that can allow for 
    /// viewing a graph but not editing. The interface is simplified so that 
    /// it will work with touch devices so that it can be run inside of a 
    /// build.
    /// </summary>
    public class RuntimeVignettitor : VignettitorCore.Vignettitor
    {
        #region -- Public Fields ----------------------------------------------
        /// <summary>Function to call when close button is clicked.</summary>
        public System.Action CloseCallback = null;
        #endregion -- Public Fields -------------------------------------------

        #region -- Private Fields ---------------------------------------------
        /// <summary>
        /// Listens for down, up and double click events.
        /// </summary>
        private readonly ClickMonitor clickMonitor = new ClickMonitor(0.4f, 3.0f);
        #endregion -- Private Fields ------------------------------------------

        #region -- Initialization ---------------------------------------------
        /// <summary>Create a new editor to view the given vignette.</summary>
        /// <param name="head">Head of the vigentte to edit.</param>
        /// <param name="dataController">
        /// Defines how to read and write data.
        /// </param>
        public RuntimeVignettitor(VignetteGraph head, VignettitorDataController dataController, DrawAdapter drawAdapter) 
            : base(head, dataController, drawAdapter)
        {
            // prevent constructor from being stripped.
            #pragma warning disable 219
            VignetteNodeView nv = new VignetteNodeView();
            #pragma warning restore 219
        }
        #endregion -- Initialization ------------------------------------------

        #region -- Modification Prevention ------------------------------------
        /// <summary> Do not draw the grid. </summary>
        protected override void DrawGrid()
        { }

        /// <summary> Prevents the new node state. </summary>
        /// <param name="source"></param>
        /// <param name="output"></param>
        /// <param name="position"></param>
        public override void EnterCreateNodeState(int source, int output, Vector2 position)
        { }

        /// <summary> Prevents the edit state. </summary>
        public override void EnterEditState()
        { }
        #endregion -- Modification Prevention ---------------------------------

        public override void Update()
        {
            base.Update();

            if (Input.touchCount == 2)
            {
                Touch first = Input.GetTouch(0);
                Touch second = Input.GetTouch(1);
                float lastDelta = ((first.position - first.deltaPosition) - 
                    (second.position - second.deltaPosition)).magnitude;
                Vector2 offset = second.position - first.position;
                float delta = lastDelta - offset.magnitude;
                Vector2 center = first.position + offset.normalized * offset.magnitude * 0.5f;
                AdjustZoom(delta * 0.1f, center);
            }
        }

        protected override void OnNodeMouseUp(int id, Vector2 position)
        {
            base.OnNodeMouseUp(id, position);

            Vector2 screenMousePos = GraphToScreenPoint(
                ViewStates[id].Position + (Event.current.mousePosition / Zoom));

            if (clickMonitor.Up(screenMousePos) == 1)            
            {
                if (!(state is SelectedNodeState))
                    EnterSelectedState();
                SelectionManager.Clear();
                SelectionManager.AddToSelection(id);
            }
        }

        protected override void OnNodeMouseDown(int id, Vector2 position)
        {
            base.OnNodeMouseDown(id, position);
            Vector2 screenMousePos = GraphToScreenPoint(
                ViewStates[id].Position + (Event.current.mousePosition / Zoom));
            clickMonitor.Down(screenMousePos);
        }

        #region -- Drawing ----------------------------------------------------
        public override void DrawOverlay()
        {
            base.DrawOverlay();
            Rect r = new Rect(Screen.width * 0.88f, 60, Screen.width * 0.12f, Screen.height - 120);
            GUI.Window(87033, r, DrawControls, "");
            GUI.BringWindowToFront(87033);
            if (Event.current.isMouse && r.Contains(Event.current.mousePosition))
                Event.current.Use();

            Rect z = new Rect(0.0f, 60, Screen.width * 0.05f, Screen.height - 120);
            GUI.Window(23432, z, DrawZoom, "Zoom");
            GUI.BringWindowToFront(23432);
            if (Event.current.isMouse && r.Contains(Event.current.mousePosition))
                Event.current.Use();
        }

        protected void DrawZoom(int id)
        {
            Vector2 fp = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
            Vector2 before = ScreenToGraphPoint(fp);
            Zoom = GUILayout.VerticalSlider(Zoom, visuals.MinZoom, visuals.MaxZoom, GUILayout.Height(Screen.height * 0.7f));
            Vector2 after = ScreenToGraphPoint(fp);
            FocusOnPoint(GetFocalPoint() - (after - before));
        }

        protected void DrawControls(int id)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            bool lastEnabled = GUI.enabled;
            if (GUILayout.Button("Close"))
            {
                if (CloseCallback != null)
                    CloseCallback();
            }

            if (GUILayout.Button("Clear Hardwire"))
            {
                ClearHardwire();
            }

            GUI.enabled = SelectionManager.AllSelected.Count > 0;
            if (GUILayout.Button("Hardwire Path"))
            {
                HardwirePath(allNodes[SelectionManager.AllSelected[0]]);
            }

            GUI.enabled = state is VignetteConnectionEditState;
            VignetteConnectionEditState s = state as VignetteConnectionEditState;
            if (GUILayout.Button("Toggle Hardwire"))
            {
                ToggleHardwire(s.Node, s.OutputIndex);
            }
            GUI.enabled = lastEnabled;

            if (GUILayout.Button("Skip To Node"))
            {
                SkipToNode(allNodes[SelectionManager.AllSelected[0]]);
            }

            if (GUILayout.Button("Reset Pan / Zoom"))
            {
                Scroll = new Vector2();
                Zoom = 1.0f;
            }

            GUILayout.EndVertical();
        }
        #endregion -- Drawing -------------------------------------------------
    }
}
