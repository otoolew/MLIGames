//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   02/20/2015
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using SG.Core;
using SG.Vignettitor.Editor.Search;
using SG.Vignettitor.Graph;
using SG.Vignettitor.Graph.Config;
using SG.Vignettitor.VignetteData;
using SG.Vignettitor.VignettitorCore;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace SG.Vignettitor.Editor
{
    /// <summary>
    /// Window container for the vignettitor.
    /// </summary>
    [VignettitorWindow(typeof(VignettitorCore.Vignettitor))]
    public class VignettitorWindow : EditorWindow, IHasCustomMenu
    {
        #region -- Constants --------------------------------------------------
        /// <summary>
        /// Key to save user's choice to save view changes automatically.
        /// </summary>
        protected const string AUTOSAVE_VIEW_KEY = "AutosaveView";

        protected const string HIDE_NODE_ASSETS_KEY = "HideNodeAssets";

        protected const int MAX_HISTORY_ITEMS = 5;

        /// <summary> Tips to randomly present on open. </summary>
        protected static readonly string[] TIPS = new string[]
        {
            "Welcome to Vignettitor",
            "Tip: Ctrl-A selects all nodes in the graph.",
            "Tip: Cut, copy, paste, or duplicate using Ctrl-X, Ctrl-C, Ctrl-V, or Ctrl-D.",
            "Tip: Press \"F\" to frame the selected nodes.",
            "Tip: Selecting a node will select it in the project view.",
            "Tip: Click the vignette path up top to select it in the project.",
            "Tip: Ctrl-F will open the node search menu.",
            "Tip: Use shift to add to selection.",
            "Tip: Press Ctrl-G to open a \"Go To Node By ID\" window."
        };
        #endregion -- Constants -----------------------------------------------

        /// <summary>
        /// Provides a graph navigation history for each graph type.
        /// </summary>
        public static HistoryManager HistoryManager;

        /// <summary>
        /// Provides breadcrumb navigation for each graph type.
        /// </summary>
        public static BreadcrumbManager BreadcrumbManager;

        #region -- Protected Variables ----------------------------------------
        /// <summary>The active vignette head.</summary>
        protected VignetteGraph current;

        /// <summary>
        /// Instance Id of current vignette head. (Used on reimporting data.)
        /// </summary>
        protected int currentInstanceId;

        /// <summary> The vignettitor drawn inside of this window. </summary>
        protected VignettitorCore.Vignettitor vignettitor;

        protected string titleString = "";

        protected Breadcrumbs breadcrumbs;

        protected History history;
        #endregion -- Protected Variables -------------------------------------

        #region -- Window Functions -------------------------------------------
        /// <summary> Called when the window is opened. </summary>
        protected virtual void OnEnable()
        {
            titleContent = new GUIContent(" Vignettitor", AssetCreator.VignetteGraphIcon);
            if (!EditorPrefs.HasKey(AUTOSAVE_VIEW_KEY))
                EditorPrefs.SetBool(AUTOSAVE_VIEW_KEY, true);
            SetupRendering(false);
            ShowRandomTip();
            SetupNavigation();

            Undo.undoRedoPerformed += HandleUndo;
        }

        protected virtual void OnDisable()
        {
            if (Undo.undoRedoPerformed != null)
                Undo.undoRedoPerformed -= HandleUndo;
        }

        /// <summary>
        /// When an undo occurs, refresh the data due to a Unity "Redo" bug.
        /// </summary>
        private void HandleUndo()
        {
            if (vignettitor != null && vignettitor.head != null)
            {
                vignettitor.RefreshNodesAndViews();
                vignettitor.OnGraphStructureChange();
            }
        }

        /// <summary> Called 100 times per second by the editor. </summary>
        protected virtual void Update()
        {
            Repaint();
            if (vignettitor != null)
                vignettitor.Update();
        }

        /// <summary>Fill in the window drop down options.</summary>
        /// <param name="menu"></param>
        public void AddItemsToMenu(GenericMenu menu)
        {
            var moduleInfo = new VignettitorModuleInfo();
            string version = string.Format("Vignettitor {0}, File Format {1}",
                moduleInfo.Version, VignettitorVersion.SAVE_FORMAT_VERSION);
            bool autosaveView = EditorPrefs.GetBool(AUTOSAVE_VIEW_KEY, true);
            bool hideNodeAssets = EditorPrefs.GetBool(HIDE_NODE_ASSETS_KEY, true);
            menu.AddItem(
                new GUIContent("Documentation"), false,
                () => Application.OpenURL(moduleInfo.DocsUrl.ToString()));
            menu.AddItem(
                new GUIContent("Search"), false, OpenSearch);
            menu.AddItem(
                new GUIContent("Show Tip"), false, ShowRandomTip);
            menu.AddItem(
                new GUIContent("Customize Visuals"), false, CustomizeVisuals);
            menu.AddItem(
                new GUIContent("Auto-Save View Changes"), autosaveView,
                () =>
                {
                    autosaveView = !autosaveView;
                    EditorPrefs.SetBool(AUTOSAVE_VIEW_KEY, autosaveView);
                    if (autosaveView)
                        vignettitor.SaveGraph();
                    SetupRendering(true);
                });
            menu.AddItem(
                new GUIContent("Hide Node Assets"), hideNodeAssets,
                () =>
                {
                    hideNodeAssets = !hideNodeAssets;
                    EditorPrefs.SetBool(HIDE_NODE_ASSETS_KEY, hideNodeAssets);
                    EditorVignettitorDataController.ChangeGraphVisibility(vignettitor.head, hideNodeAssets ? HideFlags.HideInHierarchy : HideFlags.None);
                    vignettitor.SaveGraph();
                    SetupRendering(true);
                });
            menu.AddItem(
                new GUIContent("Generate Asset Icon"), false,
                () =>
                {
                    if (vignettitor != null && vignettitor.head != null)
                        AssetCreator.CreateGraphIcon(vignettitor.head.GetType().Name);
                });
            menu.AddItem(new GUIContent(version), false, () => { });
        }

        protected virtual void OnFocus()
        {
            if (vignettitor != null)
                vignettitor.OnWindowFocusChange(true);
        }

        protected virtual void OnLostFocus()
        {
            if (vignettitor != null)
                vignettitor.OnWindowFocusChange(false);
        }

        protected virtual void OnHierarchyChange()
        {
            if (vignettitor != null) 
                vignettitor.OnSceneHierarchyChange();
        }
        

        /// <summary>
        /// Called whenever the project selection changes, this will sync the 
        /// project selection with the selection in the graph. if the project 
        /// selection is different than the vignettitor selection, the 
        /// vignettitor selection will be cleared and rebuilt based on the
        ///  project selection.
        /// </summary>
        protected virtual void OnSelectionChange()
        {
            if (vignettitor == null) return;

            vignettitor.OnSelectionChange(Selection.objects);

            List<int> idsToAdd = new List<int>();
            int newSelections = 0;
            foreach (Object o in Selection.objects)
            {
                VignetteNode node = o as VignetteNode;
                if (node != null && vignettitor.allNodes.Contains(node))
                {
                    int id = vignettitor.GetIndex(node);
                    idsToAdd.Add(id);
                    if (!vignettitor.SelectionManager.IsSelected(id))
                        newSelections++;
                }
            }

            if (newSelections > 0 || vignettitor.SelectionManager.AllSelected.Count != idsToAdd.Count)
            {
                if (idsToAdd.Count > 0)
                    vignettitor.SelectionManager.Clear();

                for (int index = 0; index < idsToAdd.Count; index++)
                {
                    int id = idsToAdd[index];
                    vignettitor.SelectionManager.AddToSelection(id);
                }
            }
        }

        #endregion -- Window Functions ----------------------------------------

        #region -- Protected Methods ------------------------------------------
        /// <summary>
        /// Selects the visual config or creates a new one and selects it if 
        /// none exists.
        /// </summary>
        protected virtual void CustomizeVisuals()
        {
            GraphVisualConfig config = 
                Resources.Load(GraphVisualConfig.GetConfigPath(vignettitor.GetType()), typeof(GraphVisualConfig)) as GraphVisualConfig;
            if (config == null)
            {
                config = CreateInstance<GraphVisualConfig>();
                EditorAssetDirectoryUtility.CreateDirectoriesAndAsset(config, "Assets/Resources/" + GraphVisualConfig.GetConfigPath(vignettitor.GetType()) + ".asset");
                AssetDatabase.SaveAssets();
            }
            vignettitor.visuals = config;
            Selection.activeObject = config;
        }

        /// <summary> Shows a random tip notification. </summary>
        protected virtual void ShowRandomTip()
        {
            // This can throw a null reference inside of Unity if it is called 
            // after an assembly reload.
            try
            {
                ShowNotification(new GUIContent(TIPS[Random.Range(0, TIPS.Length)]));
            }
            catch (NullReferenceException)
            { }
        }

        protected virtual void SetupRendering(bool rebuild)
        {
            if ((vignettitor == null || rebuild) && current != null)
            {
                if (VignettitorCore.Vignettitor.ClipboardManager == null)
                    VignettitorCore.Vignettitor.ClipboardManager = new ClipboardManager(typeof(EditorVignetteClipboard));

                vignettitor = Create(new EditorDrawing());

                vignettitor.DrawExternalCommands += DrawEditorCommands;
                vignettitor.ShowNotification += ShowNotification;
                vignettitor.OnWindowFocusChange(true);
            }
        }

        /// <summary> Create a new vignettitor to use in this window.</summary>
        /// <returns>A new Vignettitor.</returns>
        protected virtual VignettitorCore.Vignettitor Create(DrawAdapter drawAdapter)
        {
            object[] attributes = GetType().GetCustomAttributes(typeof(VignettitorWindowAttribute), false);
            for (int a = 0; a < attributes.Length; a++)
            {
                VignettitorWindowAttribute vwa = attributes[a] as VignettitorWindowAttribute;
                if (vwa != null)
                {
                    EditorVignettitorDataController dc = new EditorVignettitorDataController();
                    dc.UseViewStateCopy = !EditorPrefs.GetBool(AUTOSAVE_VIEW_KEY, true);
                    dc.HideSubAssets = EditorPrefs.GetBool(HIDE_NODE_ASSETS_KEY, true);
                    //titleContent = new GUIContent(vwa.vignettitorType.ToString(), AssetCreator.VignetteGraphIcon);
                    VignettitorCore.Vignettitor v = Activator.CreateInstance(vwa.vignettitorType, current, dc, drawAdapter) as VignettitorCore.Vignettitor;
                    return v;
                }
            }
            return null;
        }

        /// <summary>
        /// Open the node search window for this vignettitor and set the 
        /// search scope to the current graph.
        /// </summary>
        protected virtual void OpenSearch()
        {
            NodeSearchWindow window = NodeSearchWindow.Open();
            if (current != null)
                window.SetScope(new[] { current });
        }

        /// <summary>
        /// Inititalize history and breadcrumb navigation.
        /// </summary>
        protected virtual void SetupNavigation()
        {
            if (BreadcrumbManager == null)
                BreadcrumbManager = new BreadcrumbManager();

            if (HistoryManager == null)
                HistoryManager = new HistoryManager();

            history = HistoryManager.GetInstance(typeof(VignetteGraph));
            breadcrumbs = BreadcrumbManager.GetInstance(typeof(VignetteGraph));

            if (vignettitor != null)
            {
                Type graphType = vignettitor.head.GetType();
                breadcrumbs = BreadcrumbManager.GetInstance(graphType);
                SetupHistory();
            }
        }
        #endregion -- Protected Methods ---------------------------------------

        #region -- Public Methods ---------------------------------------------
        /// <summary>Sets the vignette that will be drawn.</summary>
        /// <param name="head">The head of the vignette graph to draw.</param>
        public virtual void SetVignette(VignetteGraph head)
        {
            var tracker = new Breadcrumbs();
            if (vignettitor != null)
            {
                tracker = breadcrumbs;
            }

            current = head;
            currentInstanceId = current.GetInstanceID();
            SetupRendering(true);

            if (vignettitor != null)
            {
                vignettitor.Container = new Rect(0, 0, position.width, position.height);
                UpdateBreadcrumbs(tracker);
                if (history != null && !history.Contains(vignettitor.head))
                    history.Add(vignettitor.head);
            }
        }

        /// <summary>
        /// Adds a graph to this window's breadcrumb list.
        /// </summary>
        /// <param name="graph">Graph to add</param>
        public void AddBreadcrumb(VignetteGraph graph)
        {
            breadcrumbs.Push(graph);
        }

        /// <summary>
        /// Returns the current active vignette graph.
        /// </summary>
        /// <returns>
        /// The instance of the graph currently being edited.
        /// </returns>
        public VignetteGraph GetActiveGraph()
        {
            return vignettitor.head;
        }

        /// <summary>
        /// Gets the vignettitor used by this window.
        /// </summary>
        /// <returns></returns>
        public VignettitorCore.Vignettitor GetVignettitor()
        {
            return vignettitor;
        }
        #endregion -- Public Methods ------------------------------------------

        #region -- GUI --------------------------------------------------------

        protected virtual void DrawEditorCommands()
        {
            if (history == null)
                SetupHistory();
            else
            {
                GUILayout.Label("______________________");
                GUILayout.Label("History");
                var localHistory = history.GetAll();
                for (int i = 0; i < localHistory.Length; i++)
                {
                    VignetteGraph recent = localHistory[i];
                    if (!recent)
                        continue;
                    if (GUILayout.Button(recent.name, EditorStyles.miniButton))
                    {
                        SetVignette(recent);
                    }
                }
            }
        }

        /// <summary>
        /// Draws a window in the lower left that has buttons to navigate 
        /// upwards to container graphs. This is done in a window to prevent
        /// the clicks from being interfered with by the graph.
        /// </summary>
        protected virtual void DrawBreadCrumbWindow()
        {
            if (CanNavigate())
            {
                Rect r = position;
                r.width -= GraphEditor.COMMAND_WIDTH;
                Rect crumbRect = BreadcrumbGUI.GetRect(r, breadcrumbs);

                GUI.Window(BreadcrumbGUI.DefaultWindowID, crumbRect, (id) =>
                {
                    BreadcrumbGUI.Draw(crumbRect, breadcrumbs, SetVignette);

                }, "", GUIStyle.none);
                GUI.BringWindowToFront(BreadcrumbGUI.DefaultWindowID);
            }
        }

        protected virtual void OnGUI()
        {
            try
            {
                BeginWindows();
                wantsMouseMove = true;
                Rect p = new Rect(0, 0, position.width, position.height);

                if (vignettitor != null)
                {
                    if (current == null)
                    {
                        current = EditorUtility.InstanceIDToObject(currentInstanceId) as VignetteGraph;
                        if (current != null)
                        {
                            SetupRendering(true);
                            vignettitor.Draw(p);
                        }
                        else
                            vignettitor = null;
                    }
                    else
                        vignettitor.Draw(p);

                    DrawBreadCrumbWindow();
                }
                EndWindows();
                if (Event.current.type == EventType.MouseMove ||
                    Event.current.type == EventType.MouseDrag ||
                    Event.current.type == EventType.MouseUp ||
                    Event.current.type == EventType.MouseDown)
                    Repaint();

                if (Event.current.type == EventType.ValidateCommand)
                {
                    if (Event.current.commandName == "Find")
                        Event.current.Use();
                    else if (vignettitor != null && vignettitor.OnValidateCommand(Event.current.commandName))
                        Event.current.Use();
                }
                else if (Event.current.type == EventType.ExecuteCommand)
                {
                    if (Event.current.commandName == "Find")
                        OpenSearch();
                    else if (vignettitor != null) vignettitor.OnExecuteCommand(Event.current.commandName);
                }

                if (vignettitor != null && vignettitor.visuals.LinesOnTop)
                {
                    vignettitor.DrawOverlay();
                }
            }
            catch (Exception)
            {
                Close();
                throw;
            }
        }
        #endregion -- GUI -----------------------------------------------------

        #region -- Private Methods---------------------------------------------

        private void UpdateBreadcrumbs(Breadcrumbs nav)
        {
            breadcrumbs = nav;
            if (breadcrumbs.Top == vignettitor.head)
            {
                breadcrumbs.Pop();
            }
        }

        private bool CanNavigate()
        {
            if (vignettitor == null)
                return false;

            return !breadcrumbs.AtRoot;
        }

        private void SetupHistory()
        {
            if (history == null &&
                vignettitor != null)
            {
                history = HistoryManager.GetInstance(vignettitor.head.GetType());
                history.Add(vignettitor.head);
            }
        }
        #endregion -- Private Methods------------------------------------------
    }
}
