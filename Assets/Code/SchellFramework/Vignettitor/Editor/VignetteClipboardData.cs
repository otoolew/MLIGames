//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   09/09/2014
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using SG.Core;
using SG.Core.Contracts;
using SG.Vignettitor.Graph;
using SG.Vignettitor.VignetteData;
using SG.Vignettitor.VignettitorCore;
using UnityEditor;
using UnityEngine;

namespace SG.Vignettitor.Editor
{
    /// <summary>
    /// Data stored by a vignette clipboard about each node that may be copied.
    /// </summary>
    public class NodeCopyData
    {
        /// <summary>The node that may be instantiated.</summary>
        public VignetteNode Node;

        /// <summary>
        /// A list of clipboard IDs to represent all nodes in this clipboard 
        /// that are child nodes of this node. -1 Indicates that the node is 
        /// not in the clipboard.
        /// </summary>
        public List<int> Connections = new List<int>();

        /// <summary>
        /// A mapping of property paths to ISubordinateAsset objects that are
        /// stored on the node.
        /// </summary>
        public SerializedPropertyMap<ISubordinateAsset> SubordinateAssetMap;

        /// <summary>Position of the node in the graph.</summary>
        public Vector2 Position;
    }

    /// <summary>
    /// Stores data about vignette nodes that may be pasted into a graph.
    /// </summary>
    public class VignetteClipboardData
    {
        /// <summary>Name of this clipboard.</summary>
        public string Name;

        /// <summary>
        /// List of data about each node that is in the clipboard.
        /// </summary>
        public List<NodeCopyData> NodeCopyList;

        /// <summary>
        /// The annotations that may be pasted. The bound IDs for each 
        /// annotation map to the clipboard ID of each node and not the actual
        /// ID so it needs to be converted after each paste operation.
        /// </summary>
        public List<Annotation> AnnotationCopyList;

        /// <summary>
        /// The data controller that may be used to access vignette context.
        /// This will hold a reference to the vignette graph and all nodes.
        /// </summary>
        public VignettitorDataController DataController;

        /// <summary>
        /// Make a new VignetteClipboardData object with a name and nodes. 
        /// This will make copies of each node so that they may be pasted in 
        /// the graph.
        /// </summary>
        /// <param name="name">Name to assign to the clipboard.</param>
        /// <param name="nodes">Nodes to add to the clipboard.</param>
        /// <param name="positions">Placement of the nodes.</param>
        /// <param name="annotations">Annotations to be copied.</param>
        /// <param name="dataController">
        /// The data controller that may be used to access vignette context.
        /// </param>
        public VignetteClipboardData(string name, List<VignetteNode> nodes, List<Vector2> positions, List<Annotation> annotations, VignettitorDataController dataController)
        {
            // TODO: verify that positions and nodes are the same length.
            Name = name;
            DataController = dataController;
            AnnotationCopyList = new List<Annotation>();
            for (int i = 0; i < annotations.Count; i++)
            {
                Annotation newNote = new Annotation(annotations[i]);
                // Map the bound nodes of each annotation to the clipboard ID 
                // of the nodes instead of their graph ID.
                if (annotations[i].BoundNodes != null)
                {
                    for (int n = 0; n < annotations[i].BoundNodes.Count; n++)
                    {
                        for (int ni = 0; ni < nodes.Count; ni++)
                        {
                            if (nodes[ni].NodeID == annotations[i].BoundNodes[n])
                            {
                                newNote.BoundNodes[n] = nodes.IndexOf(nodes[ni]);
                                break;
                            }
                        }
                    }
                }
                AnnotationCopyList.Add(newNote);
            }

            NodeCopyList = new List<NodeCopyData>();
            for (int i = 0; i < nodes.Count; i++)
            {
                INodeCopyPasteHandler copyPasteHandler = nodes[i] as INodeCopyPasteHandler;
                if (copyPasteHandler != null)
                    copyPasteHandler.OnWillCopy();

                NodeCopyData copyData = new NodeCopyData();         
                for (int ci = 0; ci < nodes[i].Children.Length; ci++)
                {
                    // If the connection is in the copy buffer, save it.
                    int clipID = nodes.IndexOf(nodes[i].Children[ci]);
                    copyData.Connections.Add(clipID);
                }

                VignetteNode node = Object.Instantiate(nodes[i]);
                node.name = nodes[i].name;

                copyPasteHandler = node as INodeCopyPasteHandler;
                if (copyPasteHandler != null)
                    copyPasteHandler.OnDidCopy();

                copyData.Node = node;
                copyData.Position = positions[i];

                SerializedObject so = new SerializedObject(nodes[i]);

                copyData.SubordinateAssetMap =
                    SubordinateAssetUtility.GetSubordinateAssetCopies(so.GetIterator());

                NodeCopyList.Add(copyData);
            }            
        }
    }
}