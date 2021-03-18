//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   01/21/2015
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using SG.Vignettitor.VignetteData;
using UnityEditor;
using UnityEngine;

namespace SG.Vignettitor.Editor.Search.CustomSearches
{
    /// <summary>
    /// Performs a text search over the selected vignettes, displaying each 
    /// occurance of the search string in the results, where a replacement may
    /// be done.
    /// 
    /// This currently only supports finding and replacing text in fields 
    /// stored directly on a node and not in sub objects or arrays.
    /// </summary>
    public class TextVignetteSearch : CustomVignetteSearch
    {
        /// <summary>
        /// Text Search Results have additional data that is used for display 
        /// and text replacement.
        /// </summary>
        public class TextSearchResult : VignetteSearchResult
        {
            /// <summary>The Text that is indicated in the result.</summary>
            public string Text;

            /// <summary> The field the text is stored in. </summary>
            public FieldInfo FieldInfo;

            /// <summary>Index into text where the result is found.</summary>
            public int Index;

            /// <summary>How many characters were searched for.</summary>
            public int Length;

            /// <summary>Has a replacement occuded on this text?</summary>
            public bool Replaced;

            public TextSearchResult(VignetteGraph vignette, VignetteNode node,
                FieldInfo fieldInfo, string text, int index, int length)
                : base(vignette, node)
            {
                Text = text;
                Index = index;
                Length = length;
                FieldInfo = fieldInfo;
            }

            /// <summary>
            /// Perform a replacement on the text referenced by this result. 
            /// This will swap in the given text and work with the Undo System.
            /// </summary>
            /// <param name="replacement">
            /// Text to put in place of the result.
            /// </param>
            public void PerformReplace(string replacement)
            {
                string s = Text;
                string s0 = s.Substring(0, Index);
                string s1 = s.Substring(Index + Length);
                string newText = s0 + replacement + s1;
                Undo.RecordObject(vignette, "Text Replace");
                FieldInfo.SetValue(node, newText);
                Replaced = true;
                EditorUtility.SetDirty(vignette);
            }
        }

        private const string PRE_RESULT_FORMAT = "<b><color=#000000ff>";

        private const string POST_RESULT_FORMAT = "</color></b>";

        private const int MAX_RESULTS = 1000;

        #region -- Private Fields ---------------------------------------------
        /// <summary>
        /// A mapping of types to the fields that could have text.
        /// </summary>
        private Dictionary<Type, FieldInfo[]> textPerType;

        /// <summary>Should the search care about case?</summary>
        private bool caseSensitive;

        /// <summary>String that will be used for the search.</summary>
        private string searchString = "";

        /// <summary>String that can replace the text in results.</summary>
        private string replaceString = "";
        #endregion -- Private Fields ------------------------------------------

        /// <summary>
        /// Gets field identifiers for all fields on the given type that are of
        /// type LocalizedText or LocalizedText[]. This caches found entries to
        /// speed up future lookups.
        /// </summary>
        /// <param name="t">The type to search.</param>
        /// <returns>A FieldInfo for each LocalizedText field.</returns>
        private FieldInfo[] GetTextFields(Type t)
        {
            if (!textPerType.ContainsKey(t))
            {
                List<FieldInfo> fields = new List<FieldInfo>();
                foreach (FieldInfo fi in t.GetFields())
                    if (fi.FieldType == typeof(string))
                        fields.Add(fi);

                if (fields.Count == 0)
                    textPerType.Add(t, null);
                else
                    textPerType.Add(t, fields.ToArray());
            }
            return textPerType[t];
        }

        public static string GetFormattedResultContext(string searchString, string context, int indexOfResult)
        {
            string t = context;
            int start = Mathf.Max(0, indexOfResult - 15);
            int end = Mathf.Min(50 + PRE_RESULT_FORMAT.Length + POST_RESULT_FORMAT.Length, t.Length - start + PRE_RESULT_FORMAT.Length + POST_RESULT_FORMAT.Length);
            t = t.Insert(indexOfResult, PRE_RESULT_FORMAT);
            t = t.Insert(searchString.Length + indexOfResult + PRE_RESULT_FORMAT.Length, POST_RESULT_FORMAT);
            return t.Substring(start, end);
        }

        private static GUIStyle textHighlightGUIStyle;
        public static GUIStyle GetTextHighlightGUIStyle()
        {
            if (textHighlightGUIStyle == null)
            {
                textHighlightGUIStyle = new GUIStyle(GUI.skin.label) { richText = true };
            }
            return textHighlightGUIStyle;
        }

