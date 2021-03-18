// ------------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//  Contact: Ryan Hipple
//  Created: 10/16/2015 1:08:19 AM
// ------------------------------------------------------------------------------

using System;

namespace SG.Vignettitor.VignetteData
{
    /// <summary>
    /// Attribute used to add help text to a vignette node that can be shown to
    ///  a user via various help buttons in Vignettitor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class NodeHelpAttribute : Attribute
    {
        /// <summary>
        /// Designer-facing node description, describing what a node does and 
        /// how to use it.
        /// </summary>
        public string HelpText;

        /// <summary> Create a new NodeHelp item. </summary>
        /// <param name="help">
        /// Designer-facing node description, describing what a node does and 
        /// how to use it.
        /// </param>
        public NodeHelpAttribute(string help)
        {
            HelpText = help;
        }
    }
}
