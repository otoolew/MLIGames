// -----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Max Golden
//
//  Created: 12/6/2016 10:37 AM
// -----------------------------------------------------------------------------

using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using SG.Core;

namespace SG.Vignettitor.NodeMaker
{
    /// <summary>
    /// Shell of the editor window to display the information contained in a NodeMakerView
    /// </summary>
    public class NodeMakerWindow : EditorWindow
    {
        private readonly Notify Log = NotifyManager.GetInstance<NodeMakerWindow>();

        // EditorPrefs keys
        const string SELECTED_VIEW_KEY = "NodeMaker_SelectedView";

        // Editor window variables
        private Vector2 scroll;
        private Type[] definedNodeMakerViews;
        private string[] definedNodeMakerViewNames;
        private NodeMakerView view;  // Currently instantiated view

        // SelectedNodeMakerView will save the current view index to EditorPres any time it's changed
        private int _selectedNodeMakerView;
        private int SelectedNodeMakerView
        {
            get { return _selectedNodeMakerView; }
            set
            {
                _selectedNodeMakerView = value;
                EditorPrefs.SetInt(SELECTED_VIEW_KEY, value);
            }
        }

        [MenuItem("Framework/Node Maker")]
        public static void OpenNodeMaker()
        {
            var newMakerWindow = GetWindow<NodeMakerWindow>("Node Maker");
            newMakerWindow.minSize = new Vector2(300, 225);
            newMakerWindow.Show();
        }

        /// <summary>
        /// Set up all of the window variables and instantiate the NodeMakerView to display
        /// </summary>
        /// <returns>True if initialization was successful, false otherwise</returns>
        public bool InitializeView()
        {
            // Get all NodeMakerView types in the project
            definedNodeMakerViews = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                     from assemblyType in domainAssembly.GetExportedTypes()
                                     where typeof(NodeMakerView).IsAssignableFrom(assemblyType)
                                     select assemblyType).ToArray();
            definedNodeMakerViewNames = definedNodeMakerViews.Select(v => v.Name).ToArray();

            if (definedNodeMakerViews.Length == 0)
            {
                // idk why this would ever happen, seeing as NodeMakerView dot is assignable from NodeMakerView
                Log.Error("Cannot find any exported types that extend NodeMakerView!");
                Close();
                return false;
            }

            // Figure out which view to display
            SelectedNodeMakerView = EditorPrefs.GetInt(SELECTED_VIEW_KEY, 0);
            if (SelectedNodeMakerView >= definedNodeMakerViews.Length)
                SelectedNodeMakerView = 0;

            view = (NodeMakerView)Activator.CreateInstance(definedNodeMakerViews[SelectedNodeMakerView]);
            view.CloseWindow += Close;
            view.Initialize();
            return true;
        }

        /// <summary>
        /// Call the draw methods and the view draw methods to render the window
        /// </summary>
        public void OnGUI()
        {
            if (definedNodeMakerViews == null || view == null)
            {
                if (!InitializeView())
                    return;
            }

            scroll = EditorGUILayout.BeginScrollView(scroll);

            DrawViewSelector();

            EditorGUIUtility.labelWidth = view.DefaultLabelWidth;

            view.DrawHeader();
            EditorGUILayout.Space();
            view.DrawBody();
            EditorGUILayout.Space();
            view.DrawButtons();

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// If there are multiple valid NodeMakerViews to choose from, draw the dropdown
        /// to allow the user to switch between them
        /// </summary>
        private void DrawViewSelector()
        {
            if (definedNodeMakerViews.Length > 1)
            {
                EditorGUI.BeginChangeCheck();
                SelectedNodeMakerView = EditorGUILayout.Popup(SelectedNodeMakerView, definedNodeMakerViewNames);
                if (EditorGUI.EndChangeCheck())
                {
                    view.CloseWindow -= Close;
                    view = (NodeMakerView)Activator.CreateInstance(definedNodeMakerViews[SelectedNodeMakerView]);
                    view.CloseWindow += Close;
                    view.Initialize();
                }
                EditorGUILayout.Space();
            }
        }
    }
}