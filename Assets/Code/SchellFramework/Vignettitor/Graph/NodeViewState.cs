//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   05/08/2014
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using SG.Vignettitor.Graph.NodeViews;
using UnityEngine;
using UnityEngine.Serialization;

namespace SG.Vignettitor.Graph
{
    /// <summary>
    /// Information about a node that is used for display purposes. Some of the 
    /// data is serialized to disk and some is only used during runtime.
    /// </summary>
    [Serializable]
    public class NodeViewState
    {
        #region -- Serialized Fields ------------------------------------------
        /// <summary>Graph position of this node.</summary>
        [FormerlySerializedAs("position")]
        public Vector2 Position;

        /// <summary> ID for this node that is unique per graph. </summary>
        [FormerlySerializedAs("id")]
        public int ID = -1;
        #endregion -- Serialized Fields ---------------------------------------

        #region -- Runtime Fields ---------------------------------------------
        /// <summary>Scale to draw this node.</summary>
        [NonSerialized]
        public Vector2 scale = new Vector2(300, 220);

        /// <summary>Rect where the node will draw in screen space.</summary>
        [NonSerialized]
        public Rect renderRect = new Rect(0, 0, 300, 220);

        [NonSerialized]
        public ContentValidation validation = new ContentValidation();

        /// <summary>The NodeView to draw this node.</summary>
        [NonSerialized]
        public NodeView nodeView;

        /// <summary>
        /// The NodeEditView to draw/edit this node. 
        /// (Supplied only via Editor assemblies)
        /// </summary>
        [NonSerialized]
        public NodeEditView nodeEditView;
        #endregion -- Runtime Fields ------------------------------------------

        #region -- Public Methods ---------------------------------------------
        /// <summary>Gets a graph space rectangle for this node.</summary>
        /// <returns>The nodes rect.</returns>
        public Rect GetRect()
        { return new Rect(Position.x, Position.y, scale.x, scale.y); }

        /// <summary>
        /// Gets the labels to use on this nodes connections or outputs based 
        /// on the nodeView for the node.
        /// </summary>
        /// <param name="c">Index of the output to get a label for.</param>
        /// <returns>Label text to draw.</returns>
        public string GetConnectionLabel(int c)
        {
            if (nodeView != null)
                return nodeView.GetConnectionLabel(c);
            return NodeView.DefaultConnectionLabel(c);
        }
        #endregion -- Public Methods ------------------------------------------
    }

    /// <summary>
    /// Provides a way to sort View states in a list so that they are ordered 
    /// by the ID of the node they represent.
    /// </summary>
    public class ViewStateComparer : IComparer<NodeViewState>
    {
        public int Compare(NodeViewState x, NodeViewState y)
        { return x.ID - y.ID; }
    }
}
