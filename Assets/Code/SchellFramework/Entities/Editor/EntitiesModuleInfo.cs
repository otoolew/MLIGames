// ------------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 7/13/2016 2:28:43 PM
// ------------------------------------------------------------------------------

using System;
using SG.Core;

namespace SG.Entities
{
    public class EntitiesModuleInfo : ModuleInfo
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
            get { return "Entities"; }
        }

        /// <summary>
        /// Gets a short one or two sentence module description.
        /// </summary>
        public override string Description
        {
            get
            {
                return "Entity, Tag & Binder method for organizing scene objects and referencing them from scripts.";
            }
        }

        /// <summary>
        /// Gets the URL to the module's tasks and bugs in Jira.
        /// </summary>
        public override Uri IssuesUrl
        {
            get
            {
                return new Uri("https://jira.schellgames.com:8443/browse/UF-262");
            }
        }

        /// <summary>
        /// Gets the URL to the module's documentation.
        /// </summary>
        public override Uri DocsUrl
        {
            get
            {
                return new Uri("https://docs.google.com/document/d/1Puu4sFcXADlgpEszA-6S8rWoWUX1HXLPaiJFBgIgqzo");
            }
        }
    }
}
