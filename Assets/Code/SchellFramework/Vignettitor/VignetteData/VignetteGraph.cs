//-----------------------------------------------------------------------------
//  Copyright © 2013 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   01/24/2013
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using SG.Core;
using UnityEngine;

namespace SG.Vignettitor.VignetteData
{
    /// <summary>
    /// A Vignette Graph is the container object for vignette nodes as well as
    /// metadata for a vignette. This references the entry node of a vignette.
    /// </summary>
    [CreateAssetMenu(fileName = "NewVignetteGraph.asset", menuName = "Script/Vignette")]
    public class VignetteGraph : ScriptableObject 
    {
        private static readonly Notify Log = NotifyManager.GetInstance<VignetteGraph>();

        #region -- Inspector Variables ----------------------------------------
        /// <summary>The entry point for the vignette graph.</summary>
        public VignetteNode Entry;

        /// <summary>
        /// Indicates the version of the save data that was last used to save 
        /// this vignette. This way, there can be defined upgrage steps if data
        /// significantly changes in later versions.
        /// </summary>
        public int SaveFormatVersion = 0;

        [SerializeField]
        private GraphSignature _signature;
        public GraphSignature Signature { get { return _signature; } }
        #endregion -- Inspector Variables -------------------------------------

        #region -- Public Variables -------------------------------------------
        /// <summary>
        /// A unique human-readable identifier for a vignette. This mirrors the
        /// path that the vignette can be found at on disk. Ex:
        /// Year01/Season01/DefaultVignettes/VignetteTests/VignetteName_Graph
        /// This is intended to filled in by the VignetteAssetPostprocessor or 
        /// the VignetteHeadInspector
        /// </summary>
        [HideInInspector]
        public string VignettePath;

        /// <summary>
        /// A persistent internal identifier for a vignette. This is the GUID 
        /// that Unity uses to reference the vignette.  This is intended to 
        /// filled in by the VignetteAssetPostprocessor or the 
        /// VignetteHeadInspector.
        /// </summary>
        [HideInInspector]
        public string VignetteGUID;

        /// <summary>All nodes under the entry node.</summary>
        [SerializeField]
        public List<VignetteNode> allNodes;
        #endregion -- Public Varaibles ----------------------------------------
        
        /// <summary>
        /// Get the index of the given node in the graphs list of connected 
        /// nodes.
        /// </summary>
        /// <param name="node">Node to look for.</param>
        /// <returns>
        /// The index of the node or -1 if it is not conencted to the graph.
        /// </returns>
        public int GetIndex(VignetteNode node)
        {
            // TODO: make more efficient - maybe use a dictionary lookup
            for (int i = 0; i < allNodes.Count; i++)
                if (allNodes[i] == node)
                    return i;
            return -1;
        }


        public VignetteNode this[int nodeId]
        {
            get
            {
                for (int i = 0; i < allNodes.Count; i++)
                    if (allNodes[i].NodeID == nodeId)
                        return allNodes[i];
                Log.Error(this, "Couldn't locate node with id {0}", nodeId);
                return null;
            }
        }

        /// <summary>
        /// Gets the first identifiable path to the given node. 
        /// </summary>
        /// <param name="node">Node to find a path for.</param>
        /// <returns>
        /// The list of nodes on the path to the specified node. If null is 
        /// returned, that means that there is no path to the node. This is the 
        /// case if a node has no path from the graph root.
        /// </returns>
        public List<VignetteNode> GetPath(VignetteNode node)
        {
            List<VignetteNode> result = new List<VignetteNode>();
            // Fill in -1 for parents to see what is not hit.
            int[] parents = new int[allNodes.Count];
            for (int i = 0; i < parents.Length; i++)
                parents[i] = -1;

            for (int i = 0; i < allNodes.Count; i++)
            {
                int[] children = new int[allNodes[i].Children.Length];
                for (int c = 0; c < children.Length; c++)
                {
                    children[c] = GetIndex(allNodes[i].Children[c]);
                    if (children[c] != -1 && parents[children[c]] == -1)
                        parents[children[c]] = i;
                }
            }

            VignetteNode current = node;
            while (current != null)
            {
                result.Add(current);
                int currentIndex = GetIndex(current);
                if (currentIndex == -1)
                {
                    return null;
                }
                int pi = parents[currentIndex];
                if (pi == -1)
                    break;

                VignetteNode parent = allNodes[pi];
                // set current to parent
                current = parent;
            }
            result.Reverse();
            return result;
        }

        /// <summary>
        /// Returns true if any node in the graph performs any functions that 
        /// take more than a frame to complete.
        /// </summary>
        public bool TakesTime
        {
            get
            {
                if (allNodes == null || allNodes.Count == 0)
                    CollectConnectedNodes();

                if (allNodes != null)
                {
                    foreach (VignetteNode vn in allNodes)
                    {
                        try
                        {
                            if (vn.TakesTime)
                                return true;
                        }
                        catch (NullReferenceException ex)
                        {
                            if (vn != null)
                            {
                                Debug.LogError(
                                    string.Format(
                                        "There was a problem with the VignetteNode of type {0}.",
                                        vn.GetType()
                                    )
                                );
                            }

                            throw new NullReferenceException("There was a problem with an VignetteNode.", ex);
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Gets a list of all nodes that are of the given type or interface.
        /// </summary>
        /// <param name="t">
        /// The type (should inherit from VignetteNode) or the interface to 
        /// look for.
        /// </param>
        /// <param name="inherit">
        /// Should nodes that inherit from the supplied type be returned as 
        /// well as nodes of the type. If an interface is supplied as the type, 
        /// this is ignored.
        /// </param>
        /// <returns>A list of nodes that are of the given type.</returns>
        public List<VignetteNode> GetNodesOfType(Type t, bool inherit)
        {
            if (allNodes == null || allNodes.Count == 0)
                CollectConnectedNodes();

            List<VignetteNode> result = new List<VignetteNode>();

            if (allNodes != null)
            {
                for (int index = 0; index < allNodes.Count; index++)
                {
                    VignetteNode vn = allNodes[index];

                    if (!t.IsInstanceOfType(vn))
                        continue;

                    if (inherit || t.IsInterface)
                        result.Add(vn);
                    else if (!vn.GetType().IsSubclassOf(t))
                        result.Add(vn);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets all of the nodes used in this vignette.
        /// </summary>
        /// <returns>A list of vignette nodes.</returns>
        public VignetteNode[] GetAllNodes()
        {
            if (allNodes == null)
                return new VignetteNode[0];
            return allNodes.ToArray();
        }

        /// <summary>
        /// Steps through the Entry node and builds a list of all nodes and 
        /// all vignette interaction nodes used in this vignette graph.
        /// </summary>
        public virtual void CollectConnectedNodes()
        {
            allNodes = new List<VignetteNode>();
            Queue<VignetteNode> processQueue = new Queue<VignetteNode>();
            processQueue.Enqueue(Entry);
            while (processQueue.Count > 0)
            {
                VignetteNode current = processQueue.Dequeue();
                while (current != null)
                {
                    allNodes.Add(current);

                    for (int i = 0; i < (current.Children == null?0:current.Children.Length); i++)
                    {
                        if (allNodes.Contains(current.Children[i]) || processQueue.Contains(current.Children[i]))
                            continue;
                        processQueue.Enqueue(current.Children[i]);
                    }
                    current = processQueue.Count > 0 ? processQueue.Dequeue() : null;
                }
            }
        }
    }
}
