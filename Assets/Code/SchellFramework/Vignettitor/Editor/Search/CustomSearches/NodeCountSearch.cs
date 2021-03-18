//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   02/10/2015
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using SG.Vignettitor.VignetteData;
using UnityEditor;
using UnityEngine;

namespace SG.Vignettitor.Editor.Search.CustomSearches
{
    /// <summary>
    /// Gets a sorted list of the largest vignettes.
    /// </summary>
    public class NodeCountSearch : CustomVignetteSearch
    {
        private const int MIN_SIZE = 25;

        private static int CompareVignettes(VignetteSearchResult x, VignetteSearchResult y)
        {
            if (x.vignette.allNodes.Count == y.vignette.allNodes.Count)
                return 0;
            if (x.vignette.allNodes.Count > y.vignette.allNodes.Count)
                return -1;
            return 1;
        }

        #region -- CustomVignetteSearch Overrides -----------------------------
        public override void PerformSearch(List<VignetteGraph> heads)
        {
            for (int i = 0; i < heads.Count; i++)
                if (heads[i].allNodes.Count > MIN_SIZE)
                    OnResultFound(new VignetteSearchResult(heads[i], heads[i].allNodes[0]));
        }       

        public override void SearchComplete(List<VignetteSearchResult> results)
        {
            base.SearchComplete(results);
            searchResults.Sort(CompareVignettes);
        }

        public override string GetDescription()
        {
            return "Finds all vignettes larger than " + MIN_SIZE + 
                " nodes and lists them largest to smallest.";
        }

        public override string GetName()
        { return "Node Count"; }

        public override void DrawResult(VignetteSearchResult result)
        {
            if (GUILayout.Button(result.vignette.allNodes.Count + " " + result.vignette.name,
                NodeSearchWindow.resultStyle))
            {
                EditorGUIUtility.PingObject(result.vignette);
                Selection.activeObject = result.node;
            }
        }
        #endregion -- CustomVignetteSearch Overrides --------------------------
    }
}
