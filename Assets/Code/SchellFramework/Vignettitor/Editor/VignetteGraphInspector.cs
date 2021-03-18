//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   08/19/2015
//-----------------------------------------------------------------------------

using SG.Core.OnGUI;
using SG.Vignettitor.VignetteData;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace SG.Vignettitor.Editor
{
    /// <summary>
    /// A Unity editor for VignetteGraph types that will draw fields and 
    /// display a button that opens the graph in the vignettitor window that is
    /// meant for graphs of this Type.
    /// </summary>
    [CustomEditor(typeof(VignetteGraph), true)]
    public class VignetteGraphInspector : UnityEditor.Editor
    {
        #region SubAsset Visibility -------------------------------------------
        [MenuItem("CONTEXT/VignetteGraph/Hide All Sub-assets")]
        private static void HideAllSubAssets(MenuCommand c)
        {
            VignetteGraph graph = AssetDatabase.LoadAssetAtPath<VignetteGraph>(AssetDatabase.GetAssetPath(c.context));
            EditorVignettitorDataController.ChangeGraphVisibility(graph, HideFlags.HideInHierarchy);
        }

        [MenuItem("CONTEXT/VignetteGraph/Show All Sub-assets")]
        private static void ShowAllSubAssets(MenuCommand c)
        {
            VignetteGraph graph = AssetDatabase.LoadAssetAtPath<VignetteGraph>(AssetDatabase.GetAssetPath(c.context));
            EditorVignettitorDataController.ChangeGraphVisibility(graph, HideFlags.None);
        }
        #endregion SubAsset Visibility ----------------------------------------

        /// <summary>
        /// Draw a basic vignette graph editor.
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorOnGUILayout.DrawPropertiesExcluding(serializedObject, "m_Script", "allNodes", "Entry", "SaveFormatVersion");
            serializedObject.ApplyModifiedProperties();
            bool wasEnabled = GUI.enabled;
            GUI.enabled = true;
            if (GUILayout.Button("Edit " + target.GetType()))
            {
                OpenGraph(target as VignetteGraph);
            }
            GUI.enabled = false;
        }

        /// <summary>
        /// Handler for the OpenAsset callback from double-clicking or pressing enter
        /// </summary>
        /// <returns>True if the graph was opened in a window and false otherwise.</returns>
        [OnOpenAsset]
        public static bool HandleOpenAssetCallback(int instanceID, int line)
        {
            VignetteGraph graph = EditorUtility.InstanceIDToObject(instanceID) as VignetteGraph;

            if (graph != null)
            {
                OpenGraph(graph);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Opens the specified graph in a proper vignette window
        /// </summary>
        /// <param name="graph">Graph to open</param>
        static void OpenGraph(VignetteGraph graph)
        {
            VignettitorWindow window = VignettitorWindowAttribute.GetVignettitorWindow(graph);
            window.SetVignette(graph);
        }
    }
}
