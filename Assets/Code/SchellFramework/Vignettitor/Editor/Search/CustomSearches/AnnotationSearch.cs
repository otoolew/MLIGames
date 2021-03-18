//-----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   04/20/16
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using SG.Vignettitor.VignetteData;
using UnityEditor;
using UnityEngine;
using SG.Vignettitor.Graph;

namespace SG.Vignettitor.Editor.Search.CustomSearches
{   
    /// <summary>
    /// Search for text in the annotations of vignettes.
    /// </summary>
    public class AnnotationSearch : CustomVignetteSearch
    {     
        public class AnnotationSearchResult : VignetteSearchResult
        {
            /// <summary>The Text that is indicated in the result.</summary>
            public string Text;

            public int AnnotationIndex;

            /// <summary>Index into text where the result is found.</summary>
            public int Index;

            /// <summary>How many characters were searched for.</summary>
            public int Length;

            /// <summary>
            /// The rect where the annotation exists in the graph.
            /// </summary>
            public Rect AnnotationRect;

            public AnnotationSearchResult(VignetteGraph vignette, int annotationIndex, 
                string text, int index, int length, Rect annotationRect)
                :base(vignette, null)
            {
                Text = text;
                Index = index;
                Length = length;
                AnnotationIndex = annotationIndex;
                AnnotationRect = annotationRect;
            }            
        }

        private const int MAX_RESULTS = 1000;

        #region -- Private Fields ---------------------------------------------
        /// <summary>Should the search care about case?</summary>
        private bool caseSensitive;

        /// <summary>String that will be used for the search.</summary>
        private string searchString = "";
        #endregion -- Private Fields ------------------------------------------

        #region -- CustomVignetteSearch Overrides -----------------------------
        public override string GetDescription()
        {
            return "For all of the vignettes in the selected scope, this finds all annotations used in those vignettes and can search through the text within annotations.";
        }

        public override string GetName()
        { return "Annotation"; }

        public override void PerformSearch(List<VignetteGraph> graphs)
        {
            for (int i = 0; i < graphs.Count; i++)
            {
                VignetteGraph graph = graphs[i];
                string path = AssetDatabase.GetAssetPath(graph);
                GraphViewState viewState = EditorVignettitorDataController.GetGraphViewState(path);
                if (viewState != null)
                {
                    List<Annotation> annotations = viewState.Annotations;
                    for(int a = 0; a < annotations.Count; a++)
                    {
                        List<int> results = TextVignetteSearch.SearchText(annotations[a].Note, searchString, caseSensitive);
                        foreach (int start in results)
                        {
                            OnResultFound(new AnnotationSearchResult(graph, a, annotations[a].Note,
                                start, searchString.Length, annotations[a].Position));
                        }
                    }
                }
            }
        }

        public override void DrawSearch()
        {
            base.DrawSearch();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Text:", GUILayout.ExpandWidth(false));
            searchString = GUILayout.TextField(searchString, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            caseSensitive = GUILayout.Toggle(caseSensitive, "Case Sensitive");
            GUILayout.EndHorizontal();
        }
        
        public override void DrawResults()
        {
            if (searchResults != null)
            {
                GUILayout.Label("Search Results : " + searchResults.Count + " results found."
                    + ((searchResults.Count > MAX_RESULTS) ? (" Capped at " + MAX_RESULTS) : ""));
                
                for (int i = 0; i < Mathf.Min(searchResults.Count, MAX_RESULTS); i++)
                {
                    if (searchResults[i] != null && searchResults[i].vignette != null)
                    {
                        AnnotationSearchResult result = searchResults[i] as AnnotationSearchResult;
                        GUILayout.BeginVertical(GUI.skin.box);

                        GUILayout.BeginHorizontal();
                        if (result != null && GUILayout.Button(result.vignette.name + " - Annotation # " + result.AnnotationIndex,
                            NodeSearchWindow.resultStyle))
                        {
                            EditorGUIUtility.PingObject(result.vignette);
                            Selection.activeObject = result.vignette;
                        }
                        if (result != null && GUILayout.Button("open"))
                        {
                            VignettitorWindow window = VignettitorWindowAttribute.
                                GetVignettitorWindow(result.vignette as VignetteGraph);
                            window.SetVignette(result.vignette as VignetteGraph);
                            window.GetVignettitor().FocusOnAnnotation(result.AnnotationIndex);                            
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        if (result != null)
                        {
                            string ss = result.Text.Substring(result.Index, result.Length);
                            string display = TextVignetteSearch.GetFormattedResultContext(
                                ss, result.Text, result.Index);
                            GUILayout.Label(display, TextVignetteSearch.GetTextHighlightGUIStyle());
                        }

                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                    }
                }
            }
        }
        
        #endregion -- CustomVignetteSearch Overrides --------------------------
    }
}
