//------------------------------------------------------------------------------
// Copyright © 2017 Schell Games, LLC. All Rights Reserved.
//
// Contact: Eric Policaro
//
// Created: June 2017
//------------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace SG.Core
{
    [CustomEditor(typeof(NotifySettings))]
    class NotifySettingsEditor : Editor
    {
        [MenuItem("Framework/Core/Select Notify Settings")]
        public static void SelectBuildScript()
        {
            var obj = NotifySettings.Load<NotifySettings>(
                SettingsMode.Load, SettingsName);

            if (obj != null)
            {
                EditorGUIUtility.PingObject(obj);
                Selection.activeObject = obj;
            }
        }

        void OnEnable()
        {
            _target = (NotifySettings)target;
            SortNotifyOverrides();

            _severityOverrideList = new ReorderableList(
                serializedObject, 
                serializedObject.FindProperty("InstanceSettings"), 
                true, true, true, true);

            _severityOverrideList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = _severityOverrideList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                float severityW = SeverityWidth;
                float textW = rect.width - severityW;
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, textW, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("NotifyName"), GUIContent.none);
                EditorGUI.PropertyField(
                    new Rect(rect.x + textW, rect.y, severityW, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("MinimumSeverity"), GUIContent.none);
            };

            _severityOverrideList.onAddCallback = l =>
            {
                var index = l.serializedProperty.arraySize;
                l.serializedProperty.arraySize++;
                l.index = index;
                var element = l.serializedProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("NotifyName").stringValue = "<NotifyLogName>";
                element.FindPropertyRelative("MinimumSeverity").enumValueIndex = 0;
            };

            _severityOverrideList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Notify Severity Overrides");
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawGlobalSeverity();
            _severityOverrideList.DoLayoutList();

            GUI.enabled = (EditorApplication.isPlaying == false);
            if (GUILayout.Button("Sort Override Settings", GUILayout.Width(150f)))
            {
                SortNotifyOverrides();
            }

            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            
            if (GUILayout.Button("Update Notify Settings..."))
            {
                UpdateNotifySettings();
            }

            GUI.enabled = true;

            EditorUtility.SetDirty(_target);
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGlobalSeverity()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Global Minimum Severity", GUILayout.Width(SeverityLabelW));
            _target.GlobalMinimumSeverity = (NotifySeverity)EditorGUILayout.EnumPopup(
                _target.GlobalMinimumSeverity, GUILayout.Width(SeverityWidth));
            EditorGUILayout.EndHorizontal();
        }

        private void UpdateNotifySettings()
        {
            NotifyManager.InitFromLocalSettings(_target);
        }

        /// <summary>
        /// Updates the active Notify configuration from the default
        /// NotifySettings file stored in Resources.
        /// </summary>
        public static void LoadDefaultSettings()
        {
            var settings = NotifySettings.Load<NotifySettings>(
                SettingsMode.Load, SettingsName);

            if (settings == null)
                settings = NotifySettings.CreateTemporary<NotifySettings>();
            
            NotifyManager.InitFromLocalSettings(settings);
        }

        private void SortNotifyOverrides()
        {
            _target.Sort();
        }

        private static ModuleSettings NotifySettings
        {
            get
            {
                return new ModuleSettings("Core")
                {
                    Subfolder = "Resources"
                };
            }
        }

        private NotifySettings _target;
        private ReorderableList _severityOverrideList;

        private const float SeverityWidth = 75f;
        private const float SeverityLabelW = 150f;

        private const string SettingsName = "NotifySettings";
    }
}
