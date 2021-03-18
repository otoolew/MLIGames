//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   10/03/2014
//-----------------------------------------------------------------------------

using System;

namespace SG.Vignettitor.Graph.NodeViews
{
    /// <summary>
    /// Attribute to tag a class (that should inherit from NodeEditView) with 
    /// the node type that it provides an editor for.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class NodeEditViewAttribute : Attribute
    {
        /// <summary>
        /// The type of vignette node that a class can provide an editor for.
        /// </summary>
        public Type nodeType { get; private set; }

        /// <summary>
        /// Indicates that a class is used for editing a node.
        /// </summary>
        /// <param name="nodeType">
        /// The type of vignette node that a class can provide an editor for.
        /// Must inherit from VignetteNode.
        /// </param>
        public NodeEditViewAttribute(Type nodeType)
        {
            this.nodeType = nodeType;
        }
    }
}
