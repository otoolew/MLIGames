//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   05/20/2014
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SG.Core.OnGUI;
using SG.Vignettitor.VignetteData;
using UnityEditor;
using UnityEngine;

namespace SG.Vignettitor.Editor.Search.CustomSearches
{
    /// <summary>
    /// A vignette node search that finds all nodes of a certain type in the 
    /// selected vignettes.
    /// </summary>
    public class NodeTypeSearch : CustomVignetteSearch
    {
        #region -- Private Fields ---------------------------------------------
        private List<string> typeNames;
        private List<Type> types;
        private bool inherit;
        private int nodeTypeSelectedIndex;
        private bool selectingType;
        private FilteredButtonListState listState;
        #endregion -- Private Fields ------------------------------------------

        #region -- Initialization ---------------------------------------------
        public NodeTypeSearch()
        {
            BuildTypeList();
        }

        /// <summary>
        /// Builds a list of all node types for selection in the search UI.
        /// </summary>
        private void BuildTypeList()
        {
            typeNames = new List<string>();
            types = new List<Type>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                Type[] vntypes = assemblies[i].GetTypes().Where(
                    type => type.IsSubclassOf(typeof(VignetteNode))
                    ).ToArray();
                for (int t = 0; t < vntypes.Length; t++)
                {
                    if (!vntypes[t].IsAbstract)
                    {
                        typeNames.Add(vntypes[t].Name);
                        types.Add(vntypes[t]);
                    }
                }
            }
        }
        #endregion -- Initialization ------------------------------------------

        #region -- Additional Drawing -----------------------------------------

        private void OnTypeSelected(int index)
        {
            nodeTypeSelectedIndex = index;
            selectingType = false;
            DisplayingPopup = false;
        }

        /// <summary>
        /// Draws a popup window for selecting the node type to search for.
        /// </summary>
        /// <param name="id">window ID.</param>
        private void SelectionWindow(int id)
        {
            if (GUILayout.Button("Cancel"))
            {
                selectingType = false;
                DisplayingPopup = false;
            }
            listState = OnGUIUtils.FilteredButtonList(listState, 
                typeNames.ToArray(), OnTypeSelected);
        }
        #endregion -- Additional Drawing --------------------------------------

        #region -- CustomVignetteSearch Overrides -----------------------------
        public override void PerformSearch(List<VignetteGraph> heads)
        {
            for (int i = 0; i < heads.Count; i++)
            {
                Type t = types[nodeTypeSelectedIndex];
                List<VignetteNode> result = heads[i].GetNodesOfType(t, inherit);

                for (int r = 0; r < result.Count; r++)
                    OnResultFound(new VignetteSearchResult(heads[i], result[r]));
            }
        }

        public override string GetDescription()
        {
            return "For all of the vignettes in the selected scope, this finds all usages of a selected node type.";
        }

        public override string GetName()
        { return "Node Type"; }

        public override void DrawPopups()
        {
            base.DrawPopups();
            if (selectingType)
            {
                GUI.enabled = true;
                bool enabled = GUI.enabled;
                window.BeginWindows();
                Rect r = new Rect(20, 20, window.position.width - 40, window.position.height * 0.66f);
                GUI.Window(1128123, r, SelectionWindow, "Select Node Type");
                window.EndWindows();
                GUI.enabled = enabled;
            }
        }

        public override void DrawSearch()
        {
            base.DrawSearch();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Node type to search for:");
            GUILayout.Label("   " + typeNames[nodeTypeSelectedIndex]);
            if (GUILayout.Button("Change"))
            {
                selectingType = true;
                DisplayingPopup = true;
            }
            inherit = GUILayout.Toggle(inherit, "Include Inheriting Types");
            GUILayout.EndHorizontal();
        }
        #endregion -- CustomVignetteSearch Overrides --------------------------
    }
}
