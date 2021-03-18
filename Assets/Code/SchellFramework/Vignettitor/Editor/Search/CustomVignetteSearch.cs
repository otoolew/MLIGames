//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   05/20/2014
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using SG.Vignettitor.VignetteData;
using UnityEditor;
using UnityEngine;

namespace SG.Vignettitor.Editor.Search
{
    /// <summary>
    /// Defines the interface for all custom searches that can be used with the 
    /// NodeSearchWindow that searches the game's vignettes based on different 
    /// criteria. This is where the custom search parameters and display is 
    /// defined.
    /// </summary>
    public abstract class CustomVignetteSearch
    {
        #region -- Public Fields ----------------------------------------------
        /// <summary>The window conducting this search.</summary>
        public NodeSearchWindow window;

        /// <summary>Is this search currently displaying a popup?</summary>
        public bool DisplayingPopup = false;
        #endregion -- Public Fields -------------------------------------------

        #region -- Protected Fields -------------------------------------------
        /// <summary>The vignette results of the last search.</summary>
        protected List<VignetteSearchResult> searchResults;
        #endregion -- Protected Fields ----------------------------------------
        
        #region -- Abstract Methods -------------------------------------------
        /// <summary>The user facing name of this search.</summary>
        public abstract string GetName();

        /// <summary>The user facing descrtiption of this search.</summary>
        public abstract string GetDescription();

        /// <summary>
        /// Conduct this search through all of the vignettes specified.
        /// </summary>
        /// <param name="heads">Vignettes to search over.</param>
        public abstract void PerformSearch(List<VignetteGraph> heads);
        #endregion -- Abstract Methods ----------------------------------------

        #region -- Virtual Methods --------------------------------------------
        /// <summary>
        /// Adds a result to the search windows listing.
        /// </summary>
        /// <param name="result">Result to add.</param>
        protected virtual void OnResultFound(VignetteSearchResult result)
        {
            window.searchResults.Add(result);
        }

        /// <summary>
        /// Called when a search is complete, this builds up the result list in 
        /// preparation to be displayed.
        /// </summary>
        /// <param name="results">Vignettes/nodes found in the search</param>
        public virtual void SearchComplete(List<VignetteSearchResult> results)
        {
            searchResults = results;
        }

        /// <summary>
        /// Draw this custom search interface. This is where specific 
        /// parameters can be specified by the user.
        /// </summary>
        public virtual void DrawSearch() {}

        /// <summary>Draw the results of the last search.</summary>
        public virtual void DrawResults()
        {
            if (searchResults != null)
            {
                GUILayout.Label("Search Results : " + searchResults.Count + " results found.");
                for (int i = 0; i < searchResults.Count; i++)
                {
                    if (searchResults[i] != null && searchResults[i].node != null && searchResults[i].vignette != null)
                    {
                        DrawResult(searchResults[i]);
                    }
                }
            }
        }

        public virtual void DrawResult(VignetteSearchResult result)
        {
            if (GUILayout.Button(result.vignette.name + " - " + result.node.name, NodeSearchWindow.resultStyle))
            {
                EditorGUIUtility.PingObject(result.vignette);
                Selection.activeObject = result.node;
            }
        }

        /// <summary>
        /// Draw any UI that should appear overtop of all others.
        /// </summary>
        public virtual void DrawPopups()
        { }
        #endregion -- Virtual Methods -----------------------------------------
    }
}
