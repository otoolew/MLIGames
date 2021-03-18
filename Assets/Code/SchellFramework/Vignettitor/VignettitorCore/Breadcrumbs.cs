// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Eric Policaro
// 
//  Date: 02/25/2016
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using SG.Vignettitor.VignetteData;

namespace SG.Vignettitor.VignettitorCore
{
    /// <summary>
    /// Provides a navigation trail to trace the user's path
    /// back through a set of vignette hierarchal graphs.
    /// </summary>
    public class Breadcrumbs
    {
        /// <summary>
        /// Initializes a new instance <see cref="Breadcrumbs"/>.
        /// </summary>
        public Breadcrumbs()
        {
            _stack = new Stack<VignetteGraph>();
        }

        /// <summary>
        /// Gets all currently tracked graphs, ordered by most
        /// recently accessed.
        /// </summary>
        public VignetteGraph[] Tracked
        {
            get { return _stack.ToArray(); }
        }

        /// <summary>
        /// Gets the most recently visited graph.
        /// </summary>
        public VignetteGraph Top
        {
            get { return AtRoot ? null : _stack.Peek(); }
        }

        /// <summary>
        /// Gets <c>true</c> if at navigation root.
        /// </summary>
        public bool AtRoot
        {
            get { return _stack.Count == 0; }
        }

        /// <summary>
        /// Clear all breadcrumbs, effectively making the current 
        /// graph the root.
        /// </summary>
        public void Clear()
        {
            _stack.Clear();
        }

        /// <summary>
        /// Adds a graph to the top of the breadcrumb stack.
        /// </summary>
        /// <param name="g">Graph to push onto the breadcrumb stack,
        /// if this is the same as the graph at the top, it will
        /// be ignored.</param>
        public void Push(VignetteGraph g)
        {
            if (!AtRoot && _stack.Peek() == g)
                return;

            _stack.Push(g);
        }

        /// <summary>
        /// Removes and returns the most recently visited graph.
        /// </summary>
        /// <returns>
        /// Graph at the top of the stack, or null if at the root.
        /// </returns>
        public VignetteGraph Pop()
        {
            return AtRoot ? null : _stack.Pop();
        }

        /// <summary>
        /// Remove a specified number of items from the navigation stack.
        /// </summary>
        /// <param name="count">
        /// Number of items to remove. If the count meets or exceeds 
        /// the stack height, all items are removed from the stack.
        /// </param>
        public void Unwind(int count)
        {
            if (count >= _stack.Count)
            {
                Clear();
                return;
            }

            for (int i = 0; i < count; i++)
            {
                if (AtRoot)
                    return;

                Pop();
            }
        }

        private readonly Stack<VignetteGraph> _stack;
    }
}