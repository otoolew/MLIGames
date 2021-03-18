//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   09/04/2014
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using SG.Vignettitor.Graph;
using SG.Vignettitor.VignettitorCore;
using UnityEditor;
using UnityEngine;

namespace SG.Vignettitor.Editor
{
    /// <summary>
    /// A class for testing a basic graph editor window.
    /// </summary>
    public class TestGraph : GraphEditor
    {
        public TestGraph(VignettitorDataController dataController)
        {
            graphViewState = new GraphViewState();
            PopulateTestData();
            Initialize();
        }

        private void PopulateTestData()
        {
            graphViewState.NodeViewStates = new List<NodeViewState>();
            NodeViewState a = new NodeViewState();
            NodeViewState b1 = new NodeViewState();
            NodeViewState b2 = new NodeViewState();
            NodeViewState c = new NodeViewState();
            NodeViewState d = new NodeViewState();

            graphViewState.NodeViewStates.Add(a);
            graphViewState.NodeViewStates.Add(b1);
            graphViewState.NodeViewStates.Add(b2);
            graphViewState.NodeViewStates.Add(c);
            graphViewState.NodeViewStates.Add(d);
        }

        protected override void DrawDefaultNode(int id, Rect r)
        {
            GUILayout.Label(id.ToString());
        }

        public override int[] GetChildren(int id)
        {
            if (id == 0) return new int[] { 1, 2 };
            if (id == 1) return new int[] { 3 };
            if (id == 2) return new int[] { 3 };
            if (id == 3) return new int[] { 4 };
            if (id == 4) return new int[] { };
            return new int[0];
        }
    }
    public class TestGraphView : EditorWindow
    {
        private TestGraph graphRenderer;

        //TODO: OLD MENU
        private static void TestGraph()
        {
            GetWindow(typeof(TestGraphView));
        }

        void OnEnable()
        {
            graphRenderer = new TestGraph(new EditorVignettitorDataController());
            graphRenderer.DrawAdapter = new EditorDrawing();

            titleContent = new GUIContent("Test Graph");
            ShowNotification(new GUIContent("Welcome to The Test Graph"));
        }

        private void Update()
        {
            Repaint();
            graphRenderer.Update();
        }

        private void OnGUI()
        {
            try
            {
                BeginWindows();
                wantsMouseMove = true;
                Rect p = new Rect(0, 0, position.width, position.height);
                graphRenderer.Draw(p);
                EndWindows();
                if (Event.current.type == EventType.MouseMove ||
                    Event.current.type == EventType.MouseDrag ||
                    Event.current.type == EventType.MouseUp ||
                    Event.current.type == EventType.MouseDown)
                    Repaint();

                if (graphRenderer.visuals.LinesOnTop)
                    graphRenderer.DrawOverlay();
            }
            catch (System.Exception e)
            {
                this.Close();
                throw e;
            }
        }
    }
}
