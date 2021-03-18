//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   10/03/2014
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using SG.Vignettitor.Graph.Config;
using UnityEngine;

namespace SG.Vignettitor.Graph.Layout
{
    /// <summary>
    /// Takes a list of vignette nodes with node child relationships and lays 
    /// the nodes out in a tree like structure, treating the first path to a 
    /// node as it's "tree path". This will make sure no nodes in a graph 
    /// overlap.
    /// </summary>
    public class TreeLayoutNodeArranger
    {
        /// <summary>
        /// Max iterations taken when making sure nodes are spaced correctly.
        /// </summary>
        private const int SPACING_ITERATIONS = 2150;

        #region -- Private Fields ---------------------------------------------
        /// <summary> Stores the first parent of each node. </summary>
        private readonly Dictionary<int, int> hierarchicalParents = new Dictionary<int, int>();

        /// <summary> Stores a list of children for each node. </summary>
        private readonly Dictionary<int, List<int>> hierarchicalChildren = new Dictionary<int, List<int>>();

        /// <summary> 
        /// Maps a node to the bounds of that node and all recorded tree 
        /// descendants.
        /// </summary>
        private readonly Dictionary<int, Rect> subtreeBounds = new Dictionary<int, Rect>();

        /// <summary>
        /// The outer list index is the depth into the tree and the inner list
        ///  contains the nodes at that depth.
        /// </summary>
        private readonly List<List<int>> depths = new List<List<int>>();

        /// <summary> The views that represent each node index. </summary>
        private readonly Dictionary<int, NodeViewState> views = new Dictionary<int, NodeViewState>();

        /// <summary> Visual config used to space the layout. </summary>
        private readonly GraphVisualConfig visuals;

        /// <summary> ID of the node at the root. </summary>
        private readonly int rootID;

        private bool vertical;
        #endregion -- Private Fields ------------------------------------------

        /// <summary>
        /// Create a new Tree Layout Node Arranger that arranges all 
        /// given nodes in a tree like structure.
        /// </summary>
        /// <param name="visuals">Config spcifying node spacing.</param>
        /// <param name="rootID">ID of the node at the root.</param>
        /// <param name="vertical">
        /// If true, the layout will be built vertically instead of
        /// horizontally.
        /// </param>
        public TreeLayoutNodeArranger(GraphVisualConfig visuals, int rootID, bool vertical = false)
        {
            this.visuals = visuals;
            hierarchicalParents.Add(rootID, -1);
            this.rootID = rootID;
            this.vertical = vertical;
        }

        /// <summary>
        /// Add a node to the arranger to be part of the new layout.
        /// </summary>
        /// <param name="id">ID of the node.</param>
        /// <param name="state">NodeViewState used for positioning.</param>
        /// <param name="children">IDs of all of the nodes children.</param>
        public void AddNode(int id, NodeViewState state, int[] children)
        {
            views.Add(id, state);
            hierarchicalChildren.Add(id, new List<int>());
            for (int i = 0; i < children.Length; i++)
            {
                if (hierarchicalParents.ContainsKey(children[i]))
                    continue;
                hierarchicalParents.Add(children[i], id);
                hierarchicalChildren[id].Add(children[i]);
            }
        }

