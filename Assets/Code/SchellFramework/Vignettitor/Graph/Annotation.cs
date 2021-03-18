//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   10/03/2014
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace SG.Vignettitor.Graph
{
    /// <summary>
    /// An annotation stored by a graph that can display user entered text.
    /// </summary>
    [System.Serializable]
    public class Annotation
    {
        public Annotation()
        {
             BoundNodes = new List<int>();
        }

        public Annotation(Annotation other)
        {
            Note = other.Note;
            Position = other.Position;
            ColorIndex = other.ColorIndex;
            BoundNodes = new List<int>(other.BoundNodes.ToArray());
            BoundPositionOffset = other.BoundPositionOffset;
            BoundSizeOffset = other.BoundSizeOffset;
        }

        /// <summary> Text of the note to store in the graph. </summary>
        public string Note = "";

        /// <summary>
        /// Position to place the annotation on the graph as well as scale.
        /// </summary>
        public Rect Position = new Rect(0, 0, 0, 0);

        /// <summary>
        /// Index of the color (from the config) to draw this annotation with.
        /// </summary>
        public int ColorIndex;

        /// <summary> Nodes that this annotation is bound to. </summary>
        public List<int> BoundNodes;

        /// <summary>
        /// If bound to nodes, the amount that the annotation is larger or 
        /// smaller than the bounds of the nodes.
        /// </summary>
        public Vector2 BoundSizeOffset;

        /// <summary>
        /// If bound to nodes, the amount that the annotation offset by the 
        /// bounds of the nodes.
        /// </summary>
        public Vector2 BoundPositionOffset;
    }
}
