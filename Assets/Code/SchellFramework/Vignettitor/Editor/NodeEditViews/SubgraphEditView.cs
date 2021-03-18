// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Eric Policaro
// 
//  Date: 02/22/2016
// ----------------------------------------------------------------------------

using SG.Vignettitor.Graph.NodeViews;
using SG.Vignettitor.Nodes;
using SG.Vignettitor.NodeViews;
using SG.Vignettitor.VignetteData;
using UnityEditor;
using UnityEngine;

namespace SG.Vignettitor.Editor.NodeEditViews
{
    [NodeEditView(typeof(SubgraphNode))]
    public class SubgraphEditView : VignetteNodeEditView
    {
        private SerializedObject _serializedObjectNode;
        private SerializedProperty _subgraphValue;

        public override void Draw(Rect rect)
        {
            base.Draw(rect);

            if (_serializedObjectNode == null)
            {
                _serializedObjectNode = new SerializedObject(Node);
                _subgraphValue = _serializedObjectNode.FindProperty("Graph");
            }

            _serializedObjectNode.Update();

            if (EditorGUILayout.PropertyField(_subgraphValue, false))
                _serializedObjectNode.ApplyModifiedProperties();

            EditorGUI.BeginDisabledGroup(!_subgraphValue.objectReferenceValue);
            if (GUILayout.Button("View", GUILayout.Width(bodyRect.width)))
            {
                VignetteGraph graph = _subgraphValue.objectReferenceValue as VignetteGraph;
                VignettitorWindow window = VignettitorWindowAttribute.
                    GetVignettitorWindow(graph);
                if (window != null)
                {
                    VignetteGraph previous = window.GetActiveGraph();
                    window.SetVignette(graph);
                    window.AddBreadcrumb(previous);
                }
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}