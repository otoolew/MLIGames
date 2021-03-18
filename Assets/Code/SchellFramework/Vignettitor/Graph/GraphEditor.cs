//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   09/04/2014
//-----------------------------------------------------------------------------

using SG.Core;
using SG.Core.OnGUI;
using SG.Vignettitor.Graph.Config;
using SG.Vignettitor.Graph.Layout;
using SG.Vignettitor.Graph.NodeViews;
using SG.Vignettitor.Graph.States;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SG.Vignettitor.Graph
{
    public class GraphEditor
    {
        #region -- Constants --------------------------------------------------
        /// <summary>Width of the command strip window.</summary>
        public const float COMMAND_WIDTH = 160.0f;

        /// <summary>
        /// If this portion of nodes overlap at the origin, do an autolayout.
        /// </summary>
        protected const float AUTOLAYOUT_THRESHOLD = 0.75f;
        #endregion -- Constants -----------------------------------------------

        #region -- Configuration Objects --------------------------------------
        public GraphVisualConfig visuals;
        #endregion -- Configuration Objects -----------------------------------

        #region -- Protected Variables ----------------------------------------
        /// <summary>
        /// Starting time when a label is depressed.
        /// </summary>
        private float labelDownTime = -1.0f;

        /// <summary>
        /// Is this the first frame rendered? Certain OnGUI changes can only 
        /// happen in an OnGUI call so this allows for certain initialization 
        /// to take place.
        /// </summary>
        protected bool firstDraw = true;

        /// <summary>
        /// Stores position of a new node until it is written out.
        /// </summary>
        protected Vector2 newNodePos;

        /// <summary>
        /// Maps each node type to a node view type that can draw it.
        /// </summary>
        protected Dictionary<Type, Type> typeDrawers;

        /// <summary>
        /// Maps each node type to a node edit view type that can draw it.
        /// </summary>
        protected Dictionary<Type, Type> nodeEditViewMap;

        /// <summary>Validation that may be used on the graph.</summary>
        protected ContentValidation currentGraphValidation = new ContentValidation();
        #endregion -- Protected Variables -------------------------------------

        #region -- Public Variables -------------------------------------------
        /// <summary>
        /// Stores data about the current display of the graph.
        /// </summary>
        public GraphViewState graphViewState;

        /// <summary>
        /// Defines Special drawing operations for use in the graph.
        /// </summary>
        public DrawAdapter DrawAdapter = null;

        /// <summary>
        /// The bounds of the available draw area for the editor.
        /// </summary>
        public Rect Container;

        public bool ReadOnly = false;

        /// <summary>
        /// Current nodeViewState of the editor to handle input and display extra data.
        /// </summary>
        public GraphEditorState state;

        /// <summary>
        /// Actions that should be performed at the end of the frame. Some 
        /// code is not safe to call during GUI calls and needs to be deferred.
        /// </summary>
        public Action EndOfFrameActions;

        /// <summary>
        /// Manages getting and setting selections in the editor.
        /// </summary>
        public NodeSelectionManager SelectionManager { get; private set; }

        /// <summary>
        /// Manages editable note displayed on the graph.
        /// </summary>
        public AnnotationManager AnnotationManager { get; private set; }

        /// <summary>
        /// The visual representations of each node in the graph.   
        /// </summary>
        public List<NodeViewState> ViewStates
        {
            get { return graphViewState.NodeViewStates; }
            set { graphViewState.NodeViewStates = value; }
        }

        /// <summary>The scrolling offset for the graph view.</summary>
        public Vector2 Scroll
        {
            get { return graphViewState.Scroll; }
            set { graphViewState.Scroll = value; }
        }

        /// <summary>Zoom level for graph view.</summary>
        public float Zoom
        {
            get { return graphViewState.Zoom; }
            set
            {
                graphViewState.Zoom = Mathf.Clamp(value, visuals.MinZoom, visuals.MaxZoom);
            }
        }

        /// <summary>
        /// Delegate for injecting additional commands in the right panel
        /// </summary>
        public Action DrawExternalCommands = delegate { };

        /// <summary>
        /// Delegate for displaying notifications
        /// </summary>
        public Action<GUIContent> ShowNotification = delegate { };
        #endregion -- Public Variables ----------------------------------------

        #region -- Node Access ------------------------------------------------
        /// <summary>
        /// Gets the ID of the node that should be treated as the root of the 
        /// graph if applicable.
        /// </summary>
        /// <returns>A node ID.</returns>
        public virtual int GetRootID()
        { return 0; }

        /// <summary>
        /// Get the viewstate for the node with the given ID.
        /// </summary>
        /// <param name="id">ID of the node.</param>
        /// <returns>View nodeViewState representing the given node.</returns>
        public virtual NodeViewState GetViewState(int nodeIndex)
        { return ViewStates[nodeIndex]; }

        public virtual NodeViewState GetViewStateForID(int nodeID)
        {
            for (int i = 0; i < ViewStates.Count; i++)
                if (ViewStates[i].ID == nodeID)
                    return ViewStates[i];
            return null;
        }

        public virtual int GetIndexByID(int id)
        {
            return id;
        }

        public virtual int GetIDByIndex(int index)
        {
            return index;
        }

        /// <summary>
        /// Gets the children of the specified node. Classes extending this 
        /// need to implement this function to provide a child list for 
        /// hierarchies.
        /// </summary>
        /// <param name="id">ID of the node.</param>
        /// <returns>List of IDS of all of the nodes children.</returns>
        public virtual int[] GetChildren(int id)
        { return new int[0]; }
        #endregion -- Node Access ---------------------------------------------

        #region -- Lifecycle --------------------------------------------------
        public void Initialize()
        {
            StoreViewFunctions();
            AnnotationManager = new AnnotationManager(this);
            SelectionManager = new NodeSelectionManager(this,
                OnSelectionActivated, OnSelectionChanged);
            state = new IdleState(this);
        }

        public virtual void OnWindowFocusChange(bool focus)
        {
            for (int i = 0; i < ViewStates.Count; i++)
            {
                if (ViewStates[i].nodeView != null)
                    ViewStates[i].nodeView.OnWindowFocusChange(focus);
                if (ViewStates[i].nodeEditView != null)
                    ViewStates[i].nodeEditView.OnWindowFocusChange(focus);
            }
        }

        public virtual void OnSceneHierarchyChange()
        {
            for (int i = 0; i < ViewStates.Count; i++)
            {
                if (ViewStates[i].nodeView != null)
                    ViewStates[i].nodeView.OnSceneHierarchyChange();
                if (ViewStates[i].nodeEditView != null)
                    ViewStates[i].nodeEditView.OnSceneHierarchyChange();
            }
        }

        public virtual void OnSelectionChange(UnityEngine.Object[] objects)
        {
            for (int i = 0; i < ViewStates.Count; i++)
            {
                if (ViewStates[i].nodeView != null)
                    ViewStates[i].nodeView.OnSelectionChange(objects);
                if (ViewStates[i].nodeEditView != null)
                    ViewStates[i].nodeEditView.OnSelectionChange(objects);
            }
        }


        /// <summary>
        /// Builds up the typeDrawers list that maps each node type to a node
        /// view type that can draw it.
        /// </summary>
        private void StoreViewFunctions()
        {
            typeDrawers = new Dictionary<Type, Type>();
            nodeEditViewMap = new Dictionary<Type, Type>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {

                Type[] types = AssemblyUtility.GetTypes(assemblies[i]);

                for (int t = 0; t < types.Length; t++)
                {
                    object[] viewAttributes =
                        types[t].GetCustomAttributes(typeof(NodeViewAttribute), true);
                    for (int a = 0; a < viewAttributes.Length; a++)
                    {
                        if (!typeDrawers.ContainsKey(types[t]))
                        {
                            NodeViewAttribute att = (NodeViewAttribute)viewAttributes[a];
                            try
                            {
                                typeDrawers.Add(att.nodeType, types[t]);
                            }
                            catch (ArgumentException)
                            {
                                Debug.LogError("Exception Storing view functions with key " + att.nodeType);
                                throw;
                            }
                        }
                    }

                    viewAttributes = types[t].GetCustomAttributes(typeof(NodeEditViewAttribute), true);
                    for (int a = 0; a < viewAttributes.Length; a++)
                    {
                        if (!nodeEditViewMap.ContainsKey(types[t]))
                        {
                            NodeEditViewAttribute att = (NodeEditViewAttribute)viewAttributes[a];
                            try
                            {
                                nodeEditViewMap.Add(att.nodeType, types[t]);
                            }
                            catch (ArgumentException)
                            {
                                Debug.LogError("Exception Storing editView functions with key " + att.nodeType);
                                throw;
                            }
                        }
                    }
                }
            }
        }

        public virtual string GetNameForType(Type t)
        {
            return t.Name;
        }

        protected virtual Type GetDefaultView()
        { return typeof(NodeView); }

        protected void BindToViews(object node, NodeViewState nodeViewState, Type rootType)
        {
            Type nodeType = node.GetType();
            string name = GetNameForType(nodeType);

            Type viewType = GetViewForType(nodeType, typeDrawers, rootType);
            if (viewType == null)
                viewType = GetDefaultView();
            nodeViewState.nodeView = Activator.CreateInstance(viewType) as NodeView;
            nodeViewState.nodeView.Initialize(name, node);
            nodeViewState.nodeView.graphEditor = this;

            Type editViewType = GetViewForType(nodeType, nodeEditViewMap, rootType);
            if (editViewType != null)
            {
                nodeViewState.nodeEditView = Activator.CreateInstance(editViewType) as NodeEditView;
                nodeViewState.nodeEditView.Initialize(name, node);
                nodeViewState.nodeEditView.graphEditor = this;
            }
            else
            {
                CreateDefaultEditView(node, nodeViewState);
            }
        }

        protected virtual void CreateDefaultEditView(object node, NodeViewState nodeViewState)
        { }

        protected Type GetViewForType(Type nodeType, Dictionary<Type, Type> typeDrawers, Type rootType)
        {
            if (typeDrawers.ContainsKey(nodeType))
            {
                return typeDrawers[nodeType];
            }
            // search the inheritance chain for a view that matches - doesn't count interfaces at all
            var baseType = nodeType.BaseType;
            while (baseType != rootType)
            {
                Type viewType;
                if (typeDrawers.TryGetValue(baseType, out viewType))
                {
                    // add this to the dictionary to save future lookups
                    typeDrawers.Add(nodeType, viewType);
                    return viewType;
                }
                baseType = baseType.BaseType;
            }
            return null;
        }

        public virtual void Update()
        {
            AnnotationManager.Update();
            state.Update();
            if (EndOfFrameActions != null)
            {
                EndOfFrameActions();
                EndOfFrameActions = null;
            }
        }
        #endregion -- Lifecycle -----------------------------------------------

        #region -- Graph Modification -----------------------------------------
        protected virtual void OnSelectionChanged()
        { state.OnSelectionChanged(); }

        protected virtual void OnSelectionActivated()
        { EnterSelectedState(); }


        public virtual void CreateNewNode(int parent, int output, Vector2 position, Type t)
        { }

        public virtual void DeleteNode(int[] ids)
        { }

        public virtual void DeleteConnection(int id, int childIndex, bool skipUndo = false)
        { }

        public virtual void ConnectNodes(int parent, int output, int child)
        { }
        #endregion -- Graph Modification --------------------------------------

        #region -- Input ------------------------------------------------------

        public bool OnValidateCommand(string command)
        {
            return state.OnValidateCommand(command);
        }

        public void OnExecuteCommand(string command)
        {
            state.OnExecuteCommand(command);
        }

        protected virtual void OnGraphMouseDown(Vector2 localMousePos)
        {
            state.OnDashMouseDown(localMousePos);
        }

        protected virtual void OnGraphMouseUp(Vector2 localMousePos)
        {
            state.OnDashMouseUp(localMousePos);
        }

        protected virtual void OnGraphMouseMove(Vector2 localMousePos)
        {
            state.OnDashMouseMove(localMousePos);
        }

        protected virtual void OnNodeMouseUp(int nodeID, Vector2 localMousePos)
        {
            state.OnNodeMouseUp(nodeID, localMousePos);
        }

        protected virtual void OnNodeMouseDown(int nodeID, Vector2 localMousePos)
        {
            state.OnNodeMouseDown(nodeID, localMousePos);
        }

        protected virtual void OnNodeMouseWheel(int nodeID, float delta, Vector2 screenMousePos)
        {
            AdjustZoom(delta, screenMousePos);
        }

        protected virtual void OnNodeMouseMove(int nodeID, Vector2 localMousePos)
        {
            state.OnNodeMouseMove(nodeID, localMousePos);
        }

        protected virtual void OnGraphMouseWheel(float delta, Vector2 screenMousePos)
        {
            AdjustZoom(delta, screenMousePos);
        }

        protected void HandleNodeInput(int nodeID, Vector2 screenMousePos, Vector2 localMousePos)
        {
            if (Event.current.type == EventType.ScrollWheel)
                OnNodeMouseWheel(nodeID, Event.current.delta.y, screenMousePos);
            else if (Event.current.type == EventType.MouseDown)
                OnNodeMouseDown(nodeID, localMousePos);
            else if (Event.current.type == EventType.MouseUp)
                OnNodeMouseUp(nodeID, localMousePos);
            else if (Event.current.type == EventType.MouseDrag)
                OnNodeMouseMove(nodeID, localMousePos);
        }

        protected virtual void HandleGraphInput()
        {
            Vector2 mp1 = Event.current.mousePosition;
            if (Event.current.type == EventType.MouseDrag)
                OnGraphMouseMove(mp1);
            else if (Event.current.type == EventType.MouseDown)
                OnGraphMouseDown(mp1);
            else if (Event.current.type == EventType.MouseUp)
                OnGraphMouseUp(mp1);
            else if (Event.current.type == EventType.ScrollWheel)
                OnGraphMouseWheel(Event.current.delta.y, mp1);
        }
        #endregion -- Input ---------------------------------------------------

        #region -- Graph Transform Helpers ------------------------------------
        public void AdjustZoom(float increment, Vector2 mousePosition)
        {
            Vector2 before = ScreenToGraphPoint(mousePosition);
            Zoom = Mathf.Clamp(Zoom - increment * visuals.ZoomSensitivity, visuals.MinZoom, visuals.MaxZoom);
            Vector2 after = ScreenToGraphPoint(mousePosition);
            FocusOnPoint(GetFocalPoint() - (after - before));
            //DrawAdapter.AddCursorRect(new Rect(-int.MaxValue / 2, -int.MaxValue / 2, int.MaxValue, int.MaxValue), DrawAdapter.MouseCursor.Zoom);
        }

        /// <summary>
        /// Gets the graph-space rect that is displayed on the screen, taking 
        /// zoom into account.
        /// </summary>
        /// <param name="ignoreCommandStrip">
        /// If true, the rect will go behinsd the command strip.
        /// </param>
        /// <returns>Graph-space rect.</returns>
        public Rect GetDisplayedRect(bool ignoreCommandStrip)
        {
            float w = Container.width;
            if (!ignoreCommandStrip)
                w -= COMMAND_WIDTH;
            Vector2 ul = ScreenToGraphPoint(new Vector2(0, 0));
            return new Rect(ul.x, ul.y, w / Zoom, Container.height / Zoom);
        }

        /// <summary>
        /// For the given rectangle, returns a rectangle with the same 
        /// dimensions that is fully visible, assuming the width and height 
        /// fit inside the editor window.
        /// </summary>
        /// <param name="input">Rectangle to fit to the screen.</param>
        /// <returns>A rectangle that is fully visible.</returns>
        public Rect GetFullyOnScreenRect(Rect input)
        {
            Rect result = new Rect(input);
            if (result.width > Container.width || result.x < 0)
                result.x = 0;
            else
                result.x -= Mathf.Max(0.0f, result.xMax - Container.width);

            if (result.height > Container.height || result.y < 0)
                result.y = 0;
            else
                result.y -= Mathf.Max(0.0f, result.yMax - Container.height);

            return result;
        }

        public Vector2 GetFocalPoint()
        {
            return ScreenToGraphPoint(new Vector2(Container.width / 2.0f, Container.height / 2.0f));
        }

        public Rect ScreenToGraphRect(Rect input)
        {
            Rect result = new Rect(
                (input.x - Scroll.x) / Zoom,
                (input.y - Scroll.y) / Zoom,
                input.width / Zoom,
                input.height / Zoom);
            return result;
        }

        public Rect GraphToScreenRect(Rect input)
        {
            Rect result = new Rect(
                (input.x * Zoom) + Scroll.x,
                (input.y * Zoom) + Scroll.y,
                (input.width) * Zoom,
                (input.height) * Zoom);
            return result;
        }

        public Vector2 ScreenToGraphPoint(Vector2 input)
        {
            Vector2 result = new Vector2(
                (input.x - Scroll.x) / Zoom,
                (input.y - Scroll.y) / Zoom);
            return result;
        }

        public Vector2 GraphToScreenPoint(Vector2 input)
        {
            Vector2 result = new Vector2(
                (input.x * Zoom) + Scroll.x,
                (input.y * Zoom) + Scroll.y);
            return result;
        }

        /// <summary>
        /// Get the rectangle used to draw the command strip.
        /// </summary>
        /// <returns>A Screen-space rect.</returns>
        public Rect GetCommandRect()
        {
            Rect comRect = Container;
            comRect.width = COMMAND_WIDTH;
            comRect.x = Container.width - comRect.width;
            return comRect;
        }

        public void FocusOnPoint(Vector2 point, bool ignoreComRect = true)
        {
            Vector2 ul = new Vector2(-point.x * Zoom, -point.y * Zoom);
            Vector2 screenOffset;
            if (ignoreComRect)
                screenOffset = new Vector2(Container.width / 2.0f, Container.height / 2.0f);
            else
                screenOffset = new Vector2((Container.width - GetCommandRect().width) / 2.0f, Container.height / 2.0f);
            Scroll = ul + screenOffset;
        }

        public void FocusOnNode(int id)
        {
            Vector2 ul = new Vector2(
                -ViewStates[id].Position.x * Zoom - visuals.NodeWidth / 2.0f * Zoom,
                -ViewStates[id].Position.y * Zoom - visuals.NodeHeight / 2.0f * Zoom);
            Vector2 screenOffset = new Vector2(Container.width / 2.0f, Container.height / 2.0f);
            Scroll = ul + screenOffset;
        }

        public void FocusOnAnnotation(int index)
        {
            Vector2 ul = new Vector2(
                -graphViewState.Annotations[index].Position.x * Zoom - graphViewState.Annotations[index].Position.width / 2.0f * Zoom,
                -graphViewState.Annotations[index].Position.y * Zoom - graphViewState.Annotations[index].Position.height / 2.0f * Zoom);
            Vector2 screenOffset = new Vector2(Container.width / 2.0f, Container.height / 2.0f);
            Scroll = ul + screenOffset;
        }

        public Rect GetBoundsByIDs(int[] nodeIDs)
        {
            if (nodeIDs.Length < 1)
                return new Rect();


            Rect result = GetViewStateForID(nodeIDs[0]).GetRect();
            for (int i = 1; i < nodeIDs.Length; i++)
            {
                Rect r = GetViewStateForID(nodeIDs[i]).GetRect();
                result.xMax = Mathf.Max(result.xMax, r.xMax);
                result.yMax = Mathf.Max(result.yMax, r.yMax);
                result.yMin = Mathf.Min(result.yMin, r.yMin);
                result.xMin = Mathf.Min(result.xMin, r.xMin);
            }
            return result;
        }

        public Rect GetBounds(int[] nodeIndices)
        {
            if (nodeIndices.Length < 1)
                return new Rect();

            Rect result = ViewStates[nodeIndices[0]].GetRect();
            for (int i = 1; i < nodeIndices.Length; i++)
            {
                Rect r = ViewStates[nodeIndices[i]].GetRect();
                result.xMax = Mathf.Max(result.xMax, r.xMax);
                result.yMax = Mathf.Max(result.yMax, r.yMax);
                result.yMin = Mathf.Min(result.yMin, r.yMin);
                result.xMin = Mathf.Min(result.xMin, r.xMin);
            }
            return result;
        }

        public Rect GetBounds(IList<Vector2> positions)
        {
            if (positions.Count < 1)
                return new Rect();

            Rect result = new Rect(positions[0].x, positions[0].y,
                    visuals.NodeWidth, visuals.NodeHeight);
            for (int i = 1; i < positions.Count; i++)
            {
                Rect r = new Rect(positions[i].x, positions[i].y,
                    visuals.NodeWidth, visuals.NodeHeight);
                result.xMax = Mathf.Max(result.xMax, r.xMax);
                result.yMax = Mathf.Max(result.yMax, r.yMax);
                result.yMin = Mathf.Min(result.yMin, r.yMin);
                result.xMin = Mathf.Min(result.xMin, r.xMin);
            }
            return result;
        }

        public virtual Vector2 GetNodeExit(int id, int output)
        {
            NodeViewState s = GetViewState(id);

            if (s.nodeView != null && s.nodeView.Outputs.Length > 0)
            {
                Vector2 ul = new Vector2(s.renderRect.x, s.renderRect.y);
                return s.nodeView.Outputs[output].Position.center + ul;
            }
            return s.renderRect.center + new Vector2(s.renderRect.width / 2, 0);
        }

        public Rect GetNodeRect(int id)
        {
            NodeViewState s = GetViewState(id);
            return s.renderRect;
        }

        public virtual Vector2 GetNodeEnter(int id)
        {
            NodeViewState s = GetViewState(id);
            return s.renderRect.center - new Vector2(s.renderRect.width / 2, 0);
        }
        #endregion -- Graph Transform Helpers ---------------------------------

        #region -- States -----------------------------------------------------
        public virtual void EnterState(GraphEditorState state)
        { this.state = state; }

        public virtual void EnterIdleOrSelectedState()
        {
            if (SelectionManager.AllSelected.Count > 0)
            {
                EnterSelectedState();
                OnSelectionChanged();
            }
            else
            {
                EnterIdleState();
            }
        }

        public virtual void EnterIdleState()
        { state = new IdleState(this); }

        public virtual void EnterSelectedState()
        { state = new SelectedNodeState(this); }

        public virtual void EnterEditState()
        { state = new EditNodeState(this); }

        public virtual void EnterConnectionEditState(int node, int childIndex)
        {
            ConnectionEditState s = new ConnectionEditState(this, node, childIndex);
            state = s;
        }

        public virtual void EnterEditAnnotationState()
        {
            state = new EditAnnotationState(this);
        }

        public virtual void AnimatedFocusOnSelection()
        {
            Rect bounds = GetBounds(SelectionManager.AllSelected.ToArray());
            AnimatePanState s = new AnimatePanState(this, bounds, state);
            state = s;
        }
        #endregion -- States --------------------------------------------------

        #region -- Layout -----------------------------------------------------
        /// <summary>
        /// If enough nodes in the graph have undefined draw areas, recalculate
        /// the graph layout.
        /// </summary>
        protected virtual void LayoutIfNecessary()
        {
            if (ViewStates.Count == 0)
                return;
            int brokenCount = 0;

            for (int i = 0; i < ViewStates.Count; i++)
            {
                if (ViewStates[i].Position.Equals(Vector2.zero))
                    brokenCount++;
            }
            float p = brokenCount / ViewStates.Count;
            if (p >= AUTOLAYOUT_THRESHOLD)
                LayoutGraph();
        }

        protected virtual bool UseVerticalLayout { get { return false; } }

        public virtual void LayoutGraph()
        {
            Queue<int> toVisit = new Queue<int>();
            HashSet<int> unqueued = new HashSet<int>();

            TreeLayoutNodeArranger layout = new TreeLayoutNodeArranger(visuals, GetRootID(), UseVerticalLayout);

            List<int> priorities = new List<int>();
            for (int i = 0; i < ViewStates.Count; i++)
            {
                ILayoutPriority priority = ViewStates[i].nodeView as ILayoutPriority;
                int p = priority == null ? 0 : priority.GetLayoutPriority();
                if (!priorities.Contains(p))
                    priorities.Add(p);
            }
            priorities.Sort((x, y) => y - x);

            for (int i = 0; i < priorities.Count; i++)
            {
                int priorityTier = priorities[i];

                unqueued.Clear();
                for (int v = 0; v < ViewStates.Count; v++)
                    unqueued.Add(v);

                toVisit.Enqueue(GetRootID());
                unqueued.Remove(GetRootID());

                while (toVisit.Count > 0)
                {
                    int next = toVisit.Dequeue();
                    int[] children = GetChildren(next);

                    ILayoutPriority priority = GetViewState(next).nodeView as ILayoutPriority;
                    int p = priority == null ? 0 : priority.GetLayoutPriority();
                    if (p == priorityTier)
                        layout.AddNode(next, ViewStates[next], children);

                    foreach (int child in children)
                    {
                        if (unqueued.Contains(child))
                        {
                            toVisit.Enqueue(child);
                            unqueued.Remove(child);
                        }
                    }
                }
            }

            // orphan nodes
            foreach (int leftover in unqueued)
                layout.AddNode(leftover, ViewStates[leftover], GetChildren(leftover));

            layout.GenerateLayout(Container);
        }
        #endregion -- Layout --------------------------------------------------

        #region -- Drawing ----------------------------------------------------
        protected virtual void DrawDefaultNode(int id, Rect r) { }

        /// <summary>
        /// If a node viewer throws an exception, this should be used instead 
        /// to indicate the error. 
        /// </summary>
        /// <param name="id">Node that has the broken viewer.</param>
        /// <param name="r">Rectangle to draw in.</param>
        /// <param name="error">Error that occured.</param>
        protected virtual void DrawBrokenNode(int id, Rect r, string error)
        {
            OnGUIUtils.DrawBox(r, "", Color.magenta, Color.black);
            r.y += 20;
            GUI.TextArea(r, error);
        }

        protected virtual void DrawNode(int windowID)
        {
            Vector2 localMousePos = Event.current.mousePosition;

            Vector2 screenMousePos = GraphToScreenPoint(
                ViewStates[windowID].Position + (localMousePos / Zoom)
            );

            Rect r = new Rect(0, 0,
                visuals.NodeWidth * Zoom,
                visuals.NodeHeight * Zoom);

            try
            {
                //Debug.Log(nodeViewState.GetType().IsAssignableFrom(typeof(EditNodeState)).ToString()+" - "+nodeViewState.GetType().ToString());
                // check selection - if selected, try to draw the node editor
                if ((state is EditNodeState || state.GetType().IsSubclassOf(typeof(EditNodeState))) && ViewStates[windowID].nodeEditView != null && SelectionManager.IsSelected(windowID))
                {
                    ViewStates[windowID].nodeEditView.Draw(r);
                }
                else if (ViewStates[windowID].nodeView != null)
                {
                    ViewStates[windowID].nodeView.Draw(r);
                    int[] children = GetChildren(windowID);
                    ViewStates[windowID].nodeView.DrawOutputs(children);
                }
                else
                {
                    DrawDefaultNode(windowID, r);
                }
            }
            catch (Exception e)
            {
                DrawBrokenNode(windowID, r, e.Message + " : " + e.StackTrace);
            }

            HandleNodeInput(windowID, screenMousePos, localMousePos);
            if (!ReadOnly && Event.current.button == 0)
                GUI.DragWindow();
        }

        protected bool DrawOutputLabel(string text, Vector2 origin, Color highlight, bool center)
        {
            Vector2 bs = OnGUIUtils.TextStyle.CalcSize(new GUIContent(text));
            float buttonWidth = bs.x + 10;
            float buttonHeight = bs.y + 3;
            float mult = center ? 0.5f : 0.0f;

            if (Zoom >= visuals.MinZoomForLabels)
            {
                Rect labelRect = new Rect(origin.x - buttonWidth * mult, origin.y - buttonHeight * mult, buttonWidth,
                    buttonHeight);
                DrawAdapter.AddCursorRect(labelRect, DrawAdapter.MouseCursor.Link);

                // click detection since a button consumes the event
                bool result = false;
                if (Event.current.type == EventType.MouseDown && labelRect.Contains(Event.current.mousePosition))
                    labelDownTime = Time.realtimeSinceStartup;
                else if (Event.current.type == EventType.MouseUp && labelRect.Contains(Event.current.mousePosition))
                {
                    if (Time.realtimeSinceStartup - labelDownTime < 0.17f)
                        result = true;
                }

                OnGUIUtils.DrawBox(labelRect, text, visuals.LabelColor, highlight);

                return result;
            }
            return false;
        }

        protected virtual Vector2 GetNodeEnter(Rect r)
        {
            return r.center - new Vector2(r.width / 2, 0);
        }

        protected virtual Vector2 GetNodeExit(Rect r)
        {
            return r.center + new Vector2(r.width / 2, 0);
        }

        protected virtual bool DrawSelectableConnection(NodeViewState a, NodeViewState b, int ci, string label, Color color, Color highlight)
        {
            Vector2 start = GetNodeExit(a.renderRect);
            if (a.nodeView != null)
            {
                if (a.nodeView.Outputs.Length > ci)
                {
                    if (a.nodeView.Outputs[ci].State == NodeView.NodeViewOutput.NodeViewOutputState.Hidden)
                        return false;

                    Rect rr = GraphToScreenRect(a.GetRect());
                    Vector2 ul = new Vector2(rr.x, rr.y);
                    start = a.nodeView.Outputs[ci].Position.center + ul;
                }
            }

            Vector2 end = GetNodeEnter(b.renderRect);
            if (highlight != Color.clear)
                DrawLine(start, end, highlight, visuals.ConnectionWidth * 2.0f);
            DrawLine(start, end, color, visuals.ConnectionWidth);

            Vector2 connection = end - start;
            Vector2 labelVec = (connection.normalized) * connection.magnitude * 0.5f + start;

            bool result = DrawOutputLabel(label, labelVec, highlight, true);
            return result;
        }

        protected virtual Color GetConnectionColor(int parent, int childIndex)
        {
            return visuals.ConnectionColor;
        }

        protected virtual void DrawInfo()
        {
            Vector2 mp1 = Event.current.mousePosition;
            // Mouse and graph point in lower left
            OnGUIUtils.DrawBox(new Rect(0, Container.height - 41, 200, 17), "mouse " + mp1.ToString(), Color.gray, Color.black);
            OnGUIUtils.DrawBox(new Rect(0, Container.height - 20, 200, 17), "graph " + ScreenToGraphPoint(mp1).ToString(), Color.gray, Color.black);
            if (ReadOnly)
            {
                if (GUI.Button(new Rect(210, Container.height - 41, 200, 17), "Reset Pan/Zoom"))
                {
                    Scroll = new Vector2();
                    Zoom = 1.0f;
                }
            }
        }

        public virtual void DrawOverlay()
        {
            // Draw each node connections
            for (int i = 0; i < ViewStates.Count; i++)
            {
                int[] children = GetChildren(i);
                for (int c = 0; c < children.Length; c++)
                {
                    int index = -1;
                    if (children.Length > c)
                        index = children[c];
                    Color highlight = Color.clear;
                    if (state is ConnectionEditState && ((ConnectionEditState)state).Node == i && ((ConnectionEditState)state).OutputIndex == c)
                        highlight = visuals.EditConnectionColor;
                    if (index > -1)
                    {
                        Color color = visuals.ConnectionColor;
                        color = GetConnectionColor(i, c);
                        try
                        {
                            if (DrawSelectableConnection(ViewStates[i], ViewStates[index], c, ViewStates[i].GetConnectionLabel(c), color, highlight))
                                EnterConnectionEditState(i, c);
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            Debug.LogError(string.Format("Caught out of range exception for index {0} {1}", index, ViewStates.Count));
                            Debug.LogError(e.StackTrace);
                        }
                    }
                    else if (ViewStates[i].nodeView != null &&
                        ViewStates[i].nodeView.Outputs.Length > (ViewStates[i].nodeView.DrawSingleLabel() ? 0 : 1) &&
                        ViewStates[i].nodeView.Outputs.Length > c && ViewStates[i].nodeView.Outputs[c].State != NodeView.NodeViewOutput.NodeViewOutputState.Add)
                    {
                        Vector2 ul = new Vector2(ViewStates[i].renderRect.x, ViewStates[i].renderRect.y);
                        Vector2 start = new Vector2(
                            ul.x + ViewStates[i].nodeView.Outputs[c].Position.xMax + 2,
                            ul.y + ViewStates[i].nodeView.Outputs[c].Position.yMin);

                        /*
                        float buttonWidth = 50.0f;
                        float buttonHeight = 20.0f;
                        Rect labelRect = new Rect(start.x, start.y - buttonHeight / 2.0f, buttonWidth, buttonHeight);
                        OnGUIUtils.DrawBox(labelRect, ViewStates[i].GetConnectionLabel(c), visuals.LabelColor, visuals.ConnectionColor);*/
                        if (DrawOutputLabel(ViewStates[i].GetConnectionLabel(c), start, highlight, false))
                            EnterConnectionEditState(i, c);
                    }
                }
            }

            state.OverlayDraw();

            Rect comRect = GetCommandRect();
            if (!ReadOnly)
            {
                GUI.Window(8484, comRect, DrawCommands, "");
                GUI.BringWindowToFront(8484);
            }

            if (visuals.DrawInfo)
                DrawInfo();
        }

        protected virtual void DrawAdditionalStateCommands()
        { }

        public virtual void DrawCommands(int id)
        {
            DrawExternalCommands.Invoke();

            GUILayout.Label("______________________");
            GUILayout.Label(state.GetType().Name.Replace("State", ""));

            DrawAdditionalStateCommands();
            state.CommandDraw();

            // snap, scroll, zoom, and reset in lower right
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Auto Layout"))
                LayoutGraph();
            if (GUILayout.Button("Reset Pan/Zoom"))
            {
                Scroll = new Vector2();
                Zoom = 1.0f;
            }
            Rect gr = GUILayoutUtility.GetRect(160, 20);
            OnGUIUtils.DrawBox(gr, Scroll + " " + Zoom.ToString("0.000"), Color.gray, Color.black);
            visuals.SnapToGrid = GUILayout.Toggle(visuals.SnapToGrid, "Grid Snap");
        }

        public virtual void Draw(Rect container)
        {
            this.Container = container;
            if (firstDraw)
            {
                firstDraw = false;
                OnFirstDraw();
            }

            DrawGrid();

            OnPreDrawGraph();
            AnnotationManager.DrawAnnotations();

            for (int i = 0; i < ViewStates.Count; i++)
            {
                ViewStates[i].renderRect = GraphToScreenRect(ViewStates[i].GetRect());
                Rect r = ViewStates[i].renderRect;
                if (container.Overlaps(r))
                {
                    //DrawAdapter.AddCursorRect(r, DrawAdapter.MouseCursor.MoveArrow);
                    Rect newRect = GUI.Window(i, r, DrawNode, "");
                    Vector2 v = new Vector2(newRect.x, newRect.y);
                    ViewStates[i].Position = (v - Scroll) / Zoom;

                    if (visuals.SnapToGrid)
                    {
                        ViewStates[i].Position.x = SG.Core.SgMath.RoundTo(ViewStates[i].Position.x, visuals.GridSpace);
                        ViewStates[i].Position.y = SG.Core.SgMath.RoundTo(ViewStates[i].Position.y, visuals.GridSpace);
                    }
                }
            }

            if (DrawAdapter == null || !visuals.LinesOnTop)
                DrawOverlay();

            if (Event.current.type == EventType.Repaint)
                GUI.BringWindowToFront(10023);
            else
                GUI.BringWindowToBack(10023);

            state.Draw();

            HandleGraphInput();
        }

        protected virtual void DrawLine(Vector2 start, Vector2 end, Color color, float width)
        {
            if (DrawAdapter == null)
                OnGUIUtils.DrawLine(start, end, color);
            else
            {
                float dist = Mathf.Abs(start.x - end.x);
                Vector3 st = new Vector3(start.x + dist / 3.0f, start.y);
                Vector3 et = new Vector3(end.x - dist / 3.0f, end.y);
                DrawAdapter.DrawConnection(start, end, st, et, color, width);
            }
        }

        protected virtual void DrawGrid()
        {
            float spacing = visuals.GridSpace;
            float step = spacing * Zoom;
            while (step <= (float)visuals.GridSpace * visuals.MinGridSpacePercent)
            {
                spacing *= 4.0f;
                step = spacing * Zoom;
            }
            float hiddenDrawn = (-Scroll.x / spacing) / Zoom;
            float remainingFraction = Mathf.Ceil(hiddenDrawn) - hiddenDrawn;
            float xLead = remainingFraction * spacing;
            for (float i = xLead * Zoom; i < Container.width; i += step)
            {
                OnGUIUtils.DrawLine(
                    new Vector2(i, 0),
                    new Vector2(i, Container.height),
                    visuals.GridColor);
            }

            hiddenDrawn = (-Scroll.y / spacing) / Zoom;
            remainingFraction = Mathf.Ceil(hiddenDrawn) - hiddenDrawn;
            float yLead = remainingFraction * spacing;
            for (float i = yLead * Zoom; i < Container.height; i += step)
            {
                OnGUIUtils.DrawLine(
                    new Vector2(0, i),
                    new Vector2(Container.width, i),
                    visuals.GridColor);
            }
        }

        public virtual void OnFirstDraw() { LayoutIfNecessary(); }
        public virtual void OnPreDrawGraph() { }
        public virtual void OnPostDrawGraph() { }

        /// <summary>
        /// Function to clear the GUI focus control. Otherwise GUI focus may be
        /// retained on text fields we are not drawing, which leads to unusual
        /// behavior when we draw them again.
        /// </summary>
        public virtual void ResetFocus()
        {
            GUI.SetNextControlName("Focus Dummy");
            GUI.Label(new Rect(0f, 0f, 0f, 0f), GUIContent.none);
            GUI.FocusControl("Focus Dummy");
        }
        #endregion -- Drawing -------------------------------------------------

        public virtual void CleanSelection()
        {

        }
    }
}