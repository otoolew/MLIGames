// ------------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 5/3/2016 1:55:03 PM
// ------------------------------------------------------------------------------

using UnityEngine;

namespace SG.Dynamics
{
    /// <summary>
    /// Mark a string field as desiring the property drawer for one of the DynamicTypes.
    /// Stores the AssemblyQualifiedName of the type in the underlying string field.
    /// </summary>
    public class DynamicTypeAttribute : PropertyAttribute { }
}
