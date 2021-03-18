//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   06/17/2014
//-----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace SG.Core.OnGUI
{
    /// <summary>
    /// Draws an integer range on a single line and enforces minimum being less 
    /// than or equal to maximum.
    /// </summary>
    [CustomPropertyDrawer(typeof(IntRange))]
    public class IntRangeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return DrawHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUIUtility.labelWidth = 60.0f;
            EditorGUIUtility.fieldWidth = 100.0f;

            SerializedProperty minProp = property.FindPropertyRelative("Minimum");
            SerializedProperty maxProp = property.FindPropertyRelative("Maximum");
            int min = minProp.intValue;
            int max = maxProp.intValue;

            position = EditorGUI.IndentedRect(position);
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            float width = 150.0f;
            Rect rect = new Rect(position);
            rect.width = width;
            int newMin = EditorGUI.IntField(rect, "Min", min);
            if (newMin != min)
            {
                if (newMin > max) newMin = max;
                minProp.intValue = newMin;
            }
            rect.x += width + 5f;
            int newMax = EditorGUI.IntField(rect, "Max", max);
            if (newMax != max)
            {
                if (newMax < min) newMax = min;
                maxProp.intValue = newMax;
            }
           
            EditorGUI.indentLevel = indent;
        }

        private const float DrawHeight = 20.0f;
    }
}
