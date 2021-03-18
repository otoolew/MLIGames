// ----------------------------------------------------------------------------
//  Copyright © 2017 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   01/27/2017
// ----------------------------------------------------------------------------

using System;
using UnityEngine;

namespace SG.Core.Inspector
{
    /// <summary>
    /// When used on a field in the inspector, the field will not draw in 
    /// normal mode but will still show in debug. This may be used instead of 
    /// HideInInspector which will not show in debug mode.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class DebugOnlyAttribute : PropertyAttribute
    {}
}