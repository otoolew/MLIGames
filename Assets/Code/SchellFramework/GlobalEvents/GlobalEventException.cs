// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   12/14/2016
// ----------------------------------------------------------------------------

using System;

namespace SG.GlobalEvents
{
    /// <summary>
    /// Base exception for any exceptions raised by the GlobalEvent system.
    /// </summary>
    public class GlobalEventException : Exception
    {
        public GlobalEventException() {}

        public GlobalEventException(string message) : base(message) {}

        public GlobalEventException(string message, Exception inner)
            : base(message, inner) {}
    }

    /// <summary>
    /// Exception raised when there is a problem with the argument passed with
    /// a GlobalEvent.
    /// </summary>
    public class GlobalEventArgumentException : GlobalEventException
    {
        public GlobalEventArgumentException() {}

        public GlobalEventArgumentException(string message) : base(message) {}

        public GlobalEventArgumentException(string message, Exception inner)
            : base(message, inner) {}
    }

    /// <summary>
    /// Exception raised when there is a problem with a GlobalEventListener.
    /// </summary>
    public class GlobalEventListenerException : GlobalEventException
    {
        public GlobalEventListenerException() {}

        public GlobalEventListenerException(string message) : base(message) {}

        public GlobalEventListenerException(string message, Exception inner)
            : base(message, inner) {}
    }

    /// <summary>
    /// Exception raised when a deprecated event is being illegally used.
    /// </summary>
    public class GlobalEventDeprecationException : GlobalEventException
    {
        public GlobalEventDeprecationException() { }

        public GlobalEventDeprecationException(string message) : base(message) { }

        public GlobalEventDeprecationException(string message, Exception inner)
            : base(message, inner) { }
    }
}