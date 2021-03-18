//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   08/19/2015
//-----------------------------------------------------------------------------

using JetBrains.Annotations;

namespace SG.Vignettitor.Runtime
{
    /// <summary>
    /// An exception indicating an issue with the data format in a vignette 
    /// graph. VignetteDataExceptions are problems that can generally be 
    /// resolved by content creators rather than by changing code.
    /// </summary>
    public class VignetteDataException : VignetteException
    {
        public VignetteDataException(string message)
            : base(message)
        {}

        [StringFormatMethod("message")]
        public VignetteDataException(string message, params object[] args)
            : base(message, args)
        { }
    }
}
