//-----------------------------------------------------------------------------
// Copyright © 2014 Schell Games, LLC. All Rights Reserved.
//
// Contact: Tim Sweeney
//
// Created: Oct 2014
//-----------------------------------------------------------------------------

using System;

namespace SG.Vignettitor.VignetteData
{
    /// <summary>
    /// Attribute put on a vignette node that declares it may show up in the 
    /// "new node" menu and that defines the user facing name of the node.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class NodeMenuAttribute : Attribute
    {
        /// <summary>
        /// Slash separated path where this class should appear in the node 
        /// menu. Intended to mimic the attributes Unity uses to position 
        /// context menu items.
        ///  ex. "Data/Get Bool"
        /// </summary>
        public string MenuPath { get; private set; }

        /// <summary>
        /// The type of vignette graph that a node may be used in. This type
        /// must inherit from VignetteGraph.
        /// </summary>
        public Type VignetteGraphType { get; private set; }

        /// <summary>
        /// May be used to control the order nodes appear in the new node list.
        /// Higher priority items will appear towards the top of the list.
        /// Items with the same priority are sorted alphabetically.
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        /// Get the user facing name of this node, not including nesting. Think 
        /// of this like file name as opposed to file path.
        /// </summary>
        public string DisplayName 
        { get { return System.IO.Path.GetFileName(MenuPath); }}

        /// <summary>
        /// New NodeMenuAttribute, describing how a node is used and displayed.
        /// </summary>
        /// <param name="menuPath">
        /// Slash separated path where this class should appear in the node 
        /// menu. Intended to mimic the attributes Unity uses to position 
        /// context menu items.
        ///  ex. "Data/Get Bool"
        /// </param>
        public NodeMenuAttribute(string menuPath)
            : this(menuPath, typeof(VignetteGraph))
        {}

        /// <summary>
        /// New NodeMenuAttribute, describing how a node is used and displayed.
        /// </summary>
        /// <param name="menuPath">
        /// Slash separated path where this class should appear in the node 
        /// menu. Intended to mimic the attributes Unity uses to position 
        /// context menu items.
        ///  ex. "Data/Get Bool"
        /// </param>
        /// <param name="vignetteGraphType">
        /// The type of vignette graph that a node may be used in. This type
        /// must inherit from VignetteGraph.
        /// </param>
        public NodeMenuAttribute(string menuPath, Type vignetteGraphType)
            : this(menuPath, vignetteGraphType, 0)
        {}

        /// <summary>
        /// New NodeMenuAttribute, describing how a node is used and displayed.
        /// </summary>
        /// <param name="menuPath">
        /// Slash separated path where this class should appear in the node 
        /// menu. Intended to mimic the attributes Unity uses to position 
        /// context menu items.
        ///  ex. "Data/Get Bool"
        /// </param>
        /// <param name="vignetteGraphType">
        /// The type of vignette graph that a node may be used in. This type
        /// must inherit from VignetteGraph.
        /// </param>
        /// <param name="priority">
        /// May be used to control the order nodes appear in the new node list.
        /// Higher priority items will appear towards the top of the list.
        /// Items with the same priority are sorted alphabetically.
        /// </param>
        public NodeMenuAttribute(string menuPath, Type vignetteGraphType, int priority)
        {
            this.MenuPath = menuPath;
            this.VignetteGraphType = vignetteGraphType;
            this.Priority = priority;
        }
    }
}