        /// <summary>
        /// After adding all of the nodes of a graph using AddNode, this will 
        /// adjust all node placement such that no nodes overlap and that they 
        /// are in a tree like arrangment.
        /// </summary>
        /// <param name="container">Rect to fit the first node in.</param>
        public void GenerateLayout(Rect container)
        {
            if (vertical)
                views[rootID].Position = new Vector2(0, container.height/2.0f + visuals.NodeHeight/2.0f + visuals.NodeYSpace);
            else
                views[rootID].Position = new Vector2(visuals.NodeWidth + visuals.NodeXSpace, container.height / 2 - visuals.NodeHeight / 2);

            depths.Add(new List<int>());
            depths[0].Add(rootID);
            Dictionary<int, int> idToDepth = new Dictionary<int, int> { { rootID, 0 } };

            Queue<int> processQueue = new Queue<int>();

            bool completed = false;
            bool[] hits = new bool[views.Count];

            int current = rootID;
            while (!completed)
            {
                int treeChildrenCount = 0;
                List<int> children = hierarchicalChildren[current];

                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i] == -1 || hits[children[i]])
                        continue;
                    treeChildrenCount++;
                }
                //children.Clear();
                int used = 0;
                for (int i = 0; i < children.Count; i++)
                {
                    int childID = children[i];
                    // If the child has been visited, continue.
                    if (childID == -1 || hits[childID])
                        continue;
                    float offset = used - (treeChildrenCount - 1) / 2.0f;
                    used++;
                    offset = offset * visuals.NodeHeight + offset * visuals.NodeYSpace;
                    Vector2 parentPos = views[current].Position;
                    
                    if (vertical)
                        views[childID].Position = new Vector2(parentPos.x + offset, parentPos.y + visuals.NodeHeight + visuals.NodeYSpace);
                    else
                        views[childID].Position = new Vector2(parentPos.x + visuals.NodeWidth + visuals.NodeXSpace, parentPos.y + offset);

                    hits[childID] = true;

                    int depth = idToDepth[current];
                    while (depths.Count < depth)
                        depths.Add(new List<int>());
                    depths[depth].Add(childID);
                    idToDepth.Add(childID, depth);

                    processQueue.Enqueue(childID);
                }
                if (processQueue.Count > 0)
                    current = processQueue.Dequeue();
                else
                    completed = true;
            }

            SpaceNodes(rootID);
        }

        /// <summary>
        /// Space out all of the nodes of the graph until there is no more 
        /// overlap.
        /// </summary>
        /// <param name="rootIndex">Node to begin spacing with.</param>
        private void SpaceNodes(int rootIndex)
        {
            int iterationsRemaining = SPACING_ITERATIONS;
            GetSubtreeBounds(rootIndex);
            bool spaced = true;
            while (spaced && iterationsRemaining > 0)
            {
                bool acted = false;
                for (int i = depths.Count - 1; i >= 0; i--)
                {
                    foreach (int id in depths[i])
                    {
                        bool result = SpaceChildren(id);
                        if (result)
                            acted = true;
                    }
                }
                spaced = acted;
                iterationsRemaining--;
            }
            GetSubtreeBounds(rootIndex);
        }

        /// <summary>
        /// Find the bounds of a node plus all descendant nodes.
        /// </summary>
        /// <param name="id">ID of the node.</param>
        /// <returns>
        /// A rect that fits around the given node and all descendants.
        /// </returns>
        private Rect GetSubtreeBounds(int id)
        {
            NodeViewState view = views[id];
            subtreeBounds[id] = view.GetRect();
            List<int> children = hierarchicalChildren[id];
            if (children != null && children.Count > 0)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i] == -1)
                        continue;
                    Rect cb = GetSubtreeBounds(children[i]);
                    Rect newRect = new Rect();
                    newRect.xMax = Mathf.Max(subtreeBounds[id].xMax, cb.xMax);
                    newRect.yMax = Mathf.Max(subtreeBounds[id].yMax, cb.yMax);
                    newRect.yMin = Mathf.Min(subtreeBounds[id].yMin, cb.yMin);
                    newRect.xMin = Mathf.Min(subtreeBounds[id].xMin, cb.xMin);
                    subtreeBounds[id] = newRect;
                }
            }
            return subtreeBounds[id];
        }

        /// <summary>
        /// For a given node, space out all of it's children (and descendants) 
        /// such that there is no overlap.
        /// </summary>
        /// <param name="id">ID of the parent node.</param>
        /// <returns>
        /// True if any spacing has taken place, false if everything is 
        /// already spaced enough.
        /// </returns>
        private bool SpaceChildren(int id)
        {
            if (id == -1)
                return false;
            List<int> children = hierarchicalChildren[id];
            if (children == null || children.Count == 0)
                return false;
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] == -1)
                    continue;
                Rect firstBounds = GetSubtreeBounds(children[i]);
                for (int j = i + 1; j < children.Count; j++)
                {
                    if (children[j] == -1)
                        continue;
                    Rect secondBounds = GetSubtreeBounds(children[j]);
                    float overlap = vertical ? (firstBounds.xMax - secondBounds.xMin) :
                        firstBounds.yMax - secondBounds.yMin;
                    if (overlap >= (vertical?-visuals.NodeXSpace: -visuals.NodeYSpace) / 2.0f)
                    {
                        float amount = overlap / 2.0f;
                        amount += visuals.NodeXSpace;
                        float x = vertical ? amount : 0;
                        float y = vertical ? 0 : amount;

                        int c1 = children[i];
                        int c2 = children[j];

                        for(int o = 0; o <i; o++)
                            MoveSubtree(children[o], new Vector2(-x, -y));
                        MoveSubtree(c1, new Vector2(-x, -y));
                        MoveSubtree(c2, new Vector2(x, y));

                        for (int o = j+1; o < children.Count; o++)
                            MoveSubtree(children[o], new Vector2(x, y));

                        GetSubtreeBounds(c1);
                        GetSubtreeBounds(c2);
                        return true;
                    }                    
                }
            }         
            return false;
        }

        /// <summary>
        /// Moves a node and all of it's tree children.
        /// </summary>
        /// <param name="id">ID of the node to move.</param>
        /// <param name="move">Amount to move.</param>
        private void MoveSubtree(int id, Vector2 move)
        {
            if (id == -1)
                return;
            NodeViewState v = views[id];
            List<int> children = hierarchicalChildren[id];
            v.Position += move;
            for (int i = 0; i < children.Count; i++)
            {                
                MoveSubtree(children[i], move);
            }
        }
    }
}
