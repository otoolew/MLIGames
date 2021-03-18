//-----------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   12/10/2015 12:44:17 PM
//-----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SG.Core;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SG.GlobalEvents.Editor
{
    [CustomEditor(typeof(BaseGlobalEvent), true)]
    public class GlobalEventEditor : UnityEditor.Editor
    {
        private Vector2 typeScroll;
        private FieldInfo listenersField;

        private GUIStyle style;

        private bool pickingType;
        private Rect pickerRect;

        private string filter = "";

        private bool historyExpanded;
        private readonly List<bool> historyExpandedIndices = new List<bool>();

        private bool listenersExpanded;

        [SerializeField]
        private ScriptableObject ThingToReload;
        [SerializeField]
        private string TypeName;

        protected void OnEnable()
        {
            if (ThingToReload != null)
            {
                SetType(ThingToReload, TypeName);
                ThingToReload = null;
                TypeName = null;
                EditorApplication.delayCall += () =>
                    EditorApplication.ExecuteMenuItem("Assets/Reimport");
            }
        }
        
        private void SetType(ScriptableObject obj, string passingTypeName)
        {
            Type passingType = Type.GetType(passingTypeName);
            ThingToReload = obj;
            TypeName = passingTypeName;
            Type eventType = GlobalEventTypeGenerator.FindGlobalEventTypeForPassingType(passingType);
            if (eventType != null)
            {
                SerializedObject so = new SerializedObject(obj);
                ScriptableObject temp = CreateInstance(eventType);
                MonoScript ms = MonoScript.FromScriptableObject(temp);
                so.FindProperty("m_Script").objectReferenceValue = ms;
                so.ApplyModifiedProperties();
                ThingToReload = null;
                TypeName = null;
            }
        }

        #region -- Drawing ----------------------------------------------------
        private void DrawArgument(BaseGlobalEvent ge, Type argType)
        {
            SerializedProperty argProp = serializedObject.FindProperty("lastArgument");

            if (argProp != null)
            {
                object propValue = argProp.GetPropertyValue();
                if (propValue != null && propValue.GetType() != argType)
                {
                    argProp.ClearPropertyValue();
                    serializedObject.ApplyModifiedProperties();
                }

                PropertyInfo pi = ge.GetType().GetProperty("LastArgument");
                object arg = pi.GetValue(ge, new object[0]);

                GUILayout.BeginHorizontal(GUI.skin.box);
                GUILayout.Space(10);
                GUILayout.BeginVertical();
                if (argProp.propertyType == SerializedPropertyType.ObjectReference)
                {
                    EditorGUI.BeginChangeCheck();
                    arg = EditorGUILayout.ObjectField(new GUIContent("Argument"), (Object)arg, argType, true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        pi.SetValue(ge, arg, new object[] { });
                    }
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(argProp, new GUIContent("Argument"), true);
                    if (EditorGUI.EndChangeCheck())
                        serializedObject.ApplyModifiedProperties();
                }

                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(" ");
                if (GUILayout.Button("Raise Event"))
                {
                    ge.RaiseGeneric(arg);
                    //MethodInfo mi = ge.GetType().GetMethod("Raise", new[] { argType });
                    //mi.Invoke(ge, new [] { arg });
                }
                FieldInfo sfi = typeof(BaseGlobalEvent).GetField("stuck", BindingFlags.Instance | BindingFlags.NonPublic);
                bool existing = sfi != null && (bool)sfi.GetValue(ge);
                GUI.enabled = existing;
                if (GUILayout.Button("Clear Sticky"))
                {
                    ge.ClearStickyEvents();
                }
                GUI.enabled = true;
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.EndHorizontal();
            }
            else
            {
                if (GUILayout.Button("Raise Event"))
                {
                    ge.RaiseGeneric(null);
                }
            }
        }

        private void DrawHistory(BaseGlobalEvent ge)
        {
#if UNITY_EDITOR && !GLOBAL_EVENT_DISABLE_HISTORY
            DateTime startTime = DateTime.Now.AddSeconds(-Time.realtimeSinceStartup);
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            historyExpanded = EditorGUILayout.Foldout(historyExpanded, "History (" + ge.EventRecorder.ActionCount + ") (Game started at " + startTime.ToString("HH:mm:ss.ff") + ")", true);
            GUILayout.EndHorizontal();

            if (historyExpanded)
            {
                while (historyExpandedIndices.Count < ge.EventRecorder.RecentRecords.Length)
                    historyExpandedIndices.Add(false);
                while (historyExpandedIndices.Count > ge.EventRecorder.RecentRecords.Length)
                    historyExpandedIndices.RemoveAt(historyExpandedIndices.Count - 1);

                for (int i = 0; i < ge.EventRecorder.RecentRecords.Length; i++)
                {
                    DateTime eventTime = startTime.AddSeconds(ge.EventRecorder.RecentRecords[i].Time);

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    historyExpandedIndices[i] = EditorGUILayout.Foldout(historyExpandedIndices[i], ge.EventRecorder.RecentRecords[i].ActionName + " At Time: " + eventTime.ToString("HH:mm:ss.ff") + " (Frame: " + ge.EventRecorder.RecentRecords[i].Frame + ")", true);
                    GUILayout.EndHorizontal();
                    if (historyExpandedIndices[i])
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(30);
                        GUILayout.Label(ge.EventRecorder.RecentRecords[i].Trace);
                        GUILayout.EndHorizontal();
                    }
                }
            }
            GUILayout.EndVertical();
#endif
        }

        private void DrawListeners(BaseGlobalEvent ge)
        {
            if (listenersField == null)
            {
                listenersField = ge.GetType().BaseType.GetField("genericListeners", BindingFlags.NonPublic | BindingFlags.Instance);
                if (listenersField == null)
                    listenersField = ge.GetType().GetField("listeners", BindingFlags.NonPublic | BindingFlags.Instance);
                if (listenersField == null)
                {
                    GUILayout.Label("Can not find event field of global event manager.", EditorStyles.helpBox);
                    return;
                }
            }
            IList events = listenersField.GetValue(ge) as IList ?? new object[0];

            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            listenersExpanded = EditorGUILayout.Foldout(listenersExpanded, new GUIContent("Registered Listeners (" + events.Count + ")"), true);
            GUILayout.EndHorizontal();

            if (listenersExpanded)
            {
                TextAnchor oldButtonAlign = GUI.skin.button.alignment;
                GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginVertical();
                for (int i = 0; i < events.Count; i++)
                {
                    // 
                    DrawListenerButton(events[i]);
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUI.skin.button.alignment = oldButtonAlign;
            }
            GUILayout.EndVertical();
        }

        private void DrawListenerButton(object target)
        {
            FieldInfo listenerFI = target.GetType()
                        .GetField("listener", BindingFlags.Instance | BindingFlags.NonPublic);
            PropertyInfo pri = target.GetType().GetProperty("Priority", BindingFlags.Instance | BindingFlags.Public);
            MonoBehaviour targetMB = listenerFI == null ? null : listenerFI.GetValue(target) as MonoBehaviour;
            string function = "";
            string typeName = "";
            if (targetMB == null)
            {
                FieldInfo delFI = target.GetType().GetField("callback", BindingFlags.Instance | BindingFlags.NonPublic);
                Delegate d = delFI.GetValue(target) as Delegate;
                if (d.Target is MonoBehaviour)
                    targetMB = (MonoBehaviour)d.Target;
                function = "." + d.Method.Name;
                typeName = d.Target.GetType().Name;
            }
            else
            {
                typeName = targetMB.GetType().Name;
            }
            string mbName = targetMB == null ? 
                "No Unity Object" : targetMB.name;

            bool wasEnabled = GUI.enabled;
            GUI.enabled = targetMB != null;
            if (GUILayout.Button(mbName + " - " + typeName + function + " | priority: " + pri.GetValue(target, new object[0])))
                EditorGUIUtility.PingObject(targetMB);
            GUI.enabled = wasEnabled;
        }

        private void DrawSectionSpace()
        { GUILayout.Space(8); }

        private void DrawRuntime(BaseGlobalEvent ge, Type argType)
        {
            GUILayout.Space(20);
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Runtime Debugging");
            DrawSectionSpace();

            DrawArgument(ge, argType);
            DrawSectionSpace();
            DrawHistory(ge);
            DrawSectionSpace();
            DrawListeners(ge);

            GUILayout.EndVertical();
        }

        private void DrawArgumentTypeSelector()
        {
            GUI.FocusControl("TextFilter");
            Rect r = pickerRect;
            r.height += 350;
            r.width = 300;

            GUI.BeginGroup(r);
            GUILayout.BeginArea(r, GUI.skin.box);
            if (GUILayout.Button("Cancel"))
            {
                pickingType = false;
            }
            GUI.SetNextControlName("TextFilter");
            TextAnchor oldButtonAlign = GUI.skin.button.alignment;
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            filter = GUILayout.TextField(filter);
            typeScroll = GUILayout.BeginScrollView(typeScroll, GUI.skin.window);
            for (int i = 0; i < GlobalEventTypeGenerator.PotentialDataTypes.Count; i++)
            {
                Type type = GlobalEventTypeGenerator.PotentialDataTypes[i];
                if (filter.Length == 0 || type.Name.ToLower().Contains(filter.ToLower()))
                {
                    if (GlobalEventTypeGenerator.ExistingGlobalEventTypeCount == i)
                    {
                        GUILayout.Box("----------------------------------------------------");
                    }
                    if (GUILayout.Button(type.Name + (i >= GlobalEventTypeGenerator.ExistingGlobalEventTypeCount ? "    (Create)" : "")))
                    {
                        Type eventType = GlobalEventTypeGenerator.FindGlobalEventTypeForPassingType(type);
                        if (eventType != null)
                        {
                            SerializedObject so = new SerializedObject(target);
                            ScriptableObject temp = CreateInstance(eventType);
                            MonoScript ms = MonoScript.FromScriptableObject(temp);
                            so.FindProperty("m_Script").objectReferenceValue = ms;
                            so.ApplyModifiedProperties();
                            EditorApplication.ExecuteMenuItem("Assets/Reimport");
                        }
                        else
                        {
                            GlobalEventTypeGenerator.CreateEventFile(type);
                            GlobalEventTypeGenerator.CreateListenerFile(type);
                            AssetDatabase.Refresh();
                            SetType(target as ScriptableObject, type.AssemblyQualifiedName);
                        }
                    }
                }
            }
            GUI.skin.button.alignment = oldButtonAlign;

            GUILayout.EndScrollView();
            GUILayout.EndArea();
            GUI.EndGroup();
        }

        private void CacheStyles()
        {
            if (style == null)
            {
                style = new GUIStyle(GUI.skin.textArea);
                style.wordWrap = true;
            }
        }

        public override void OnInspectorGUI()
        {
            CacheStyles();
            
            BaseGlobalEvent ge = target as BaseGlobalEvent;
            if (ge == null)
            {
                EditorGUILayout.HelpBox("The target is not a GlobalEvent", MessageType.Error);
                return;
            }
            Type globalEventType = ge.GetType();

            // Disable drawing if there is a pop-up
            GUI.enabled = !pickingType;
            
            // Draw base
            EditorGUI.BeginChangeCheck();
            DrawPropertiesExcluding(serializedObject, "lastArgument");
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();


            //Draw notes
            EditorGUILayout.LabelField("Developer Notes:");
            ge.Notes = EditorGUILayout.TextArea(ge.Notes, style);

            // Draw type selection button
            GUILayout.BeginHorizontal();
            pickerRect = GUILayoutUtility.GetRect(new GUIContent("Select Data Type"), GUI.skin.label);
            GUI.Label(pickerRect, "Select Data Type");
            string typeName = "Void";
            Type argType = null;
            if (globalEventType.BaseType != null)
            {
                Type[] generics = globalEventType.BaseType.GetGenericArguments();
                if (generics.Length > 0)
                {
                    argType = generics[0];
                    typeName = generics[0].Name;
                }
            }

            if (!pickingType && GUILayout.Button(typeName))
            {
                GlobalEventTypeGenerator.GenerateTypeList();
                pickingType = true;
            }
            GUILayout.EndHorizontal();

            // Draw runtime debugging
            if (Application.isPlaying)
            {
                DrawRuntime(ge, argType);
            }

            // Re-enable if a pop-up disabled
            GUI.enabled = true;

            // Draw pop-up
            if (pickingType)
            {
                DrawArgumentTypeSelector();
            }
        }
        #endregion -- Drawing -------------------------------------------------
    }
}
