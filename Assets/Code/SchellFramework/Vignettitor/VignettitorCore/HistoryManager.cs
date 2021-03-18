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
    /// Provides a history of recent graph navigation for
    /// each vignette graph type.
    /// </summary>
    public class HistoryManager : UtilityManager<History>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HistoryManager"/>.
        /// </summary>
        public HistoryManager() : base(typeof(History))
        {
        }
    }
}