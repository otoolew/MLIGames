// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   12/12/2016
// ----------------------------------------------------------------------------

using System;
using UnityEditor;
using UnityEngine;

namespace SG.Core.Inspector
{
    [CustomPropertyDrawer(typeof (BaseValueRangeSet), true)]
    public class ValueRangeSetDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                SerializedProperty ranges = property.FindPropertyRelative("Ranges");
                return Mathf.Max((ranges.arraySize + 1)*16, 16);
            }
            return 16;
        }

        public static void Insert(int index, SerializedProperty ranges, object max, object min, bool minInc, bool maxInc,
            ComparisonOperator op)
        {
            ranges.InsertArrayElementAtIndex(index);
            SerializedProperty newProp = ranges.GetArrayElementAtIndex(index);
            if (newProp.FindPropertyRelative("Value") != null)
            {
                newProp.FindPropertyRelative("Value").FindPropertyRelative("Min").SetPropertyValue(min);
                newProp.FindPropertyRelative("Value").FindPropertyRelative("Max").SetPropertyValue(max);
                newProp.FindPropertyRelative("Value").FindPropertyRelative("MinInclusive").boolValue = minInc;
                newProp.FindPropertyRelative("Value").FindPropertyRelative("MaxInclusive").boolValue = maxInc;
                newProp.FindPropertyRelative("Operator").enumValueIndex = (int) op;
            }
            ranges.serializedObject.ApplyModifiedProperties();
        }

        public static void DrawRowButtons(Rect rect, SerializedProperty ranges, int i, bool isAndUp, SerializedProperty max, Action<int> delCallback, Action<int> addCallback)
        {
            Rect addRect = new Rect(rect.x, rect.y, rect.width/2.0f, rect.height);
            Rect delRect = new Rect(addRect.xMax, rect.y, rect.width / 2.0f, rect.height);

            if (!isAndUp)
            {
                if (GUI.Button(addRect, "+", EditorStyles.miniButtonLeft))
                {
                    int index = i;
                    EditorApplication.delayCall += () =>
                    {
                        addCallback(index);
                        Insert(index + 1, ranges, max == null ? null : max.GetPropertyValue(), max == null ? null : max.GetPropertyValue(), true, false,
                            ComparisonOperator.Equal);
                    };
                }
            }

            GUIStyle style = isAndUp ? EditorStyles.miniButton : EditorStyles.miniButtonRight;
            if (GUI.Button(delRect, "x", style))
            {
                int index = i;
                EditorApplication.delayCall += () =>
                {
                    ranges.DeleteArrayElementAtIndex(index);
                    delCallback(index);
                    ranges.serializedObject.ApplyModifiedProperties();
                };

            }
        }

        public static void DrawRow(Rect rowRect, SerializedProperty ranges, int i, SerializedProperty previousRange, SerializedProperty previousOp, Action<int> delCallback, Action<int> addCallback)
        {
            SerializedProperty range = ranges.GetArrayElementAtIndex(i).FindPropertyRelative("Value");
            SerializedProperty min = range.FindPropertyRelative("Min");
            SerializedProperty max = range.FindPropertyRelative("Max");
            SerializedProperty minInc = range.FindPropertyRelative("MinInclusive");
            SerializedProperty maxInc = range.FindPropertyRelative("MaxInclusive");
            SerializedProperty op = ranges.GetArrayElementAtIndex(i).FindPropertyRelative("Operator");
            
            Rect indexRect = rowRect;
            indexRect.width = 20;

            Rect buttonRect = rowRect;
            buttonRect.width = 36;
            buttonRect.x = rowRect.xMax - buttonRect.width;

            Rect rangeRect = new Rect(indexRect.xMax, rowRect.y,
                rowRect.width, rowRect.height) {xMax = buttonRect.xMin - 10};
            Rect minRect = new Rect(rangeRect.x, rangeRect.y,
                (rangeRect.width / 2.0f) - 7, rangeRect.height);
            Rect maxRect = new Rect(minRect.xMax, rangeRect.y,
                (rangeRect.width / 2.0f) + 7, rangeRect.height);

            bool isLowerBound = i == 0 &&
                op.enumValueIndex == (int)ComparisonOperator.LessThan;

            bool isUpperBound = (i >= ranges.arraySize - 1) && 
                op.enumValueIndex == (int)ComparisonOperator.GreaterThanOrEqual;

            DrawRowButtons(buttonRect, ranges, i, isUpperBound, max, delCallback, addCallback);
           
            // Match lower bound max to next min
            if (isLowerBound)
            {
                if (ranges.arraySize > i + 1)
                {
                    SerializedProperty next = ranges.GetArrayElementAtIndex(i + 1).FindPropertyRelative("Value").FindPropertyRelative("Min");
                    max.SetPropertyValue(next.GetPropertyValue());
                }

                min.SetPropertyValue(max.GetPropertyValue());
                maxInc.boolValue = true;
                minInc.boolValue = true;
                ranges.serializedObject.ApplyModifiedProperties();
            }
            // Match upper bound mas to min
            else if (isUpperBound)
            {
                max.SetPropertyValue(min.GetPropertyValue());
                maxInc.boolValue = true;
                minInc.boolValue = true;
                ranges.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.LabelField(indexRect, new GUIContent(i.ToString()));

            EditorGUI.BeginChangeCheck();
            // Draw min value
            if (isLowerBound)
                EditorGUI.LabelField(minRect, "    Less Than");
            else
                ValueRangeDrawer.DrawMin(minRect, min, minInc, false, isUpperBound);

            // Draw max value
            GUI.enabled = i >= ranges.arraySize - 1;
            if (isUpperBound)
                EditorGUI.LabelField(maxRect, "    And Up");
            else
                ValueRangeDrawer.DrawMax(maxRect, max, maxInc, false, isLowerBound);
            GUI.enabled = true;

            if (previousRange != null)
            {
                SerializedProperty prevMin = previousRange.FindPropertyRelative("Min");
                SerializedProperty prevMax = previousRange.FindPropertyRelative("Max");

                // Protect against overlap
                IComparable prevMinValue = prevMin.GetPropertyValue() as IComparable;
                IComparable minValue = min.GetPropertyValue() as IComparable;
                int comp = prevMinValue.CompareTo(minValue);
                if (comp >= 0 && previousOp.enumValueIndex != (int)ComparisonOperator.LessThan)
                {
                    min.SetPropertyValue(prevMin.GetPropertyValue());
                }

                prevMax.SetPropertyValue(min.GetPropertyValue());
            }

            if (EditorGUI.EndChangeCheck())
            {
                ranges.serializedObject.ApplyModifiedProperties();
            }
        }

        public static Rect DrawLabel(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect headerRect = position;
            headerRect.height = 16;
            headerRect.width = 100;
            EditorGUI.BeginChangeCheck();
            property.isExpanded = EditorGUI.Foldout(headerRect, property.isExpanded, new GUIContent(" "), true);
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);
            return position;
        }

        public static void DrawInfo(Rect position, SerializedProperty ranges, bool expanded, Action<int> addCallback)
        {
            Rect infoRect = position;
            infoRect.height = 16;
            Rect infoCursor = infoRect;
            infoCursor.width = 60;
            EditorGUI.LabelField(infoCursor, "Size " + ranges.arraySize);
            infoCursor.x = infoCursor.xMax;
            infoCursor.width = 20;

            if (ranges.arraySize <= 0)
            {
                if (GUI.Button(infoCursor, "+", EditorStyles.miniButton))
                {
                    EditorApplication.delayCall += () =>
                    {
                        addCallback(0);
                        Insert(0, ranges, null, null, true, false, ComparisonOperator.Equal);
                    };
                }
                infoCursor.x = infoCursor.xMax;
            }
            else if (expanded)
            {
                infoCursor.width = 70;
                SerializedProperty firstMin = ranges.GetArrayElementAtIndex(0).FindPropertyRelative("Value").FindPropertyRelative("Min");
                SerializedProperty firstOp = ranges.GetArrayElementAtIndex(0).FindPropertyRelative("Operator");

                SerializedProperty lastMax = ranges.GetArrayElementAtIndex(ranges.arraySize - 1).FindPropertyRelative("Value").FindPropertyRelative("Max");
                SerializedProperty lastOp = ranges.GetArrayElementAtIndex(ranges.arraySize - 1).FindPropertyRelative("Operator");

                GUI.enabled = (firstOp.enumValueIndex != (int)ComparisonOperator.LessThan);
                if (GUI.Button(infoCursor, "Add Lower", EditorStyles.miniButtonLeft))
                {
                    EditorApplication.delayCall += () =>
                    {
                        addCallback(0);
                        Insert(0, ranges, firstMin.GetPropertyValue(), firstMin.GetPropertyValue(), true, true,
                            ComparisonOperator.LessThan);
                    };
                }
                infoCursor.x = infoCursor.xMax;

                GUI.enabled = (lastOp.enumValueIndex != (int)ComparisonOperator.GreaterThanOrEqual);
                if (GUI.Button(infoCursor, "Add Upper", EditorStyles.miniButtonRight))
                {
                    EditorApplication.delayCall += () =>
                    {
                        addCallback(ranges.arraySize);
                        Insert(ranges.arraySize, ranges, lastMax.GetPropertyValue(), lastMax.GetPropertyValue(), true,
                            true, ComparisonOperator.GreaterThanOrEqual);
                    };
                }
                infoCursor.x = infoCursor.xMax;
                GUI.enabled = true;
            }
        }
        

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int previousIndent = EditorGUI.indentLevel;
            position = EditorGUI.IndentedRect(position);
            EditorGUI.indentLevel = 0;

            SerializedProperty ranges = property.FindPropertyRelative("Ranges");

            // Draw the foldout label
            position = DrawLabel(position, property, label);
            
            // Draw the count and header buttons
            DrawInfo(position, ranges, property.isExpanded, x => { });

            if (!property.isExpanded)
                return;

            Rect rowArea = position;
            rowArea.yMin += 16;
            Rect rowRect = rowArea;
            rowRect.height = 16;
            SerializedProperty previousRange = null;
            SerializedProperty previousOp = null;
            for (int i = 0; i < ranges.arraySize; i++)
            {
                rowRect.y = rowArea.y + (rowRect.height * i);
                SerializedProperty range = ranges.GetArrayElementAtIndex(i).FindPropertyRelative("Value");
                SerializedProperty op = ranges.GetArrayElementAtIndex(i).FindPropertyRelative("Operator");
                DrawRow(rowRect, ranges, i, previousRange, previousOp, x => { }, x => { });
                previousRange = range;
                previousOp = op;
            }

            EditorGUI.EndProperty();
            EditorGUI.indentLevel = previousIndent;
        }
    }
}