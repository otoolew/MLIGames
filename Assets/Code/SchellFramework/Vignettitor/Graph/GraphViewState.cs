//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   05/15/2014
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace SG.Vignettitor.Graph
{
    /// <summary>
    /// Stores and saves the view state of a vigettitor graph, including each 
    /// node position and scroll and pan.
    /// </summary>
    public class GraphViewState : ScriptableObject
    {
        /// <summary> Graph view position. </summary>
        public Vector2 Scroll = Vector2.zero;

        /// <summary> Zoom in to the graph. </summary>
        public float Zoom = 1.0f;

        /// <summary> List tracking positions for all nodes. </summary>
        public List<NodeViewState> NodeViewStates = new List<NodeViewState>();

        /// <summary> Position for the head node. </summary>
        public NodeViewState HeadViewState;

        /// <summary> List of annotations stored in the graph. </summary>
        public List<Annotation> Annotations = new List<Annotation>();
    }
}
