// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   12/07/2016
// ----------------------------------------------------------------------------

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SG.Core.Inspector
{
    [CustomPropertyDrawer(typeof(BaseRange), true)]
    public class ValueRangeDrawer : PropertyDrawer
    {
        private const float WARN_WIDTH = 24;

        public static void DrawMin(Rect rect, SerializedProperty min, SerializedProperty minInc, bool showInc=true, bool hideDecorations=false)
        {
            if (showInc)
            {
                Rect minIncRect = rect;
                minIncRect.width = 14;
                rect.xMin = minIncRect.xMax;
                EditorGUI.PropertyField(minIncRect, minInc, GUIContent.none);
            }

            float oldW = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 16;
            EditorGUI.PropertyField(rect, min, new GUIContent(hideDecorations?" ":minInc.boolValue ? " [" : " (",
                (minInc.boolValue ? "<= " : "< ") + min.GetPropertyValue()), true);
            EditorGUIUtility.labelWidth = oldW;
        }

        public static void DrawMax(Rect rect, SerializedProperty max, SerializedProperty maxInc, bool showInc = true, bool hideDecorations = false)
        {
            rect.width -= 14;
            if (showInc)
                rect.width -= 14;
            float oldW2 = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 16;
            
            EditorGUI.PropertyField(rect, max, new GUIContent(hideDecorations?" " :" :  ",
                (maxInc.boolValue ? ">= " : "> ") + max.GetPropertyValue()), true);
            rect.x = rect.xMax;

            rect.width = 14;
            EditorGUI.LabelField(rect, hideDecorations ? " " : maxInc.boolValue ? " ]" : " )");
            EditorGUIUtility.labelWidth = oldW2;
            if (showInc)
            {
                rect.x += rect.width;
                EditorGUI.PropertyField(rect, maxInc, GUIContent.none);
            }
        }

        public static void DrawRange(Rect rect, 
            SerializedProperty min, SerializedProperty minInc, 
            SerializedProperty max, SerializedProperty maxInc)
        {
            Rect minRect = new Rect(rect.x, rect.y,
                (rect.width / 2.0f) - 7, rect.height);
            Rect maxRect = new Rect(minRect.xMax, rect.y,
                (rect.width / 2.0f) + 7, rect.height);

            DrawMin(minRect, min, minInc, true);
            DrawMax(maxRect, max, maxInc, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float indentSpace = EditorGUI.indentLevel*15;
            int oldIndent = EditorGUI.indentLevel;

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);
            position.height = base.GetPropertyHeight(property, label);
            EditorGUI.indentLevel = 0;
            position.xMin += indentSpace;
            // Get data
            SerializedProperty min = property.FindPropertyRelative("Min");
            SerializedProperty max = property.FindPropertyRelative("Max");
            SerializedProperty minInc = property.FindPropertyRelative("MinInclusive");
            SerializedProperty maxInc = property.FindPropertyRelative("MaxInclusive");

            bool empty = false;
            try
            {
                object obj = property.GetObjectForProperty();
                MethodInfo emptyTest = obj.GetType()
                    .GetMethod("IsEmptySet", BindingFlags.Instance | BindingFlags.Public);
                empty = (bool) emptyTest.Invoke(obj, new object[0]);
            }
            catch (System.Exception){}

            // Draw
            Rect center = position;
            //center.xMax = right.xMin;
            if (empty)
            {
                center.xMax -= WARN_WIDTH;
                Color oldColor = GUI.color;
                GUI.color = Color.yellow;
                GUI.Box(position, "");
                GUI.color = oldColor;
            }
            EditorGUI.BeginChangeCheck();

            float oldW = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 28;

            DrawRange(center, min, minInc, max, maxInc);

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }
            if (empty)
            {
                Rect warn = new Rect(center.xMax, center.y - 3, WARN_WIDTH, center.height+6);
                EditorGUI.HelpBox(warn, "", MessageType.Warning);
            }

            EditorGUI.EndProperty();
            EditorGUI.indentLevel = oldIndent;
        }
    }
}