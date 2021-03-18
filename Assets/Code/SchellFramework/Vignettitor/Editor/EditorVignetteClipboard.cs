//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   09/09/2014
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using SG.Core.Contracts;
using SG.Vignettitor.Graph;
using SG.Vignettitor.VignetteData;
using SG.Vignettitor.VignettitorCore;
using UnityEditor;
using UnityEngine;

namespace SG.Vignettitor.Editor
{
    /// <summary>
    /// Performs copy, paste, duplicate, etc operations on a vignette graph. 
    /// This stores copy data so that it may be pasted to a different graph.
    /// </summary>
    public class EditorVignetteClipboard : IVignetteClipboard
    {
        /// <summary>
        /// Default slot for the clipboard. Copy and paste operations use this.
        /// </summary>
        public VignetteClipboardData DefaultSlot { get; set; }

        /// <summary>
        /// Does the clipboard have data to perform a paste operation?
        /// </summary>
        /// <returns>True if Paste will perform an action.</returns>
        public bool HasPasteData()
        { return DefaultSlot != null; }

        /// <summary>Copy the given nodes to the clipboard.</summary>
        /// <param name="nodes">Nodes to copy.</param>
        /// <param name="positions">Placement of nodes in the nodes list.</param>
        /// <param name="annotations">Annotations to copy.</param>
        /// <param name="dataController">
        /// The data controller that may be used to access vignette context.
        /// </param>
        public void Copy(List<VignetteNode> nodes, List<Vector2> positions, List<Annotation> annotations, VignettitorDataController dataController)
        {
            // TODO: verify that positions and nodes are the same length.
            DefaultSlot = new VignetteClipboardData("default", nodes, positions, annotations, dataController);
        }

        /// <summary>
        /// Paste the nodes from the last copy operation, saving them into the 
        /// specified data controller.
        /// </summary>
        /// <param name="dataController">
        /// Data controller used to save the nodes that are being pasted.
        /// </param>
        /// <returns>
        /// Paste result defining the nodes created and where they should be.
        /// </returns>
        public VignettePasteResult Paste(VignettitorDataController dataController)
        {
            return Paste(dataController, DefaultSlot);
        }

        /// <summary>
        /// Paste the nodes from the last copy operation, saving them into the 
        /// specified data controller.
        /// </summary>
        /// <param name="dataController">
        /// Data controller used to save the nodes that are being pasted.
        /// </param>
        /// <param name="clipboard">The clipboard data to paste.</param>
        /// <returns>
        /// Paste result defining the nodes created and where they should be.
        /// </returns>
        public VignettePasteResult Paste(VignettitorDataController dataController, VignetteClipboardData clipboard)
        {
            VignettePasteResult result = null;
            if (clipboard != null)
            {
                result = new VignettePasteResult();
                                
                for (int i = 0; i < clipboard.NodeCopyList.Count; i++)
                {
                    VignetteNode source = clipboard.NodeCopyList[i].Node;                    
                    result.Nodes.Add(Object.Instantiate(source));
                    result.Nodes[i].name = source.name;
                    result.Nodes[i].Children = new VignetteNode[clipboard.NodeCopyList[i].Connections.Count];
                    result.Positions.Add(clipboard.NodeCopyList[i].Position);
                }

                for (int i = 0; i < clipboard.AnnotationCopyList.Count; i++)
                    result.Annotations.Add(new Annotation(clipboard.AnnotationCopyList[i]));

                // Once all nodes are created, loop through again to assign connections.
                for (int i = 0; i < clipboard.NodeCopyList.Count; i++)
                {
                    NodeCopyData copyData = clipboard.NodeCopyList[i];
                    for (int ci = 0; ci < copyData.Connections.Count; ci++)
                    {
                        int clipID = copyData.Connections[ci];
                        if (clipID != -1)
                            result.Nodes[i].Children[ci] = result.Nodes[clipID];
                    }

                    // This second loop is only here to clear out any null 
                    // children since it is currently not allowed. This may be 
                    // removed once output pins are implemented and the graph 
                    // handles null outputs.

                    for (int ci = result.Nodes[i].Children.Length - 1; ci >= 0; ci--)
                    {
                        if (result.Nodes[i].Children[ci] == null)
                            ArrayUtility.RemoveAt(ref result.Nodes[i].Children, ci);
                    }

                    SerializedObject dest = new SerializedObject(result.Nodes[i]);
                    SubordinateAssetUtility.PopulateSubordinateAssets(copyData.SubordinateAssetMap, dest, dataController.head);

                    dataController.AddNode(result.Nodes[i]);

                    INodeCopyPasteHandler copyPasteHandler = result.Nodes[i] as INodeCopyPasteHandler;
                    if (copyPasteHandler != null)
                        copyPasteHandler.OnDidPaste(clipboard.DataController, dataController);
                    
                    Undo.RegisterCreatedObjectUndo(result.Nodes[i], "Paste");
                }
            }
            return result;
        }

        /// <summary>
        /// Duplicate the given nodes into the given data controller.
        /// </summary>
        /// <param name="nodes">Nodes to duplicate.</param>
        /// <param name="positions">placement of nodes that are being duplicated.</param>
        /// <param name="annotations">Annotations to duplicate.</param>
        /// <param name="dataController">
        /// Data controller used to save the nodes that are being duplicated.
        /// </param>
        /// <returns>
        /// Paste result defining the nodes created and where they should be.
        /// </returns>
        public VignettePasteResult Duplicate(List<VignetteNode> nodes, List<Vector2> positions, List<Annotation> annotations, VignettitorDataController dataController)
        {
            // TODO: verify that positions and nodes are the same length.
            VignetteClipboardData clipboard = 
                new VignetteClipboardData("DuplicationClipboard", nodes, positions, annotations, dataController);
            return Paste(dataController, clipboard);
        }

        public void DrawClipboardPreview()
        {
            for (int i = 0; i < DefaultSlot.NodeCopyList.Count; i++)
            {
                for (int j = 0; j < DefaultSlot.NodeCopyList[i].Connections.Count; j++)
                {
                    GUILayout.Label(i + " " +
                        j + " " +
                        DefaultSlot.NodeCopyList[i].Connections[j]);
                }
            }
        }
    }
}
