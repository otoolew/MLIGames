//------------------------------------------------------------------------------
// Copyright © 2016 Schell Games, LLC. All Rights Reserved.
// Contact: William Roberts
// Created: 06/13/2016 3:51:00 PM
//------------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace SG.Core
{
    /// <summary>
    /// Renders the Layer value type as a drop down list of Layer Names.
    /// </summary>
    [CustomPropertyDrawer(typeof(Layer))]
    public class LayerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);

            var indexProperty = property.FindPropertyRelative("_layerIndex");

            if (indexProperty != null)
            {
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                indexProperty.intValue = EditorGUI.LayerField(position, indexProperty.intValue);
            }
            else
            {
                EditorGUI.LabelField(position, "**ERROR** Failed to find the Serialized Property named '_layerIndex'!");
            }

            EditorGUI.EndProperty();
        }
    }
}