        /// <summary>
        /// Searches a LocalizedText object for the given string and returns 
        /// a list of indicies where the string was found or an empty list.
        /// </summary>        
        /// <param name="source">The text to search in.</param>
        /// <param name="search">The string to search for.</param>
        /// <param name="caseSensitive">
        /// Should the search care about case?
        /// </param>
        /// <returns>
        /// A list of ints, each representing an index where the search string
        /// was found.
        /// </returns>
        public static List<int> SearchText(string source, string search, bool caseSensitive)
        {
            List<int> results = new List<int>();
            if (source != null)
            {
                string s = source;
                int start = 0;
                while (start < s.Length)
                {
                    int end = s.Length - start;
                    int result = s.IndexOf(search, start, end,
                        caseSensitive ? StringComparison.CurrentCulture :
                        StringComparison.CurrentCultureIgnoreCase);
                    if (result == -1)
                        break;
                    else
                    {
                        results.Add(result);
                    }
                    start = result + 1;
                }
            }
            return results;
        }

        #region -- CustomVignetteSearch Overrides -----------------------------
        public override string GetDescription()
        {
            return "For all of the vignettes in the selected scope, this finds all text used directly by a node that matches the search criteria.";
        }

        public override string GetName()
        { return "Text"; }

        public override void PerformSearch(List<VignetteGraph> heads)
        {
            textPerType = new Dictionary<Type, FieldInfo[]>();

            for (int i = 0; i < heads.Count; i++)
            {
                VignetteGraph head = heads[i];
                for (int n = 0; n < head.allNodes.Count; n++)
                {
                    VignetteNode node = head.allNodes[n];
                    if (node == null)
                    {
                        Debug.LogError(string.Format("Null node in {0}. If a vignette node type has been removed recently, this may be the cause.", head.VignettePath), head);
                        continue;
                    }
                    Type nodeType = node.GetType();

                    FieldInfo[] textFields = GetTextFields(nodeType);

                    if (textFields == null)
                        continue;

                    for (int index = 0; index < textFields.Length; index++)
                    {
                        FieldInfo fi = textFields[index];
                        string[] lts = fi.GetValue(node) as string[];
                        if (lts == null)
                        {
                            lts = new string[0];
                            string lt = fi.GetValue(node) as string;
                            if (lt != null)
                                lts = new[] { lt };
                        }
                        foreach (string lt in lts)
                        {
                            List<int> results = SearchText(lt, searchString, caseSensitive);
                            foreach (int start in results)
                            {
                                OnResultFound(new TextSearchResult(head, node, fi, lt, start, searchString.Length));
                            }
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

                GUILayout.BeginHorizontal();
                GUILayout.Label("Replacement Text:", GUILayout.ExpandWidth(false));
                replaceString = GUILayout.TextField(replaceString, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Replace All"))
                {
                    foreach (VignetteSearchResult r in searchResults)
                    {
                        TextSearchResult result = r as TextSearchResult;
                        if (result != null) result.PerformReplace(replaceString);
                    }
                }
                GUILayout.EndHorizontal();


                for (int i = 0; i < Mathf.Min(searchResults.Count, MAX_RESULTS); i++)
                {
                    if (searchResults[i] != null && searchResults[i].node != null && searchResults[i].vignette != null)
                    {
                        TextSearchResult result = searchResults[i] as TextSearchResult;
                        GUILayout.BeginVertical(GUI.skin.box);

                        GUILayout.BeginHorizontal();
                        if (result != null && GUILayout.Button(result.vignette.name + " - " + result.node.name + " " + result.Index,
                            NodeSearchWindow.resultStyle))
                        {
                            EditorGUIUtility.PingObject(result.node);
                            Selection.activeObject = result.node;
                        }
                        if (result != null && GUILayout.Button("open"))
                        {
                            VignettitorWindow window = VignettitorWindowAttribute.
                                GetVignettitorWindow(result.vignette as VignetteGraph);
                            window.SetVignette(result.vignette as VignetteGraph);
                            window.GetVignettitor().FocusOnNode(result.node.NodeID);
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        if (result != null)
                        {
                            string ss = result.Text.Substring(result.Index, result.Length);
                            string display = GetFormattedResultContext(
                                ss, result.Text, result.Index);
                            GUILayout.Label(display, GetTextHighlightGUIStyle());
                        }
                        if (result != null)
                        {
                            GUI.enabled = !result.Replaced;
                            if (GUILayout.Button("Replace", GUILayout.ExpandWidth(false)))
                            {
                                result.PerformReplace(replaceString);
                                EditorUtility.SetDirty(result.vignette);
                            }
                        }
                        GUI.enabled = true;

                        GUILayout.EndHorizontal();

                        GUILayout.EndVertical();


                    }
                }
            }
        }

        #endregion -- CustomVignetteSearch Overrides --------------------------
    }
}
