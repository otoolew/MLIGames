// ----------------------------------------------------------------------------
//  Copyright © 2017 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   01/27/2017
// ----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace SG.Core.Inspector
{
    [CustomPropertyDrawer(typeof(DebugOnlyAttribute))]
    public class DebugOnlyAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        { 
            // subtract the spacing used between fields.
            return -EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {}
    }
}