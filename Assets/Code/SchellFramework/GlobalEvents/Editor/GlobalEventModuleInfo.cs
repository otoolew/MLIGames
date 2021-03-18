// ----------------------------------------------------------------------------
//  Copyright © 2017 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   01/03/2017
// ----------------------------------------------------------------------------

using System;
using SG.Core;

namespace SG.GlobalEvents.Editor
{
    public class GlobalEventModuleInfo : ModuleInfo
    {
        /// <summary>
        /// Gets the module version
        /// </summary>
        public override Version Version
        {
            get { return new Version(1, 3, 0); }
        }

        /// <summary>
        /// Gets the name of the module.
        /// </summary>
        public override string Name
        {
            get { return "GlobalEvents"; }
        }

        /// <summary>
        /// Gets a short one or two sentence module description.
        /// </summary>
        public override string Description
        {
            get
            {
                return "Create event assets that can be raised, responded " +
                       "to, and debugged without any code implementation.";
            }
        }

        /// <summary>
        /// Gets the URL to the module's tasks and bugs in Jira.
        /// </summary>
        public override Uri IssuesUrl
        {
            get
            {
                return new Uri("https://jira.schellgames.com:8443/browse/UF-281?filter=12210");
            }
        }

        /// <summary>
        /// Gets the URL to the module's documentation.
        /// </summary>
        public override Uri DocsUrl
        {
            get
            {
                return new Uri("https://drive.google.com/open?id=1GomlZ6mlz_XTMJOltoBFbBQojK5nS5Q8BcIJbADcQ2U");
            }
        }
    }
}