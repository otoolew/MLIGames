// ------------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Eric Policaro
//
//  Created: 8/26/2015 6:07:27 PM
// ------------------------------------------------------------------------------

using System;

namespace SG.Core
{
    public class CoreInfo : ModuleInfo
    {
        /// <summary>
        /// Gets the module version
        /// </summary>
        public override Version Version
        {
            get { return new Version(1, 11, 0); }
        }

        /// <summary>
        /// Gets the name of the module.
        /// </summary>
        public override string Name
        {
            get { return "Core"; }
        }

        /// <summary>
        /// Gets a short one or two sentence module description.
        /// </summary>
        public override string Description
        {
            get
            {
                return "Base module for the Unity Framework. " + 
                       "Provides general purpose Unity functionality as well as services to modules.";
            }
        }

        /// <summary>
        /// Gets the URL to the module's tasks and bugs in Jira.
        /// </summary>
        public override Uri IssuesUrl
        {
            get
            {
                return new Uri("https://jira.schellgames.com:8443/browse/UF-21?filter=11311");
            }
        }

        /// <summary>
        /// Gets the URL to the module's documentation.
        /// </summary>
        public override Uri DocsUrl
        {
            get
            {
                return new Uri("https://docs.google.com/document/d/1gJ3RF1f0WpzQxMgU4NBz1EbL1yKLaSaYl17BF1_HCdo/edit#heading=h.tniey84jixfn");
            }
        }
    }
}
