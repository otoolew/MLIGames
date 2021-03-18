//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   08/19/2015
//-----------------------------------------------------------------------------

using SG.Vignettitor.VignetteData;

namespace SG.Vignettitor.Runtime
{
    /// <summary>
    /// An exception indicating something unexpected happened while exeecuting 
    /// a vignette graph.
    /// </summary>
    public class VignetteRuntimeException : VignetteException
    {
        public VignetteRuntimeException(string message)
            : base(message)
        {}

        public VignetteRuntimeException(VignetteRuntimeGraph runtime, VignetteNode node, string message)
            : base(string.Concat(runtime.Source.name, " ", node.name, " [", node.NodeID, "] ", message))
        {}
    }
}
