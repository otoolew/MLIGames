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
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttributeDrawer : PropertyDrawer
    {
        private bool ShouldLock(ReadOnlyAttribute.PlayMode mode)
        {
            if (mode == ReadOnlyAttribute.PlayMode.Both)
                return true;
            if (Application.isPlaying && mode == ReadOnlyAttribute.PlayMode.Runtime)
                return true;
            if (!Application.isPlaying && mode == ReadOnlyAttribute.PlayMode.Edit)
                return true;
            return false;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ReadOnlyAttribute r = attribute as ReadOnlyAttribute;
            bool wasEnabled = GUI.enabled;
            GUI.enabled = !ShouldLock(r.WhenToLock);
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label);
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();
            GUI.enabled = wasEnabled;
        }
    }
}