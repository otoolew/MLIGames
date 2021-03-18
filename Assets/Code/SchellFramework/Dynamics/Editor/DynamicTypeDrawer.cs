// ------------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 5/3/2016 1:56:20 PM
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using SG.Core;
using UnityEditor;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace SG.Dynamics
{
    [CustomPropertyDrawer(typeof(DynamicTypeAttribute))]
    public class DynamicTypePropertyDrawer : PropertyDrawer
    {
        private static readonly Dictionary<Type, Type> _dynamicTypeToDynamicSoType;
        private static readonly Dictionary<int, Type> _popupIndexToType;
        private static readonly Dictionary<Type, int> _typeToPopupIndex;

        private static GUIContent[] _popupLables;

        static DynamicTypePropertyDrawer()
        {
            TypeSet dynamicSoTypeSet = AssemblyUtility.GetDerivedTypes(typeof(DynamicSo));
            List<Type> dynamicSoTypes = dynamicSoTypeSet.types.Where(t => !t.IsAbstract && !t.IsGenericType).OrderBy(t => t.Name).ToList();
            _popupLables = new GUIContent[SupportedTypes.Count + dynamicSoTypes.Count];
            _popupIndexToType = new Dictionary<int, Type>(_popupLables.Length);
            _typeToPopupIndex = new Dictionary<Type, int>(_popupLables.Length);
            for (int i = 0; i < SupportedTypes.Count; i++)
            {
                string name = SupportedTypes[i].Name;
                Type type = SupportedTypes[i].Type;
                _popupLables[i] = new GUIContent(name);
                _popupIndexToType[i] = type;
                _typeToPopupIndex[type] = i;
            }

            _dynamicTypeToDynamicSoType = new Dictionary<Type, Type>(dynamicSoTypes.Count);

            int builtInCount = SupportedTypes.Count;
            for (int i = 0; i < dynamicSoTypes.Count; i++)
            {
                Type dynamicSoType = dynamicSoTypes[i];
                if (!dynamicSoType.BaseType.IsGenericType || dynamicSoType.BaseType.GetGenericTypeDefinition() != typeof(DynamicSo<>))
                    throw new DynamicException("DynamicSoType does not directly inherit from DynamicSo<> " + dynamicSoType.FullName);
                Type type = dynamicSoType.BaseType.GetGenericArguments()[0];
                int arrayIndex = builtInCount + i;
                _popupLables[arrayIndex] = new GUIContent(type.Name);
                _popupIndexToType[arrayIndex] = type;
                _typeToPopupIndex[type] = arrayIndex;
                _dynamicTypeToDynamicSoType[type] = dynamicSoType;
            }
        }

        private struct NameTypeTuple
        {
            public string Name;
            public Type Type;
        }

        private static readonly List<NameTypeTuple> SupportedTypes = new List<NameTypeTuple>
        {
            new NameTypeTuple{Name = "string", Type = typeof(string)},
            new NameTypeTuple{Name = "bool", Type = typeof(bool)},
            new NameTypeTuple{Name = "int", Type = typeof(int)},
            new NameTypeTuple{Name = "float", Type = typeof(float)},
            new NameTypeTuple{Name = "UnityEngine.Object", Type = typeof(UObject)}
        };

        public static Type GetDynamicSoType(Type underlyingType)
        {
            Type dynamicSoType;
            _dynamicTypeToDynamicSoType.TryGetValue(underlyingType, out dynamicSoType);
            return dynamicSoType;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = EditorGUIUtility.singleLineHeight;
            Type dynamicType = Type.GetType(property.stringValue);

            // make space for error box if there is a value and Type can't be found
            if (!string.IsNullOrEmpty(property.stringValue) && dynamicType == null)
                totalHeight += EditorGUIUtility.singleLineHeight * 2f;
            return totalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            if (string.IsNullOrEmpty(property.stringValue))
            {
                property.stringValue = "System.String";
                GUI.changed = true;
            }
            Type dynamicType = Type.GetType(property.stringValue);

            int popupIndex;
            if (dynamicType == null)
            {
                EditorGUI.PropertyField(position, property);
                position.y += position.height;
                position.height = EditorGUIUtility.singleLineHeight * 2f;
                EditorGUI.HelpBox(position, "Cannot Parse Dynamic Type " + property.stringValue, MessageType.Warning);
                return;
            }

            if (!_typeToPopupIndex.TryGetValue(dynamicType, out popupIndex))
                GUI.changed = EditorGUI.PropertyField(position, property);
            else
            {
                int newIndex = EditorGUI.Popup(position, label, popupIndex, _popupLables);

                if (popupIndex == newIndex)
                    return;

                dynamicType = _popupIndexToType[newIndex];
                property.stringValue = dynamicType.AssemblyQualifiedName;
                GUI.changed = true;
            }
        }
    }
}
