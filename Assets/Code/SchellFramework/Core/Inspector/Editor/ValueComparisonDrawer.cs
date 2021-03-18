// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   12/02/2016
// ----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace SG.Core.Inspector
{
    /// <summary>
    /// A property drawer for ValueComparison types. This lets content creators 
    /// specify comparision rules for types that implement IComparable using a 
    /// simple one-line drawer.
    /// 
    /// To leverage this drawer for any custom types, there are two steps:
    /// 
    ///  1 - Implement IComparable on the type.
    ///      public class MyType : IComparable ...
    /// 
    ///  2 - Create a Serializable comparison type.
    ///      [Serializable]
    ///      public class MyTypeComparison : ValueComparison<MyType> { }
    /// </summary>
    [CustomPropertyDrawer(typeof(BaseValueComparison), true)]
    public class ComparisonDrawer : PropertyDrawer
    {
        #region -- Constants --------------------------------------------------
        /// <summary>
        /// Name of the property on ValueComparison that specifies how to 
        /// compare values.
        /// </summary>
        public const string OPERATOR_PROPERTY = "Operator";

        /// <summary>
        /// Name of the property on ValueComparison that specifies the base
        /// value to compare to.
        /// </summary>
        public const string VALUE_PROPERTY = "Value";
        #endregion -- Constants -----------------------------------------------

        /// <summary>
        /// Defines how to display the value of the property and modify it.
        /// </summary>
        /// <param name="position">Where to draw</param>
        /// <param name="value">SerializedProperty with the value.</param>
        public virtual void DrawValue(Rect position, SerializedProperty value)
        {
            if (value != null)
            {
                EditorGUI.PropertyField(position, value, GUIContent.none);
            }
            else
            {
                EditorGUI.HelpBox(position, 
                    "SerializedProperty '" + VALUE_PROPERTY + "' not found", 
                    MessageType.Warning);
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Get the relevant properties.
            SerializedProperty operation = property.FindPropertyRelative(OPERATOR_PROPERTY);
            SerializedProperty value = property.FindPropertyRelative(VALUE_PROPERTY);

            // start property
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);
            float endX = position.xMax;
            position.width = 16;
            Rect xLabel = position;
            xLabel.x += EditorGUI.indentLevel*15;
            GUI.Label(xLabel, "X ");

            position.x = position.xMax + EditorGUIUtility.standardVerticalSpacing;
            position.width = 35.0f;
            Rect popUpRect = position;
            popUpRect.width += EditorGUI.indentLevel*15;
            EditorGUI.BeginChangeCheck();
            int newIndex = EditorGUI.Popup(popUpRect, "", operation.enumValueIndex,
                BaseValueComparison.OPERATOR_DISPLAY);
            if (EditorGUI.EndChangeCheck())
                operation.enumValueIndex = newIndex;

            position.x = position.xMax + EditorGUIUtility.standardVerticalSpacing;
            position.xMax = endX;
            position.x -= EditorGUI.indentLevel*15;
            position.width += EditorGUI.indentLevel * 15;
            DrawValue(position, value);

            position.x += position.width;
            EditorGUI.EndProperty();
        }
    }
}