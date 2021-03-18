//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   05/20/2014
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SG.Vignettitor.VignetteData;
using UnityEditor;
using UnityEngine;

namespace SG.Vignettitor.Editor.Search
{
    /// <summary>
    /// Provides an interface to search through the vignettes of the game by 
    /// using a custom search type.
    /// </summary>
    public class NodeSearchWindow : EditorWindow
    {
        #region -- Static Fields ----------------------------------------------
        /// <summary>Default style for drawing a search result.</summary>
        public static GUIStyle resultStyle;
        #endregion -- Static Fields -------------------------------------------

        #region -- Public Fields ----------------------------------------------
        public List<VignetteSearchResult> searchResults = new List<VignetteSearchResult>();
        #endregion -- Public Fields -------------------------------------------

        #region -- Protected Fields -------------------------------------------
        protected List<VignetteGraph> allVignettes = new List<VignetteGraph>();

        protected List<CustomVignetteSearch> searchTypes = new List<CustomVignetteSearch>();
        protected List<string> searchTypeNames = new List<string>();
        protected int selectedType;
        protected CustomVignetteSearch customSearch;

        protected Vector2 resultScroll;
        #endregion -- Protected Fields ----------------------------------------

        #region -- Initialization ---------------------------------------------
        /// <summary> Open the window. </summary>
        public static NodeSearchWindow Open()
        {
            NodeSearchWindow window = (NodeSearchWindow)GetWindow(
                typeof(NodeSearchWindow));
            window.Initialize();
            window.FindVignettesInScope();
            return window;
        }

        public virtual void SetScope(VignetteGraph[] graphs)
        {
            allVignettes.Clear();
            allVignettes.AddRange(graphs);
        }

        /// <summary>
        /// Set up the search scopes and available search types.
        /// </summary>
        protected virtual void Initialize()
        {
            titleContent = new GUIContent("Vignette Search");
            RefreshSearchTypes();
        }

        /// <summary>
        /// Finds all of the available custom searches in the currently loaded 
        /// assembly and puts them in the searchTypes list and their names in 
        /// the searchTypeNames list.
        /// </summary>
        private void RefreshSearchTypes()
        {
            searchTypes = new List<CustomVignetteSearch>();
            searchTypeNames = new List<string>();
            searchTypes.Add(null);
            searchTypeNames.Add("None");
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                Type[] types = assemblies[i].GetTypes().Where(
                    type => type.IsSubclassOf(typeof(CustomVignetteSearch))
                    ).ToArray();

                for (int t = 0; t < types.Length; t++)
                {
                    if (!types[t].IsAbstract)
                    {
                        CustomVignetteSearch search =
                            Activator.CreateInstance(types[t]) as CustomVignetteSearch;
                        if (search != null)
                        {
                            search.window = this;
                            searchTypeNames.Add(search.GetName());
                            searchTypes.Add(search);
                        }
                    }                 
                }
            }
        }
        #endregion -- Initialization ------------------------------------------

        #region -- Vignette Search --------------------------------------------
        /// <summary>
        /// Finds all of the vignettes in a specified scope and fills them in 
        /// to the "allVignettes" field. Overriding classes should use this to 
        /// narrow the search or else all assets will be traversed.
        /// </summary>
        protected virtual void FindVignettesInScope()
        {
            allVignettes.Clear();
            DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
            FileInfo[] allAssets = directory.GetFiles("*.asset", SearchOption.AllDirectories);
            for (int i = 0; i < allAssets.Length; i++)
            {
                string path = allAssets[i].FullName.Replace(@"\", "/").Replace(Application.dataPath, "Assets");
                VignetteGraph vg = AssetDatabase.LoadAssetAtPath(path, typeof(VignetteGraph)) as VignetteGraph;
                if (vg != null)
                    allVignettes.Add(vg);
            }
        }
        #endregion -- Vignette Search -----------------------------------------

        #region -- GUI --------------------------------------------------------
        /// <summary>
        /// Draw the currently selected search scope and the button to change 
        /// or refresh it.
        /// </summary>
        protected virtual void DrawSearchScope()
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Found " + allVignettes.Count + " vignettes");
            if (GUILayout.Button("Refresh"))
                FindVignettesInScope();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Draw the results of the last search.
        /// </summary>
        protected virtual void DrawSearchResults()
        {
            if (customSearch != null)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                resultScroll = GUILayout.BeginScrollView(resultScroll);
                customSearch.DrawResults();
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
        }

        /// <summary>
        /// Draw any popup UI over the entire window.
        /// </summary>
        protected virtual void DrawPopups()
        { }

        /// <summary>
        /// Indicates if there is any popup UI to be drawn over the window.
        /// </summary>
        /// <returns>
        /// True if all other window interactions should be disabled.
        /// </returns>
        protected virtual bool WillDrawPopups()
        { return false; }

        /// <summary>
        /// Draw the search type dropdown and the description of the current.
        /// </summary>
        private void DrawSearchSelector()
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Search Type:");
            int newSelection = EditorGUILayout.Popup(selectedType, searchTypeNames.ToArray());
            if (newSelection != selectedType)
            {
                selectedType = newSelection;
                customSearch = searchTypes[newSelection];
                searchResults.Clear();
            }
            if (customSearch != null)
            {
                GUILayout.Box(customSearch.GetDescription(), GUI.skin.label);
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Draw the search interface for the selected search.
        /// </summary>
        private void DrawCustomSearch()
        {
            GUILayout.BeginVertical(GUI.skin.box);
            
            if (customSearch == null)
            {
                GUILayout.Label("Select a search type.");
            }
            else
            {
                GUILayout.Label(searchTypeNames[selectedType] + " Search :");
                customSearch.DrawSearch();

                if (!customSearch.DisplayingPopup && GUILayout.Button("Perform search"))
                {
                    searchResults = new List<VignetteSearchResult>();
                    customSearch.PerformSearch(allVignettes);
                    customSearch.SearchComplete(searchResults);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>Draw the search window.</summary>
        protected virtual void OnGUI()
        {
            if (resultStyle == null)
            {
                resultStyle = new GUIStyle(GUI.skin.button)
                {alignment = TextAnchor.MiddleLeft};
            }
            try
            {
                bool willDrawPopups = WillDrawPopups();

                if (customSearch == null)
                    GUI.enabled = !willDrawPopups;
                else
                    GUI.enabled = !willDrawPopups && !customSearch.DisplayingPopup;

                DrawSearchScope();
                GUILayout.Space(10.0f);
                DrawSearchSelector();
                GUILayout.Space(10.0f);
                DrawCustomSearch();
                GUILayout.Space(10.0f);
                DrawSearchResults();                
                
                GUI.enabled = true;

                if (willDrawPopups)
                    DrawPopups();
                else if (customSearch != null && customSearch.DisplayingPopup)
                    customSearch.DrawPopups();
            }
            catch (Exception)
            {
                Close();
                throw;
            }
        }
        #endregion -- GUI -----------------------------------------------------
    }
}
