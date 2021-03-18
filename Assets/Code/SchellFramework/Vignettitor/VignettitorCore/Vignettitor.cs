//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   05/05/2014
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using SG.Core;
using SG.Core.OnGUI;
using SG.Vignettitor.Graph;
using SG.Vignettitor.Graph.Config;
using SG.Vignettitor.Graph.Drawing;
using SG.Vignettitor.Graph.NodeViews;
using SG.Vignettitor.NodeViews;
using SG.Vignettitor.Runtime;
using SG.Vignettitor.VignetteData;
using SG.Vignettitor.VignettitorCore.States;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SG.Vignettitor.VignettitorCore
{
    /// <summary>
    /// Displays and allows for editing of vignette graphs.
    /// </summary>
    public class Vignettitor : GraphEditor
    {
        private static readonly Notify Log = NotifyManager.GetInstance("SG.Vignettitor");

        #region -- Public Variables -------------------------------------------

        public NodeViewState HeadView
        {
            get { return graphViewState.HeadViewState; }
            set { graphViewState.HeadViewState = value; }
        }

        /// <summary>
        /// Provides a set of clipboards, one for each graph type that allow 
        /// for copying and pasting within and across graphs of the same type.
        /// </summary>
        public static ClipboardManager ClipboardManager;

        /// <summary>
        /// All nodes available in the graph.
        /// </summary>
        public VignetteNode[] allNodes;

        /// <summary>head of the current graph.</summary>
        public VignetteGraph head;

        /// <summary>
        /// Should nodes that are meant for parent graph types be available in
        /// this vignettitor? If this is true, nodes meant for "VignetteGraph"
        /// are available on top of nodes meant for your custom graph type.
        /// </summary>
        public virtual bool InheritNodes { get { return true; } }
        #endregion -- Public Variables ----------------------------------------

        #region -- Private Variables ------------------------------------------
        /// <summary>
        /// Maps a vignetteNode object to it's index in this editor.
        /// </summary>
        private readonly Dictionary<VignetteNode, int> nodeIndices = new Dictionary<VignetteNode, int>();

        /// <summary> Maps a node ID to the actual node. </summary>
        private readonly Dictionary<int, VignetteNode> idsToNodes = new Dictionary<int, VignetteNode>();

        /// <summary> 
        /// Maps a node ID to the view that represents that node.
        /// </summary>
        private readonly Dictionary<int, NodeViewState> idsToNodeViews = new Dictionary<int, NodeViewState>();

        /// <summary>
        /// Provides functionality to load and manipulate the graph data.
        /// </summary>
        protected VignettitorDataController dataController;

        /// <summary>
        /// Nodes to be deleted on the next update. The delete needs to happen 
        /// at the end of the frame instead of in OnGUI.
        /// </summary>
        private readonly Queue<VignetteNode> toDelete = new Queue<VignetteNode>();

        /// <summary>
        /// The clipboard for the type of graph this Vignettitor edits.
        /// </summary>
        private IVignetteClipboard clipboard;

        /// <summary>
        /// An action to perform on the next update. This is useful to avoid 
        /// editing data during OnGUI calls that reference the data.
        /// </summary>
        private Action nextFrameAction;

        /// <summary>
        /// The active runtime being shown in this editor. If there is 0 or 1 active 
        /// runtimes matching the edited graph, this will be set automatically.
        /// Otherwise, a list of all currently active runtimes will appear in the toolbar,
        /// and any can be selected.
        /// </summary>
        private VignetteRuntimeGraph _activeRuntime;
        public VignetteRuntimeGraph ActiveRuntime { get { return _activeRuntime; } set { _activeRuntime = value; } }

        /// <summary>
        /// Tracker service to show currently executing vignettes runtimes.
        /// </summary>
        private IVignetteTracker _tracker;

        /// <summary>
        /// Mapping of nodes to their indices.
        /// </summary>
        private Dictionary<VignetteNode, int> nodeToIndex = new Dictionary<VignetteNode, int>();
        #endregion -- Private Variables ---------------------------------------

        #region -- Initialization ---------------------------------------------

        /// <summary>Create a new editor to view the given vignette.</summary>
        /// <param name="head">Head of the vigentte to edit.</param>
        /// <param name="dataController">
        /// The class that determines how data will be saved and loaded for
        ///  this vignettitor.
        /// </param>
        public Vignettitor(VignetteGraph head, VignettitorDataController dataController, DrawAdapter drawAdapter)
        {
            visuals = GraphVisualConfig.GetConfig(GetType());
            DrawAdapter = drawAdapter;
            _tracker = Services.Locate<IVignetteTracker>();
            this.head = head;
            this.dataController = dataController;
            if (ClipboardManager != null)
                clipboard = ClipboardManager.GetClipboard(head.GetType());
            dataController.SetVignetteHead(head);
            graphViewState = dataController.GetOrCreateViewState();
            allNodes = dataController.GetAllNodeAssets();

            Initialize();
            RefreshNodesAndViews();

            if (HeadView == null)
            {
                HeadView = new NodeViewState
                {
                    Position = new Vector2(visuals.GridSpace, visuals.NodeHeight)
                };
            }

            HeadView.scale = new Vector2(visuals.NodeWidth, visuals.NodeHeight);
            ReadOnly = false;
        }

        public Vignettitor()
        {
        }

        public override int GetIndexByID(int id)
        {
            for(int i = 0; i < allNodes.Length; i++)
                if (allNodes[i].NodeID == id)
                    return i;
            return -1;
        }

        public override int GetIDByIndex(int index)
        {
            return allNodes[index].NodeID;
        }

        /// <summary>
        /// This call prevents the node view list from getting out of sync with 
        /// the all nodes list. Due to a unity Undo/Redo issue 
        /// (https://fogbugz.unity3d.com/default.asp?723042_oskr9cfrv644qljj), 
        /// a redo may bring back broken view states. This will remove the 
        /// invalid view states but the position data is lost.
        /// 
        /// Additionally, this attempts to clean up corrupt data if possible.
        /// </summary>
        public void RefreshNodesAndViews()
        {
            dataController.UpgradeDataIfNecessary();

            allNodes = dataController.GetAllNodeAssets();
            idsToNodeViews.Clear();
            idsToNodes.Clear();
            nodeIndices.Clear();
            nodeToIndex.Clear();

            // Build a map of all nodes, making sure there are no duplicates.
            for (int i = 0; i < allNodes.Length; i++)
            {
                if (!idsToNodes.ContainsKey(allNodes[i].NodeID))
                {
                    idsToNodes.Add(allNodes[i].NodeID, allNodes[i]);
                    nodeIndices.Add(allNodes[i], i);
                }
                else
                {
                    Debug.LogError("Found duplicate NodeID for " +
                        idsToNodes[allNodes[i].NodeID] + " and " + allNodes[i]);
                }
            }

            // Loop through and clean up all view states
            for (int i = ViewStates.Count-1; i >=0; i--)
            {
                // Remove any view states pointing to a node with ID 0, since 
                // it means the data was corrupted (probably from a redo bug)
                if (ViewStates[i].ID == 0)
                {
                    Debug.LogError("Found illegal view state at index " + i + ". Removing. This may happen due to a Unity Redo bug.");
                    ViewStates.RemoveAt(i);
                    continue;
                }

                // Populate id to view mapping and remove duplicates.
                if (idsToNodes.ContainsKey(ViewStates[i].ID))
                {
                    if (idsToNodeViews.ContainsKey(ViewStates[i].ID))
                    {
                        Debug.LogError("Found duplicate viewstates for node " + ViewStates[i].ID);
                        ViewStates.RemoveAt(i);
                    }
                    else
                    {
                        idsToNodeViews.Add(ViewStates[i].ID, ViewStates[i]);
                    }
                }
                // If there is no node for this view, remove it.
                else
                {
                    Debug.LogError("Found viewstate that references a node that does not exist at index " + i);
                    ViewStates.RemoveAt(i);
                }
            }

            // If any nodes are not represented with a node view, create the 
            // missing view. Also rebind all views.
            for (int i = 0; i < allNodes.Length; i++)
            {
                VignetteNode node = allNodes[i];
                NodeViewState nvs = GetViewStateForID(node.NodeID);
                if (nvs == null)
                {
                    Debug.LogError("No view state found for node " + allNodes[i] + ". Generating one now. This may happen due to a Unity Redo bug.");
                    AddNodeView(allNodes[i], newNodePos);
                }
                else
                {
                    BindToViews(node, nvs, typeof(VignetteNode));
                }
            }

            // Since allNodes are sorted by ID, sort the viewstates to keep the
            // indices in sync
            ViewStates.Sort(0, ViewStates.Count, new ViewStateComparer());

            CleanSelection();
        }

        protected override void CreateDefaultEditView(object node, NodeViewState nodeViewState)
        {
            base.CreateDefaultEditView(node, nodeViewState);
            if (DrawAdapter.DefaultEditView != null)
            {
                List<NodeViewFieldAttribute.FieldAttributePair> fields =
                    NodeViewFieldAttribute.GetFields<NodeEditViewFieldAttribute>(node);

                if (fields.Count > 0)
                {
                    string name = GetNameForType(node.GetType());
                    nodeViewState.nodeEditView = Activator.CreateInstance(DrawAdapter.DefaultEditView, fields) as NodeEditView;
                    nodeViewState.nodeEditView.Initialize(name, node);
                    nodeViewState.nodeEditView.graphEditor = this;
                }
            }
        }

        public override void CleanSelection()
        {
            base.CleanSelection();
            // Remove any nodes from selection that no longer exist
            for (int i = 0; i < SelectionManager.AllSelected.Count; i++)
            {
                int index = SelectionManager.AllSelected[i];
                VignetteNode n = null;
                if (index < allNodes.Length)
                    n = allNodes[index];
                if (n == null)
                    SelectionManager.RemoveFromSelection(SelectionManager.AllSelected[i]);
            }
        }
        #endregion -- Initialization ------------------------------------------

        #region -- Mouse Input Overrides --------------------------------------
        protected override void OnNodeMouseDown(int nodeID, Vector2 localMousePos)
        {
            int outputCount = ViewStates[nodeID].nodeView.Outputs.Length;
            if (outputCount > 0)
            {
                bool hit = false;
                for (int i = 0; i < outputCount; i++)
                {
                    NodeView.NodeViewOutput output = ViewStates[nodeID].nodeView.Outputs[i];
                    if (!ReadOnly && Event.current.button == 0 && output.Position.Contains(localMousePos))
                    {
                        hit = true;
                        ConnectDragState cd = new ConnectDragState(this, nodeID, i, GetNodeExit(nodeID, i));
                        state = cd;
                        Event.current.Use();
                    }
                }
                if (!hit)
                {
                    state.OnNodeMouseDown(nodeID, localMousePos);
                }
            }
            else
            {
                state.OnNodeMouseDown(nodeID, localMousePos);
            }
        }
        #endregion -- Mouse Input Overrides -----------------------------------

        #region -- Node Access ------------------------------------------------
        public override int GetRootID()
        { return nodeIndices[head.Entry]; }

        public override NodeViewState GetViewState(int id)
        {
            if (id < 0) return HeadView;
            return base.GetViewState(id);
        }

        public override int[] GetChildren(int id)
        {
            if (allNodes[id] == null)
            {
                Debug.LogError("Node " + id + "(" +") has a null child.");
                return new int[0];
            }
            else
            {
                int[] result = new int[allNodes[id].Children.Length];
                for (int i = 0; i < allNodes[id].Children.Length; i++)
                    result[i] = GetIndex(allNodes[id].Children[i]);
                return result;
            }
        }

        public int GetIndex(VignetteNode node)
        {
            if (node == null)
            {
                return -1;
            }

            if (nodeToIndex.ContainsKey(node))
                return nodeToIndex[node];

            // TODO: make more efficient
            for (int i = 0; i < allNodes.Length; i++)
            {
                if (allNodes[i] == node)
                {
                    nodeToIndex[node] = i;
                    return i;
                }
            }
            nodeToIndex[node] = -1;
            return -1;
        }

        public VignetteNode GetNode(int id)
        {
            // TODO: make more efficient
            for (int i = 0; i < allNodes.Length; i++)
                if (allNodes[i].NodeID == id)
                    return allNodes[i];
            return null;
        }
        #endregion -- Node Access ---------------------------------------------

        #region -- States -----------------------------------------------------
        public override void EnterEditState()
        {
            state = new VignetteNodeEditState(this);
        }

        public override void EnterSelectedState()
        { state = new VignetteNodeSelectedState(this); }

        public override void EnterConnectionEditState(int node, int childIndex)
        {
            VignetteConnectionEditState s = new VignetteConnectionEditState(this, node, childIndex);
            state = s;
        }

        public virtual void EnterCreateNodeState(int source, int output, Vector2 position)
        {
            NewNodeState nns = new NewNodeState(this, source, output, position);
            state = nns;
        }
        #endregion -- States --------------------------------------------------

        #region -- Graph Changes ----------------------------------------------
        public List<VignetteNode> GetAllSelectedVignetteNodes()
        {
            List<VignetteNode> nodes = new List<VignetteNode>();
            for (int i = 0; i < SelectionManager.AllSelected.Count; i++)
                nodes.Add(allNodes[SelectionManager.AllSelected[i]]);
            return nodes;
        }

        public List<Vector2> GetAllSelectedPositions()
        {
            List<Vector2> positions = new List<Vector2>();
            for (int i = 0; i < SelectionManager.AllSelected.Count; i++)
                positions.Add(ViewStates[SelectionManager.AllSelected[i]].Position);
            return positions;
        }

        /// <summary>Copies the current selection.</summary>
        public void Copy()
        {
            if (clipboard != null)
            {
                List<VignetteNode> nodes = GetAllSelectedVignetteNodes();
                clipboard.Copy(nodes, GetAllSelectedPositions(), AnnotationManager.GetAnnotationsForBoundNodes(nodes), dataController);
            }
        }

        /// <summary>Pastes a previously copied selection.</summary>
        public void Paste()
        {
            nextFrameAction = PerformPaste;
        }

        private void PerformPaste()
        {
            if (clipboard != null && clipboard.HasPasteData())
            {
                VignettePasteResult result = clipboard.Paste(dataController);
                dataController.RegisterUndo(new Object[] { dataController.head, graphViewState }, "Paste");

                // Get the bounds of the pasted nodes, and calculate the 
                // offset needed to center them on the screen.
                Rect bounds = GetBounds(result.Positions);
                Rect newBounds = bounds;
                newBounds.center = GetFocalPoint();
                Vector2 offset = newBounds.min - bounds.min;

                CompletePaste(result, offset);
            }
        }

        private void CompletePaste(VignettePasteResult result, Vector2 offset)
        {
            // New IDs will store the IDs of the nodes that were pasted.
            int[] newIDs = new int[result.Nodes.Count];

            // Create node views and store IDs.
            for (int i = 0; i < result.Nodes.Count; i++)
            {
                AddNodeView(result.Nodes[i], result.Positions[i] + offset);
                newIDs[i] = result.Nodes[i].NodeID;
            }
            SelectionManager.Clear();
            RefreshNodesAndViews();

            // Add pasted data to the selection.
            for (int i = 0; i < result.Nodes.Count; i++)
            {
                int index = GetIndex(result.Nodes[i]);
                SelectionManager.AddToSelection(index);
            }

            // Set the pasted annotations to point to the pasted node IDs.
            for (int i = 0; i < result.Annotations.Count; i++)
            {
                Annotation a = result.Annotations[i];
                a.Position.center += visuals.PasteOffset;
                if (a.BoundNodes != null)
                    for (int ai = 0; ai < a.BoundNodes.Count; ai++)
                        a.BoundNodes[ai] = newIDs[a.BoundNodes[ai]];

                graphViewState.Annotations.Add(a);
            }
        }

        /// <summary>
        /// Duplicates the selected items in the current graph.
        /// </summary>
        public void DuplicateNodes()
        {
            nextFrameAction = PerformDuplicateNodes;
        }

        private void PerformDuplicateNodes()
        {
            if (clipboard != null)
            {
                List<VignetteNode> nodes = GetAllSelectedVignetteNodes();
                VignettePasteResult result = clipboard.Duplicate(nodes, GetAllSelectedPositions(),
                    AnnotationManager.GetAnnotationsForBoundNodes(nodes), dataController);
                dataController.RegisterUndo(new Object[] { dataController.head, graphViewState }, "Duplicate");

                Rect bounds = GetBounds(result.Positions);
                Vector2 offset = visuals.PasteOffset;
                // If the destination is not on screen, center it
                if (!GetDisplayedRect(false).Overlaps(bounds))
                {
                    Rect newBounds = bounds;
                    newBounds.center = GetFocalPoint();
                    offset = newBounds.min - bounds.min;
                }
                CompletePaste(result, offset);
            }
        }

        public void SaveGraph()
        {
            dataController.SaveData();
        }

        protected override void OnSelectionChanged()
        {
            base.OnSelectionChanged();
            Object[] selection = new Object[SelectionManager.AllSelected.Count];
            for (int i = 0; i < selection.Length; i++)
            {
                selection[i] = allNodes[SelectionManager.AllSelected[i]];
            }
            dataController.SelectNodes(selection);
        }

        private void SetChildAt(VignetteNode parent, int index, VignetteNode child)
        {
            List<VignetteNode> children = new List<VignetteNode>(parent.Children);
            int childCount = children.Count;
            if (index >= childCount)
            {
                for (int i = 0; i <= index - childCount; i++)
                    children.Add(null);
            }

            children[index] = child;
            parent.Children = children.ToArray();
        }

        private void AddNodeView(VignetteNode node, Vector2 position)
        {
            NodeViewState nvs = new NodeViewState();
            nvs.Position = position;
            nvs.scale = new Vector2(visuals.NodeWidth, visuals.NodeHeight);
            nvs.ID = node.NodeID;

            ViewStates.Add(nvs);
            BindToViews(node, nvs, typeof(VignetteNode));
            // sort all node views by id
        }

        protected virtual Vector2 GetNewNodePosition(Vector2 input)
        {
            return input - new Vector2(0.0f, visuals.NodeHeight / 2.0f * Zoom);
        }

        public override void CreateNewNode(int parent, int output, Vector2 position, Type t)
        {
            VignetteNode node = null;
            if (parent > -1)
            {
                node = allNodes[parent];
                dataController.RegisterUndo(new Object[] { head, node }, "Create " + t.Name);
            }
            else
                dataController.RegisterUndo(new Object[] { head }, "Create " + t.Name);

            VignetteNode dest = dataController.CreateNewNode(t) as VignetteNode;

            if (dest != null)
            {
                dest.Children = new VignetteNode[0];
                newNodePos = GetNewNodePosition(position);

                AddNodeView(dest, (newNodePos - Scroll) / Zoom);

                if (parent == -1)
                {
                    head.Entry = dest;
                }
                else
                {
                    SetChildAt(node, output, dest);
                }
            }

            RefreshNodesAndViews();
            OnGraphStructureChange();
            SelectionManager.Clear();
            nextFrameAction += () => SelectionManager.AddToSelection(GetIndexByID(dest.NodeID));
        }

        public override void DeleteNode(int[] ids)
        {
            List<Object> affected = new List<Object> { head, graphViewState };

            // Store states of nodes and parents affected
            for (int index = 0; index < ids.Length; index++)
            {
                int id = ids[index];
                VignetteNode target = allNodes[id];
                affected.Add(target);
                for (int i = 0; i < allNodes.Length; i++)
                {
                    for (int c = 0; c < allNodes[i].Children.Length; c++)
                    {
                        int childID = GetIndex(allNodes[i].Children[c]);
                        if (childID == id)
                        {
                            affected.Add(allNodes[i]);
                        }
                    }
                }
            }
            dataController.RegisterUndo(affected.ToArray(), "Delete Nodes");

            // build list of nodes to delete and clear out incoming connections
            List<VignetteNode> tbd = new List<VignetteNode>();
            for (int index = 0; index < ids.Length; index++)
            {
                int id = ids[index];
                VignetteNode target = allNodes[id];
                tbd.Add(target);

                // Build a list of parents and which index points to this node.
                List<int> toDeleteParents = new List<int>();
                List<int> nodesToDelete = new List<int>();
                for (int i = 0; i < allNodes.Length; i++)
                {
                    for (int c = 0; c < allNodes[i].Children.Length; c++)
                    {
                        int childID = GetIndex(allNodes[i].Children[c]);

                        if (childID == id)
                        {
                            toDeleteParents.Add(i);
                            nodesToDelete.Add(c);
                        }
                    }
                }

                if (head.Entry == target)
                    DeleteConnection(-1, 0, true);

                // delete the incoming connections
                for (int i = 0; i < nodesToDelete.Count; i++)
                    DeleteConnection(toDeleteParents[i], nodesToDelete[i], true);
            }

            dataController.SaveData();
            List<NodeViewState> viewsToDelete = new List<NodeViewState>();
            for (int i = 0; i < tbd.Count; i++)
            {
                VignetteNode target = tbd[i];
                head.allNodes.Remove(target);
                int index = GetIndex(target);
                viewsToDelete.Add(ViewStates[index]);
                toDelete.Enqueue(target);
            }

            for (int i = 0; i < viewsToDelete.Count; i++)
                graphViewState.NodeViewStates.Remove(viewsToDelete[i]);

            dataController.GetOrCreateViewState();

            SelectionManager.Clear();
            OnGraphStructureChange();
        }

        public override void Update()
        {
            base.Update();

            if (nextFrameAction != null)
            {
                nextFrameAction.Invoke();
                nextFrameAction = null;
            }


            if (toDelete.Count > 0)
            {
                while (toDelete.Count > 0)
                {
                    VignetteNode n = toDelete.Dequeue();
                    dataController.DeleteNode(n);
                }
                RefreshNodesAndViews();
                dataController.SaveData();
            }
        }

        public override void DeleteConnection(int id, int outputIndex, bool skipUndo = false)
        {
            bool save = false;
            if (id == -1)
            {
                if (!skipUndo)
                {
                    dataController.RegisterUndo(new Object[] { head }, "Delete Entry Connection");
                }
                head.Entry = null;
                save = true;
            }
            else
            {
                VignetteNode node = allNodes[id];
                if (outputIndex < node.Children.Length)
                {
                    if (!skipUndo)
                    {
                        dataController.RegisterUndo(new Object[] { head, node }, "Delete Connection");
                    }
                    // If fixed length > 1, null out the index
                    bool delete =
                        (node.OutputRule.Rule == OutputRule.RuleType.Variable &&
                        node.Children[outputIndex] == null) ||
                        node.OutputRule.Rule == OutputRule.RuleType.Passthrough;

                    if (delete)
                    {
                        List<VignetteNode> c = new List<VignetteNode>(node.Children);
                        c.RemoveAt(outputIndex);
                        node.Children = c.ToArray();
                        save = true;
                    }
                    else if (node.Children[outputIndex] != null)
                    {
                        save = true;
                        node.Children[outputIndex] = null;
                    }
                }
            }
            if (save)
                dataController.SaveData();
            OnGraphStructureChange();
        }

        public override void ConnectNodes(int parent, int output, int child)
        {
            VignetteNode dest = allNodes[child];
            if (parent == -1)
            {
                dataController.RegisterUndo(new Object[] { head }, "Set Entry Connection");
                head.Entry = dest;
            }
            else
            {
                VignetteNode node = allNodes[parent];
                dataController.RegisterUndo(new Object[] { head, node }, "Create Connection");
                if (dest != node)
                {
                    SetChildAt(node, output, dest);
                }
            }
            dataController.SaveData();
            OnGraphStructureChange();
        }
        #endregion -- Graph Changes -------------------------------------------

        #region -- Hardwiring -------------------------------------------------
        /// <summary>
        /// Force execution to jump to a particular node for debugging 
        /// purposes. This will exit the current node and enter the target.
        /// </summary>
        /// <param name="targetNode">Node to skip to.</param>
        public void SkipToNode(VignetteNode targetNode)
        {
            if (_activeRuntime == null)
            {
                Log.Warning(targetNode, "RuntimeGraph must be set to use Runtime manipulations.");
                return;
            }

            _activeRuntime.SetNode(targetNode.NodeID);
        }

        /// <summary>
        /// Set the output overrides for nodes in the graph such that the given 
        /// target node will be hit in execution. This is useful for debugging 
        /// a node's execution.
        /// </summary>
        /// <param name="targetNode"></param>
        /// <exception cref="VignetteException">
        /// Thrown if the target node is not accesible from the graph root.
        /// </exception>
        public void HardwirePath(VignetteNode targetNode)
        {
            if (_activeRuntime == null)
            {
                Log.Warning(targetNode, "RuntimeGraph must be set to use Runtime manipulations.");
                return;
            }

            ClearHardwire();
            List<VignetteNode> path = head.GetPath(targetNode);

            if (path == null)
                throw new VignetteException("Can not hardwire a path to a node that is not accessible from the root.");

            for (int i = 0; i < path.Count-1; i++)
            {
                VignetteNode next = path[i + 1];

                int childIndex = 0;
                for (; childIndex < path[i].Children.Length; childIndex++)
                    if (path[i].Children[childIndex] == next)
                        break;
                _activeRuntime.SetHardwire(path[i].NodeID, childIndex);
            }
        }


        public void ClearHardwire()
        {
            if (_activeRuntime == null)
            {
                Log.Warning(head, "RuntimeGraph must be set to use Runtime manipulations.");
                return;
            }
            _activeRuntime.ClearHardwire();
        }

        public void ToggleHardwire(int id, int outputIndex)
        {
            if (id == -1)
                return;

            if (_activeRuntime == null)
            {
                Log.Warning(head, "RuntimeGraph must be set to use Runtime manipulations.");
                return;
            }
            _activeRuntime.SetHardwire(id, outputIndex);
        }
        #endregion -- Hardwiring ----------------------------------------------

        public virtual void OnGraphStructureChange()
        {
            Validate(false);
        }

        public void Validate(bool outputValidation)
        {
            head.CollectConnectedNodes();
            ContentValidation validation = head.Signature.Validate(head);
            for (int i = 0; i < allNodes.Length; i++)
            {
                VignetteNode node = allNodes[i];
                NodeViewState nvs = GetViewStateForID(node.NodeID);
                if (nvs != null)
                {
                    nvs.validation.Clear();
                    nvs.validation.Add(node.Validate());
                    validation.Add(nvs.validation);
                }
            }

            if (outputValidation)
                validation.OutputToDebugLog();
        }

        #region -- Drawing ----------------------------------------------------
        protected override Type GetDefaultView()
        { return typeof(VignetteNodeView); }

        public override void LayoutGraph()
        {
            HeadView.Position = new Vector2(0.0f, Container.height / 2 - visuals.NodeHeight / 2);
            base.LayoutGraph();
        }

        protected override Color GetConnectionColor(int parent, int childIndex)
        {
            if (parent == -1)
                return visuals.ConnectionColor;
            if (_activeRuntime != null && _activeRuntime[GetIDByIndex(parent)] != null &&
                _activeRuntime[GetIDByIndex(parent)].OverrideOutput == childIndex)
                return visuals.HardConnectionColor;
            return visuals.ConnectionColor;
        }

        public override void Draw(Rect container)
        {
            // Before drawing any view states, make
            if (allNodes.Length != ViewStates.Count)
            {
                Debug.LogError("Nodes and node view count mismatch. " + allNodes.Length + " " + ViewStates.Count);
                RefreshNodesAndViews();
            }
            base.Draw(container);
        }

        public override void DrawCommands(int id)
        {
            if (GUILayout.Button("Save"))
                SaveGraph();
            if (GUILayout.Button("Validate"))
                Validate(true);
            bool wasEnabled = GUI.enabled;
            GUI.enabled = clipboard != null && clipboard.HasPasteData();
            if (AnnotationManager.AnnotationInEdit == null && (GUILayout.Button("Paste") || state.MatchExecuteCommand("Paste")))
                Paste();
            GUI.enabled = wasEnabled;

            DrawRuntimeSelector();

            base.DrawCommands(id);
        }

        private void DrawRuntimeSelector()
        {
            List<VignetteRuntimeGraph> runtimes = _tracker.GetRuntimes(head);
            if (runtimes == null || runtimes.Count == 0)
            {
                if (_activeRuntime != null)
                    nextFrameAction = () => _activeRuntime = null;
                return;
            }
            if (runtimes.Count == 1)
            {
                VignetteRuntimeGraph properRuntime = runtimes[0];
                if (_activeRuntime != properRuntime)
                    nextFrameAction = () => _activeRuntime = properRuntime;
                return;
            }

            GUILayout.Label("Active Runtimes:");

            int selected = runtimes.IndexOf(_activeRuntime);
            int newSelection = GUILayout.SelectionGrid(selected, runtimes.Select((x, i) => i.ToString()).ToArray(), 1);
            if (newSelection != selected)
            {
                VignetteRuntimeGraph properRuntime = runtimes[newSelection];
                nextFrameAction = () => _activeRuntime = properRuntime;
            }
        }

        protected override void DrawAdditionalStateCommands()
        {
            base.DrawAdditionalStateCommands();
            if (GUILayout.Button("Clear All Hardwire"))
                ClearHardwire();
        }

        public override void OnFirstDraw()
        {
            // If there are no nodes yet, center the head to make it easy to find.
            if (graphViewState.NodeViewStates.Count == 0)
                HeadView.Position = new Vector2(0.0f, Container.height / 2 - visuals.NodeHeight / 2);
            base.OnFirstDraw();
        }

        public override void OnPreDrawGraph()
        {
            HeadView.renderRect = GraphToScreenRect(HeadView.GetRect());
            Rect newEntryRect = GUI.Window(-1, HeadView.renderRect, DrawHead, "");
            HeadView.Position.x = (newEntryRect.x - Scroll.x) / Zoom;
            HeadView.Position.y = (newEntryRect.y - Scroll.y) / Zoom;

            if (visuals.SnapToGrid)
            {
                HeadView.Position.x = SgMath.RoundTo(HeadView.Position.x, visuals.GridSpace);
                HeadView.Position.y = SgMath.RoundTo(HeadView.Position.y, visuals.GridSpace);
            }
        }

        protected virtual void DrawTitle(Rect windowRect)
        {
            Rect titleRect = new Rect(0, 0, windowRect.width, windowRect.height);
            GUI.Window(9777659, windowRect, id =>
            {
                if (GUI.Button(titleRect, ""))
                    dataController.SelectNodes(new Object[] { head });
                OnGUIUtils.DrawBox(titleRect, head.VignettePath, Color.white, Color.black);
            }, "", GUI.skin.label);
            GUI.BringWindowToFront(9777659);
        }

        public override void DrawOverlay()
        {
            // Draw the head connections
            if (head.Entry != null)
            {
                NodeViewState entryView = ViewStates[GetIndex(head.Entry)];
                if (DrawSelectableConnection(HeadView, entryView, 0, "Entry", visuals.ConnectionColor, Color.clear))
                    EnterConnectionEditState(-1, 0);
            }

            base.DrawOverlay();
            DrawTitle(new Rect(Container.width / 2.0f - 350, 0, 700, 17));

            // draw highlighting
            for (int i = 0; i < allNodes.Length; i++)
            {
                if (_activeRuntime != null)
                {
                    if (_activeRuntime.CurrentNodeId > -1 && allNodes[i].NodeID == _activeRuntime.CurrentNodeId)
                    {
                        Rect outerHighlight = new RectOffset(5, 5, 5, 5).Add(ViewStates[i].renderRect);
                        OnGUIUtils.DrawBox(outerHighlight, "", Color.clear, Color.red);
                    }

                    if (_activeRuntime.VisitedNodes.Contains(allNodes[i].NodeID))
                    {
                        Rect highlight = new RectOffset(3, 3, 3, 3).Add(ViewStates[i].renderRect);
                        OnGUIUtils.DrawBox(highlight, "", Color.clear, Color.red);
                    }
                }

                if (ViewStates[i].validation.validations.Count > 0)
                {
                    Rect highlight = new RectOffset(3, 3, 3, 3).Add(ViewStates[i].renderRect);
                    Rect outerHighlight = new RectOffset(5, 5, 5, 5).Add(ViewStates[i].renderRect);

                    Color32 color;
                    switch (ViewStates[i].validation.maxSeverity)
                    {
                        case NotifySeverity.Trace:
                            color = visuals.TraceColor;
                            break;
                        case NotifySeverity.Debug:
                            color = visuals.DebugColor;
                            break;
                        case NotifySeverity.Warning:
                            color = visuals.WarningColor;
                            break;
                        default:
                            color = visuals.ErrorColor;
                            break;
                    }

                    OnGUIUtils.DrawBox(highlight, "", color, color);
                    OnGUIUtils.DrawBox(outerHighlight, "", color, color);
                }
            }
        }

        protected override void DrawDefaultNode(int id, Rect r)
        {
            OnGUIUtils.DrawBox(r, allNodes[id].GetType().ToString(), Color.white, Color.black);
        }

        protected virtual void DrawHead(int windowId)
        {
            Rect r = new Rect(0, 0,
                    visuals.NodeWidth * Zoom, visuals.NodeHeight * Zoom);
            OnGUIUtils.DrawBox(r, " -- Graph Entry -- ", Color.white, Color.black);

            float w = r.width * NodeView.PIN_RATIO;
            Rect newArea = new Rect(r.xMax - w, r.center.y-w/2, w, w);
            Rect dot = new Rect(newArea);
            dot.width *= 0.85f;
            dot.height *= 0.85f;
            dot.x += newArea.width - dot.width;

            GUI.DrawTexture(dot, GraphDrawingAssets.GrayDotTexture);
            DrawAdapter.AddCursorRect(dot, DrawAdapter.MouseCursor.Link);

            Vector2 localMousePos = Event.current.mousePosition;

            if (!ReadOnly)
            {
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    if (newArea.Contains(localMousePos))
                    {
                        ConnectDragState cd = new ConnectDragState(this, windowId, 0, GetNodeExit(-1, 0));
                        state = cd;
                        Event.current.Use();
                    }
                    else if (r.Contains(localMousePos))
                    {
                        dataController.SelectNodes(new Object[] { head });
                    }
                }
                GUI.DragWindow();
            }
        }

        public override string GetNameForType(Type t)
        {
            NodeMenuAttribute[] attributes = 
                t.GetCustomAttributes(typeof(NodeMenuAttribute), true)
                as NodeMenuAttribute[];
            if (attributes != null && attributes.Length > 0)
                return attributes[0].DisplayName;
            return base.GetNameForType(t);
        }
        #endregion -- Drawing -------------------------------------------------
    }
}
