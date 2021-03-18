//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   08/19/2015
//-----------------------------------------------------------------------------

using System;
using JetBrains.Annotations;
using SG.Vignettitor.VignetteData;

namespace SG.Vignettitor.Runtime
{
    /// <summary>
    /// An exception indicating an issue with a vignette or a vignette 
    /// execution.
    /// </summary>
    public class VignetteException : Exception
    {
        public VignetteException(string message)
            : base(message)
        {}

        [StringFormatMethod("message")]
        public VignetteException(string message, params object[] args)
            : base(string.Format(message, args))
        { }
    }
	
	/// <summary>
    /// An exception indicating an issue with a specific node in a vignette.
    /// </summary>
	public class VignetteNodeException : VignetteException
    {
        public VignetteNode Node { get; private set; }

        public VignetteNodeException(VignetteNode node, IGraphResolver resolver, string message)
            : base(FormatMessage(node, resolver, message))
        {
            Node = node;
        }

        public static string FormatMessage(VignetteNode node, IGraphResolver resolver, string message)
        {
            return string.Concat(resolver.DebugPath, " [", node.NodeID, "] : ", message);
        }
    }
}
