// ------------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 4/26/2016 9:49:59 AM
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using SG.Core;
using UnityEditor;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace SG.Dynamics
{
    [CustomPropertyDrawer(typeof(DynamicValue))]
    public class DynamicPropertyDrawer : PropertyDrawer
    {
        private static readonly GUIContent _valueLabel = new GUIContent("Value");

        private class DynamicPropertyDrawerState
        {
            public DynamicSo ScriptableObject;
            public SerializedObject SerializedObject;
            public object LastReconciledValue;
            public bool ReconciledOnce;
        }

        private Dictionary<string, DynamicPropertyDrawerState> _states = new Dictionary<string, DynamicPropertyDrawerState>();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty lockTypeProperty = property.FindPropertyRelative("_lockType");
            bool lockType = lockTypeProperty.boolValue;

            if (!lockType && !property.isExpanded)
                return EditorGUIUtility.singleLineHeight;

            float totalHeight = lockType ? 0f : EditorGUIUtility.singleLineHeight * 2f; // foldout + Type field

            SerializedProperty typeProperty = property.FindPropertyRelative("_type");
            Type dynamicType = Type.GetType(typeProperty.stringValue);

            // make space for error box or prepare for default value (string)
            if (dynamicType == null)
            {
                if (!string.IsNullOrEmpty(typeProperty.stringValue))
                    return totalHeight + EditorGUIUtility.singleLineHeight*2f;
                dynamicType = typeof(string);
            }

            // primitive types will be handled with one of the internal fields
            if (dynamicType == typeof(string) || dynamicType == typeof(bool)
                || dynamicType == typeof(int) || dynamicType == typeof(float)
                || dynamicType == typeof(UObject))
                return totalHeight + EditorGUIUtility.singleLineHeight;

            // otherwise we'll need to set up a dynamic scriptable object to support the system
            DynamicPropertyDrawerState state = GetOrCreateDynamicState(property.propertyPath, dynamicType);

            // make space for error box
            if (state.ScriptableObject == null)
                return totalHeight + EditorGUIUtility.singleLineHeight*2f;

            SerializedProperty iterator = state.SerializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                if (iterator.propertyPath == "m_Script")
                    continue;

                totalHeight += EditorGUI.GetPropertyHeight(iterator, null, true);
                enterChildren = false;
            }
            return totalHeight;
        }

        /// <summary>
        /// Checks the type of the currently bound ScriptableObject and if it is an incompatible type, discards it.
        /// </summary>
        private DynamicPropertyDrawerState GetOrCreateDynamicState(string propertyPath, Type dynamicType)
        {
            DynamicPropertyDrawerState state;
            if (!_states.TryGetValue(propertyPath, out state))
                _states.Add(propertyPath, state = new DynamicPropertyDrawerState());

            if (state.ScriptableObject != null && dynamicType.IsAssignableFrom(state.ScriptableObject.Type))
                return state;

            if (state.ScriptableObject != null)
                UObject.DestroyImmediate(state.ScriptableObject);

            state.LastReconciledValue = null;
            state.ReconciledOnce = false;

            Type dynamicSoType = DynamicTypePropertyDrawer.GetDynamicSoType(dynamicType);
            if (dynamicSoType == null)
            {
                state.ScriptableObject = null;
                if (state.SerializedObject == null)
                    return state;

                state.SerializedObject.Dispose();
                state.SerializedObject = null;
                return state;
            }
            state.ScriptableObject = (DynamicSo)ScriptableObject.CreateInstance(dynamicSoType);

            if (state.SerializedObject != null)
                state.SerializedObject.Dispose();
            state.SerializedObject = new SerializedObject(state.ScriptableObject);
            return state;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty lockTypeProperty = property.FindPropertyRelative("_lockType");
            bool lockType = lockTypeProperty.boolValue;
            
            // skip over drawing any Type picking code if we're set to LockType (type dictated by other source)
            if (!lockType)
            {
                position.height = EditorGUIUtility.singleLineHeight;
                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
                position.y += position.height;

                if (!property.isExpanded)
                    return;

                EditorGUI.indentLevel++;
            }

            bool guiChange = false;

            SerializedProperty typeProperty = property.FindPropertyRelative("_type");

            if (!lockType)
            {
                position.height = EditorGUI.GetPropertyHeight(typeProperty);
                guiChange = EditorGUI.PropertyField(position, typeProperty);
                position.y += position.height;
            }

            Type dynamicType = Type.GetType(typeProperty.stringValue);
            if (dynamicType == null)
                return;

            SerializedProperty innerProperty = null;
            if (dynamicType == typeof(string))
                innerProperty = property.FindPropertyRelative("_string");
            else if (dynamicType == typeof(bool))
                innerProperty = property.FindPropertyRelative("_bool");
            else if (dynamicType == typeof(int))
                innerProperty = property.FindPropertyRelative("_int");
            else if (dynamicType == typeof(float))
                innerProperty = property.FindPropertyRelative("_float");
            else if (dynamicType == typeof(UObject))
                innerProperty = property.FindPropertyRelative("_object");

            // if dealing with a non-primitive type, we must generate a dynamic scriptable object to generate a proper drawer
            if (innerProperty == null)
            {
                DynamicPropertyDrawerState state = GetOrCreateDynamicState(property.propertyPath, dynamicType);

                if (state.ScriptableObject == null)
                {
                    position.height = EditorGUIUtility.singleLineHeight * 2f;
                    EditorGUI.HelpBox(position, "Cannot Create Dynamic Scriptable Object For " + typeProperty.stringValue, MessageType.Warning);
                    if (!lockType)
                        EditorGUI.indentLevel--;
                    return;
                }

                // in the case of a single UObject reference, we do not need to do the full serialization loop to and from strings
                bool storeValueInObjectValue = typeof(UObject).IsAssignableFrom(dynamicType);
                innerProperty = property.FindPropertyRelative(storeValueInObjectValue ? "_object" : "_string");

                state.SerializedObject.Update();
                SerializedProperty fakeObjectValueProperty = state.SerializedObject.FindProperty("Value");

                if (state.ReconciledOnce)
                {
                    // first figure out the actual current value of the two places
                    // a - the value inside our fake object (may have been changed by a propertydrawer like the uberpicker outside of this function)
                    // b - the serialized data inside this object (may have been changed by undo/redo or debug inspector
                    // whichever doesn't match our previous reconciled data trumps, with the tie breaker given to the data inside this object
                    if (storeValueInObjectValue)
                    {
                        int fromFakeObject = fakeObjectValueProperty.objectReferenceInstanceIDValue;
                        int fromThisObject = innerProperty.objectReferenceInstanceIDValue;
                        int reconcileValue = state.LastReconciledValue as int? ?? 0;
                        if (fromThisObject == reconcileValue && fromFakeObject != reconcileValue)
                        {
                            state.LastReconciledValue = innerProperty.objectReferenceInstanceIDValue = fromFakeObject;
                            guiChange = true;
                        }
                        else if (fromThisObject != reconcileValue)
                        {
                            state.LastReconciledValue =
                                fakeObjectValueProperty.objectReferenceInstanceIDValue = fromThisObject;
                            state.SerializedObject.ApplyModifiedProperties();
                        }
                    }
                    else
                    {
                        string fromFakeObject = Services.Locate<IDynamicSerializer>().Serialize(Convert.ChangeType(state.ScriptableObject.RawValue, dynamicType), false);
                        string fromThisObject = innerProperty.stringValue;
                        string reconcileValue = state.LastReconciledValue as string ?? string.Empty;
                        if (fromThisObject == reconcileValue && fromFakeObject != reconcileValue)
                        {
                            state.LastReconciledValue = innerProperty.stringValue = fromFakeObject;
                            guiChange = true;
                        }
                        else if (fromThisObject != reconcileValue)
                        {
                            state.LastReconciledValue = fromThisObject;
                            object deserialized = Services.Locate<IDynamicSerializer>().Deserialize(innerProperty.stringValue, dynamicType);
                            state.ScriptableObject.RawValue = deserialized ?? Activator.CreateInstance(dynamicType);

                            // need to update again since we pushed data into the C# layer
                            state.SerializedObject.Update();
                        }
                    }
                }
                else
                {
                    // just dump the value from the serialized data into the dynamic object
                    if (storeValueInObjectValue)
                    {
                        state.LastReconciledValue =
                            fakeObjectValueProperty.objectReferenceInstanceIDValue =
                                innerProperty.objectReferenceInstanceIDValue;
                        state.SerializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        state.LastReconciledValue = innerProperty.stringValue;
                        object deserialized = Services.Locate<IDynamicSerializer>().Deserialize(innerProperty.stringValue, dynamicType);
                        state.ScriptableObject.RawValue = deserialized ?? Activator.CreateInstance(dynamicType);

                        // need to update again since we pushed data into the C# layer
                        state.SerializedObject.Update();
                    }

                    state.ReconciledOnce = true;
                }

                EditorGUI.BeginChangeCheck();
                SerializedProperty iterator = state.SerializedObject.GetIterator();
                bool enterChildren = true;
                while (iterator.NextVisible(enterChildren))
                {
                    if (iterator.propertyPath == "m_Script")
                        continue;

                    float propertyHeight = EditorGUI.GetPropertyHeight(iterator, null, true);
                    position.height = propertyHeight;

                    EditorGUI.PropertyField(position, iterator, true);
                    position.y += propertyHeight;

                    enterChildren = false;
                }

                if (EditorGUI.EndChangeCheck())
                {
                    state.SerializedObject.ApplyModifiedProperties();
                    guiChange = true;

                    if (storeValueInObjectValue)
                        state.LastReconciledValue = innerProperty.objectReferenceInstanceIDValue = fakeObjectValueProperty.objectReferenceInstanceIDValue;
                    else
                        state.LastReconciledValue = innerProperty.stringValue =
                            Services.Locate<IDynamicSerializer>().Serialize(Convert.ChangeType(state.ScriptableObject.RawValue, dynamicType), false);
                }
            }
            else
            {
                guiChange |= EditorGUI.PropertyField(position, innerProperty, _valueLabel);
            }

            if (!lockType)
                EditorGUI.indentLevel--;
            GUI.changed = guiChange;
        }
    }
}
