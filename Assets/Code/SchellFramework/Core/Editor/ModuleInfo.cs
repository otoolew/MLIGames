// ------------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Eric Policaro
//
//  Created: 8/26/2015 6:07:00 PM
// ------------------------------------------------------------------------------

using System;

namespace SG.Core
{
    public abstract class ModuleInfo
    {
        /// <summary>
        /// Gets the module version
        /// </summary>
        public abstract Version Version { get; }

        /// <summary>
        /// Gets the name of the module.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets a short one or two sentence module description.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Gets the URL to the module's tasks and bugs in Jira.
        /// </summary>
        public abstract Uri IssuesUrl { get; }

        /// <summary>
        /// Gets the URL to the module's documentation.
        /// </summary>
        public abstract Uri DocsUrl { get; }
    }
}
