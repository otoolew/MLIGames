// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Eric Policaro
// 
//  Date: 03/09/2016
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using SG.Core.Collections;
using SG.Vignettitor.VignetteData;

namespace SG.Vignettitor.VignettitorCore
{
    /// <summary>
    /// Tracks the most recently visited vignette graphs.
    /// </summary>
    public class History
    {
        /// <summary>
        /// Default history size.
        /// </summary>
        public const int DEFAULT_MAX_HISTORY = 5;

        /// <summary>
        /// Initializes a new instance of <see cref="History"/>
        /// with the default history length.
        /// </summary>
        public History() : this(DEFAULT_MAX_HISTORY) { }

        /// <summary>
        /// Initializes a new instance of <see cref="History"/>.
        /// </summary>
        /// <param name="max">Maximum number of items to track</param>
        public History(int max)
        {
            _history = new RecentsList<VignetteGraph>(max);
        }

        /// <summary>
        /// Gets an array of all history items with the most 
        /// recently visited graph listed virst (position 0).
        /// </summary>
        public VignetteGraph[] GetAll()
        {
            var result = new List<VignetteGraph>();
            for (int i = 0; i < _history.Length; i++)
            {
                result.Add(_history[i]);
            }
            result.Reverse();
            return result.ToArray();
        }

        /// <summary>
        /// Gets the <see cref="VignetteGraph"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="VignetteGraph"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public VignetteGraph this[int index]
        {
            get { return _history[index]; }
        }

        /// <summary>
        /// Gets the number of items in the history.
        /// </summary>
        public int Length
        {
            get { return _history.Length; }
        }

        /// <summary>
        /// Adds a graph instance to the history
        /// </summary>
        /// <param name="g">Graph to add</param>
        public void Add(VignetteGraph g)
        {
            _history.Add(g);
        }

        /// <summary>
        /// Determines whether the history contains a graph
        /// </summary>
        /// <param name="g">Graph to look for</param>
        /// <returns>
        /// <c>True</c> if the history contains the specified graph
        /// </returns>
        public bool Contains(VignetteGraph g)
        {
            return _history.Contains(g);
        }

        private readonly RecentsList<VignetteGraph> _history;
    }
}