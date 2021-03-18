//-----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   7/20/2016
//-----------------------------------------------------------------------------

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using SG.Core.Inspector;

namespace SG.GlobalEvents.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BaseGlobalEventListener), true)]
    public class GlobalEventListenerEditor : UnityEditor.Editor
    {
        private static int swapInstanceID;
        private static Object swapObject;

        
        protected void OnEnable()
        {
            if (target.GetInstanceID() == swapInstanceID)
            {
                SerializedProperty eventProp = serializedObject.FindProperty("GlobalEvent");
                SerializedProperty oldEventProp = serializedObject.FindProperty("oldEvent");
                if (oldEventProp != null)
                    oldEventProp.objectReferenceValue = swapObject;
                eventProp.objectReferenceValue = swapObject;
                eventProp.isExpanded = true;
                serializedObject.ApplyModifiedProperties();
                swapObject = null;
                swapInstanceID = 0;
            }
        }

        private static Type GetListenerTypeForEvent(Type get)
        {
            if (get == typeof(GlobalEvent))
                return typeof(GlobalEventListener);
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                if (assemblies[i].ManifestModule.Name != "Assembly-CSharp.dll")
                    continue;

                Type[] types = assemblies[i].GetTypes();
                for (int t = 0; t < types.Length; t++)
                {
                    Type type = types[t];
                    if (typeof(BaseGlobalEventListener).IsAssignableFrom(type))
                    {
                        Type[] genT = type.BaseType.GetGenericArguments();

                        if (genT.Length > 0 && genT[0] == get)
                            return types[t];
                    }
                }
            }
            return null;
        }
   
        // TODO: handle changing event type on the event and then coming back to the editor. maybe show a warning if the event does not match the type

        public void ChangeListenerType(Type listenerType, Object eventObject)
        {
            BaseGlobalEventListener gel = target as BaseGlobalEventListener;

            MonoBehaviour dummy = gel.gameObject.AddComponent(listenerType) as MonoBehaviour;
            MonoScript ms = MonoScript.FromMonoBehaviour(dummy);
            DestroyImmediate(dummy);

            swapInstanceID = target.GetInstanceID();
            swapObject = eventObject;
            serializedObject.FindProperty("m_Script").objectReferenceValue = ms;
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            BaseGlobalEventListener gel = target as BaseGlobalEventListener;
            if (gel == null)
                return;
            gel.OnValidate();
            
            SerializedProperty eventProp = serializedObject.FindProperty("GlobalEvent");
            SerializedProperty oldEventProp = serializedObject.FindProperty("oldEvent");
            SerializedProperty responseProp = serializedObject.FindProperty("Response");
            SerializedProperty advancedProp = serializedObject.FindProperty("Advanced");

            SerializedProperty conditionsProp = serializedObject.FindProperty("Conditions");
            SerializedProperty responsesProp = serializedObject.FindProperty("Responses");
            if (serializedObject.FindProperty("ConditionSet") != null)
            {
                conditionsProp = serializedObject.FindProperty("ConditionSet").FindPropertyRelative("Ranges");
            }
            

            // TODO: To determine equality tests:
            // if it has a range compare, use the condition set
            // if unity object, use ==
            // if serializable and IComparable, use compare == 0

            bool wasVoid = false;
            Type expectedType = typeof(GlobalEvent);
            Type[] genT = target.GetType().BaseType.GetGenericArguments();
            if (genT.Length != 0)
                expectedType = genT[0];
            else
                wasVoid = true;

            BaseGlobalEvent ge = eventProp.objectReferenceValue as BaseGlobalEvent;

            Type eventType = null;
            if (ge != null)
            {
                eventType = ge.GetType();
            }

            if (eventType == null && !(oldEventProp != null && oldEventProp.objectReferenceValue != null))
            {
                EditorGUILayout.HelpBox("Please set an event below to trigger the response.", MessageType.Error);
            }
            else if (!expectedType.IsInstanceOfType(ge) ||
                (eventType == typeof(GlobalEvent) && expectedType != typeof(GlobalEvent)) ||
                (expectedType == typeof(GlobalEvent) && eventType != typeof(GlobalEvent)) )
            {
                Type et = eventType ?? oldEventProp.objectReferenceValue.GetType();
                GUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("Data type passed by event (" +
                    et.Name + 
                    ") does not match data type handled by listener (" +
                     expectedType.Name + ").", MessageType.Error);
                if (GUILayout.Button("Change Listener Type"))
                {
                    Type listenerType = GetListenerTypeForEvent(et);
                    ChangeListenerType(listenerType, ge);
                    GUIUtility.ExitGUI();
                }
                GUILayout.EndHorizontal();
            }
            else if (ge.Deprecated)
            {
                EditorGUILayout.HelpBox("The selected event is deprecated.", 
                    ge.OnDeprecatedRaise == ErrorProcedure.Exception ?
                        MessageType.Error : MessageType.Warning);
            }

            Object newObj;
            if (oldEventProp != null && oldEventProp.objectReferenceValue != null)
            {
                newObj = EditorGUILayout.ObjectField(new GUIContent(""),
                    oldEventProp.objectReferenceValue, typeof(BaseGlobalEvent), false);
            }
            else
            {
                newObj = EditorGUILayout.ObjectField(new GUIContent(""),
                    ge, typeof (BaseGlobalEvent), false);
            }
            if (oldEventProp != null)
                oldEventProp.objectReferenceValue = newObj;
            
            if (newObj != ge)
            {
                if (newObj == null)
                {
                    eventProp.objectReferenceValue = null;
                    serializedObject.ApplyModifiedProperties();
                }
                else if (newObj is BaseGlobalEvent && ((BaseGlobalEvent) newObj).Deprecated)
                {
                    EditorUtility.DisplayDialog("Event Deprecated", 
                        "The GlobalEvent " + newObj.name + " is deprecated so it can not be used.", 
                        "OK");
                }
                else if (expectedType.IsInstanceOfType(newObj) && !(wasVoid && newObj.GetType() != typeof(BaseGlobalEvent)))
                {
                    eventProp.objectReferenceValue = newObj;
                    serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    Type listenerType = GetListenerTypeForEvent(newObj.GetType());
                    if (listenerType != null)
                    {
                        if (listenerType == target.GetType())
                        {
                            if (oldEventProp != null)
                                eventProp.objectReferenceValue = oldEventProp.objectReferenceValue;
                            serializedObject.ApplyModifiedProperties();
                        }
                        else
                            ChangeListenerType(listenerType, newObj);
                        GUIUtility.ExitGUI();
                    }
                }
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(responseProp, true);
            Rect advancedRect = GUILayoutUtility.GetLastRect();
            advancedRect.y = advancedRect.yMax - 16;
            advancedRect.height = 16;
            advancedRect.x += 16;
            advancedProp.boolValue = EditorGUI.Foldout(advancedRect, advancedProp.boolValue, "Advanced", true);
            if (advancedProp.boolValue)
            {
                GUILayout.Space(8);
                GUILayout.BeginHorizontal();
                GUILayout.Space(16 * (EditorGUI.indentLevel + 1));
                GUILayout.BeginVertical();

                DrawPropertiesExcluding(serializedObject, "m_Script", "oldEvent", 
                    "Advanced", "GlobalEvent", "Response", "InvokeForAllMetConditions", 
                    "Conditions", "Responses", "ConditionSet", "OnNoConditionsMet");

                if (serializedObject.FindProperty("ConditionSet") != null)
                    DrawRanges(conditionsProp, responsesProp);
                else if (serializedObject.FindProperty("OnNoConditionsMet") != null)
                    DrawConditions(conditionsProp, responsesProp);

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        private void DrawConditions(SerializedProperty conditionsProp, SerializedProperty responsesProp)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.BeginHorizontal();
            GUILayout.Space(12.0f);
            conditionsProp.isExpanded = EditorGUILayout.Foldout(conditionsProp.isExpanded, "Conditional Responses", true);
            GUILayout.EndHorizontal();
            if (conditionsProp.isExpanded)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("InvokeForAllMetConditions"));

                Rect infoRect = GUILayoutUtility.GetRect(16, 16);
                infoRect.x += 1;
                infoRect.height = 16;
                Rect infoCursor = infoRect;
                infoCursor.width = 60;
                EditorGUI.LabelField(infoCursor, "Size " + conditionsProp.arraySize);
                infoCursor.x = infoCursor.xMax;
                infoCursor.width = 20;

                if (conditionsProp.arraySize <= 0)
                {
                    if (GUI.Button(infoCursor, "+", EditorStyles.miniButton))
                    {
                        conditionsProp.InsertArrayElementAtIndex(responsesProp.arraySize);
                        responsesProp.InsertArrayElementAtIndex(responsesProp.arraySize);
                    }
                }

                Rect rowArea = GUILayoutUtility.GetLastRect();
                rowArea.yMin += 16;
                Rect rowRect = rowArea;
                rowRect.height = 16;

                for (int i = 0; i < conditionsProp.arraySize; i++)
                {
                    float height = EditorGUI.GetPropertyHeight(conditionsProp.GetArrayElementAtIndex(i));

                    GUILayout.Space(height);
                    Rect indexRect = rowRect;
                    indexRect.width = 20;

                    Rect buttonRect = rowRect;
                    buttonRect.width = 36;
                    buttonRect.x = rowRect.xMax - buttonRect.width;

                    Rect rangeRect = new Rect(indexRect.xMax, rowRect.y,
                        rowRect.width, height) { xMax = buttonRect.xMin - 10 };


                    ValueRangeSetDrawer.DrawRowButtons(buttonRect, conditionsProp, i, false, null, DeleteResponse, InsertResponse);

                    EditorGUI.LabelField(indexRect, new GUIContent(i.ToString()));
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(rangeRect, conditionsProp.GetArrayElementAtIndex(i), GUIContent.none, true);

                    GUI.enabled = true;


                    if (EditorGUI.EndChangeCheck())
                    {
                        conditionsProp.serializedObject.ApplyModifiedProperties();
                    }
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(16*(EditorGUI.indentLevel + 2));
                    EditorGUILayout.PropertyField(responsesProp.GetArrayElementAtIndex(i));
                    rowRect.y = GUILayoutUtility.GetLastRect().yMax;
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnNoConditionsMet"));
            }
            GUILayout.EndVertical();
        }

        private void DrawRanges(SerializedProperty conditionsProp, SerializedProperty responsesProp)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.BeginHorizontal();
            GUILayout.Space(12.0f);
            conditionsProp.isExpanded = EditorGUILayout.Foldout(conditionsProp.isExpanded, "Conditional Responses", true);
            GUILayout.EndHorizontal();
            if (conditionsProp.isExpanded)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("InvokeForAllMetConditions"));
                GUILayout.Label(" ");
                ValueRangeSetDrawer.DrawInfo(GUILayoutUtility.GetLastRect(), conditionsProp, true, InsertResponse);


                Rect rowArea = GUILayoutUtility.GetLastRect();
                rowArea.yMin += 16;
                Rect rowRect = rowArea;
                rowRect.height = 16;
                SerializedProperty previousRange = null;
                SerializedProperty previousOp = null;
                for (int i = 0; i < conditionsProp.arraySize; i++)
                {
                    //rowRect.y = rowArea.y + (rowRect.height * i);
                    SerializedProperty range = conditionsProp.GetArrayElementAtIndex(i)
                        .FindPropertyRelative("Value");
                    SerializedProperty op = conditionsProp.GetArrayElementAtIndex(i)
                        .FindPropertyRelative("Operator");
                    GUILayout.Label(" ");
                    ValueRangeSetDrawer.DrawRow(rowRect, conditionsProp, i, previousRange, previousOp,
                        DeleteResponse, InsertResponse);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(16*(EditorGUI.indentLevel + 2));
                    EditorGUILayout.PropertyField(responsesProp.GetArrayElementAtIndex(i));
                    rowRect.y = GUILayoutUtility.GetLastRect().yMax;
                    GUILayout.EndHorizontal();
                    previousRange = range;
                    previousOp = op;
                }
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnNoConditionsMet"));
            }
            GUILayout.EndVertical();
        }

        private void InsertResponse(int index)
        {
            SerializedProperty responsesProp = serializedObject.FindProperty("Responses");
            responsesProp.InsertArrayElementAtIndex(index);
            //serializedObject.ApplyModifiedProperties();
        }

        private void DeleteResponse(int index)
        {
            SerializedProperty responsesProp = serializedObject.FindProperty("Responses");
            responsesProp.DeleteArrayElementAtIndex(index);
            //serializedObject.ApplyModifiedProperties();
        }
    }
}
