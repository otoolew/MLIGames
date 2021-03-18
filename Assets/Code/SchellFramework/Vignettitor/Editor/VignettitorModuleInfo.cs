// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Eric Policaro
// 
//  Date: 02/03/2016
// ----------------------------------------------------------------------------

using SG.Core;
using System;

namespace SG.Vignettitor
{
    public class VignettitorModuleInfo : ModuleInfo
    {
        public override Version Version
        {
            // In order to runtime accessible, the actual version number is 
            // stored in a runtime class. Make changes to the version there.
            get { return VignettitorVersion.VIGNETTITOR_VERSION; }
        }

        public override string Name
        {
            get { return "Vignettitor"; }
        }

        public override string Description
        {
            get { return "Graph editor and data format that can be used for various applications like cinematics or visual scripting."; }
        }

        public override Uri IssuesUrl
        {
            get { return new Uri("https://jira.schellgames.com:8443/issues/?filter=11350"); }
        }

        public override Uri DocsUrl
        {
            get { return new Uri("https://docs.google.com/document/d/17rS7X7E50sCL8o9qcbna-CPcHVVrY_xEkwSjXJh0wrk"); }
        }
    }
}