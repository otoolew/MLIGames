// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Eric Policaro
// 
//  Date: 03/09/2016
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace SG.Vignettitor.VignettitorCore
{
    /// <summary>
    /// Keeps track of breadcrumbs for each type of vignette graph. 
    /// This allows navigation among graph hierarchies of the same type
    /// but prevents a graph's navigation from being polluted with
    /// different graph types.
    /// </summary>
    public class BreadcrumbManager : UtilityManager<Breadcrumbs>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="BreadcrumbManager"/>.
        /// </summary>
        public BreadcrumbManager() : base(typeof(Breadcrumbs))
        {
        }
    }
}