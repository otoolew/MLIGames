// -----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Max Golden
//
//  Created: 12/9/2016 2:00 PM
// -----------------------------------------------------------------------------

using UnityEngine;
using UnityEditor;

namespace SG.Core.Templating
{
    [CustomPropertyDrawer(typeof(TextTemplate))]
    public class TextTemplateDrawer : PropertyDrawer
    {
        const float textAreaHeight = 200;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + textAreaHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect propertyRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            Rect textRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, textAreaHeight);


            EditorGUI.PropertyField(propertyRect, property);

            string textString = string.Empty;
            TextTemplate template = property.objectReferenceValue as TextTemplate;
            if (template != null && template.text != null)
                textString = template.text;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.TextArea(textRect, textString);
            EditorGUI.EndDisabledGroup();
        }
    }
}
