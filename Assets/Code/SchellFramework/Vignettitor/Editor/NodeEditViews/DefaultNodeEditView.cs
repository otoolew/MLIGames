//-----------------------------------------------------------------------------
//  Copyright © 2017 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   01/25/2017
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using SG.Vignettitor.NodeViews;
using UnityEditor;
using UnityEngine;

namespace SG.Vignettitor.Editor.NodeEditViews
{
    /// <summary>
    /// A NodeEditView that will draw all fields on a node that are tagged 
    /// with NodeEditViewFieldAttribute. Under normal circumstances, the 
    /// vignettitor will use this node edit view if there is no specific node 
    /// edit view specified for a node.
    /// </summary>
    public class DefaultNodeEditView : VignetteNodeEditView
    {
        /// <summary> The serializedobject for the node. </summary>
        private SerializedObject nodeSO;

        /// <summary>
        /// A list of fieldInfo and NodeEditViewFieldAttribute for the node 
        /// being drawn.
        /// </summary>
        private readonly List<NodeEditViewFieldAttribute.FieldAttributePair> fields;

        /// <summary>
        /// A list of properties as specified by the fields.
        /// </summary>
        private SerializedProperty[] properties = new SerializedProperty[0];

        public DefaultNodeEditView(
            List<NodeEditViewFieldAttribute.FieldAttributePair> fields)
        {
            this.fields = fields;
        }

        public override void Draw(Rect rect)
        {
            base.Draw(rect);

            if (nodeSO == null)
            {
                nodeSO = new SerializedObject(Node);

                properties = new SerializedProperty[fields.Count];
                for (int i = 0; i < fields.Count; i++)
                {
                    SerializedProperty p = nodeSO.FindProperty(fields[i].Field.Name);
                    properties[i] = p;
                }
            }

            nodeSO.Update();
                
            bool oldWide = EditorGUIUtility.wideMode;
            EditorGUIUtility.wideMode = true;
            EditorGUI.BeginChangeCheck();
            float oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth *= 0.6f;
            for (int i = 0; i < fields.Count; i++)
            {
                if (properties[i] == null)
                    continue;
                GUIContent label = fields[i].Attribute.DrawFieldName
                    ? new GUIContent(properties[i].name)
                    : GUIContent.none;
                bool oldWrap = EditorStyles.textField.wordWrap;
                EditorStyles.textField.wordWrap = true;


                // Only draw focus if it is the only field, otherwise it causes 
                // issues interacting with other properties since it keeps 
                // stealing focus.
                if (fields[i].Attribute.AutoFocus && fields[i].Field.FieldType == typeof(string) && fields.Count == 1)
                {
                    GUI.FocusControl("LogEditNodeViewText");
                    GUI.SetNextControlName("LogEditNodeViewText");
                    //SetNextControlName does not work with PropertyField... lame, now i have to do this
                    if (fields[i].Attribute.FieldHeightMode == NodeViewFieldAttribute.HeightMode.Default)
                        properties[i].stringValue = EditorGUILayout.TextField(label, properties[i].stringValue);
                    else if (fields[i].Attribute.FieldHeightMode == NodeViewFieldAttribute.HeightMode.Specify)
                        properties[i].stringValue = EditorGUILayout.TextField(label, properties[i].stringValue, GUILayout.Height(fields[i].Attribute.Height));
                    else if (fields[i].Attribute.FieldHeightMode == NodeViewFieldAttribute.HeightMode.Fill)
                        properties[i].stringValue = EditorGUILayout.TextField(label, properties[i].stringValue, GUILayout.ExpandHeight(true));
                }
                else
                {
                    if (fields[i].Attribute.FieldHeightMode == NodeViewFieldAttribute.HeightMode.Default)
                        EditorGUILayout.PropertyField(properties[i], label, true);
                    else if (fields[i].Attribute.FieldHeightMode == NodeViewFieldAttribute.HeightMode.Specify)
                        EditorGUILayout.PropertyField(properties[i], label, true,
                            GUILayout.Height(fields[i].Attribute.Height));
                    else if (fields[i].Attribute.FieldHeightMode == NodeViewFieldAttribute.HeightMode.Fill)
                        EditorGUILayout.PropertyField(properties[i], label, true, GUILayout.ExpandHeight(true));
                }

               
                EditorStyles.textField.wordWrap = oldWrap;
            }

            EditorGUIUtility.labelWidth = oldWidth;
            EditorGUIUtility.wideMode = oldWide;

            if (EditorGUI.EndChangeCheck())
                nodeSO.ApplyModifiedProperties();
        }
    }
}
