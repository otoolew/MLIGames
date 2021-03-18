// ------------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 7/13/2016 2:13:34 PM
// ------------------------------------------------------------------------------

using System;
using SG.Core;

namespace SG.Dynamics
{
    public class DynamicsModuleInfo : ModuleInfo
    {
        /// <summary>
        /// Gets the module version
        /// </summary>
        public override Version Version
        {
            get { return new Version(1, 0, 0); }
        }

        /// <summary>
        /// Gets the name of the module.
        /// </summary>
        public override string Name
        {
            get { return "Dynamics"; }
        }

        /// <summary>
        /// Gets a short one or two sentence module description.
        /// </summary>
        public override string Description
        {
            get
            {
                return "DynamicValue. A flexible Generic yet Unity-serializable type. Supports primitive, Unity object, and complex serialized types.";
            }
        }

        /// <summary>
        /// Gets the URL to the module's tasks and bugs in Jira.
        /// </summary>
        public override Uri IssuesUrl
        {
            get
            {
                return new Uri("https://jira.schellgames.com:8443/browse/UF-261");
            }
        }

        /// <summary>
        /// Gets the URL to the module's documentation.
        /// </summary>
        public override Uri DocsUrl
        {
            get
            {
                return new Uri("https://docs.google.com/document/d/1g7oUgK7iMXy5y8A9kBwuDMMien3tqUphekfhJuV5mT0");
            }
        }
    }
}
