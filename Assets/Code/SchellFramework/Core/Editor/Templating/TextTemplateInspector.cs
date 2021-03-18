// -----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Max Golden
//
//  Created: 12/9/2016 11:00 AM
// -----------------------------------------------------------------------------

using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace SG.Core.Templating
{
    [CustomEditor(typeof(TextTemplate))]
    public class TextTemplateInspector : Editor
    {
        string assetName;
        SerializedProperty textProperty;

        private void OnEnable()
        {
            assetName = Regex.Replace(serializedObject.targetObject.name, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");
            textProperty = serializedObject.FindProperty("text");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.LabelField(assetName);
            textProperty.stringValue = EditorGUILayout.TextArea(textProperty.stringValue,
                GUILayout.MinWidth(200), GUILayout.MaxWidth(700), GUILayout.ExpandWidth(true),
                GUILayout.MinHeight(100), GUILayout.MaxHeight(700), GUILayout.ExpandHeight(true));

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save", GUILayout.MaxWidth(100)))  // The save button makes it feel... better?
                serializedObject.ApplyModifiedProperties();
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
