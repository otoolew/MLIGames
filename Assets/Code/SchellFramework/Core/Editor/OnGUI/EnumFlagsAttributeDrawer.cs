// ------------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Eric Policaro
//
//  Created: August 2015
// ------------------------------------------------------------------------------

using UnityEngine;
using UnityEditor;

namespace SG.Core.OnGUI
{
    /// <summary>
    /// Draws a drawer for a [Flags] enum that can be manipulated like a bit mask.
    /// </summary>
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
        }
    }
}
