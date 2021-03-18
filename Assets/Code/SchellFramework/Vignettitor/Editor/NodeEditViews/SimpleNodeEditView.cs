// ------------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 6/16/2016 3:41:41 PM
// ------------------------------------------------------------------------------

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SG.Vignettitor.NodeViews
{
    /// <summary>
    /// An edit view that automatically draws a set of property fields for a set of
    /// properties specified by the interface ISimpleNodeEditView.
    /// </summary>
    public class SimpleNodeEditView : VignetteNodeEditView
    {
        private SerializedObject _serializedObjectNode;
        private SerializedProperty[] _properties;

        public override void Draw(Rect rect)
        {
            base.Draw(rect);

            if (_serializedObjectNode == null)
            {
                _serializedObjectNode = new SerializedObject(Node);
                ISimpleNodeEditView node = Node as ISimpleNodeEditView;
                _properties = node.EditViewPropertyNames.Select(x => _serializedObjectNode.FindProperty(x)).ToArray();
            }

            _serializedObjectNode.Update();

            EditorGUI.BeginChangeCheck();

            for (int i = 0; i < _properties.Length; i++)
                EditorGUILayout.PropertyField(_properties[i], false);

            if (EditorGUI.EndChangeCheck())
                _serializedObjectNode.ApplyModifiedProperties();
        }
    }
}