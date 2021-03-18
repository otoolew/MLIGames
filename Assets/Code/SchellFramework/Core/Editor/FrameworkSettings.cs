// ------------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Eric Policaro
//
//  Created: 9/3/2015 12:09:46 PM
// ------------------------------------------------------------------------------

using System.IO;
using UnityEditor;
using UnityEngine;

namespace SG.Core
{
    /// <summary>
    /// Settings and meta data for the Unity Framework itself.
    /// </summary>
    public class FrameworkSettings : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Project relative location of the Unity Framework.
        /// </summary>
        public string FrameworkRoot = DefaultFrameworkRoot;

        /// <summary>
        /// If true, the project is using Perforce for version control.
        /// This will allow the Framework to generate a .p4ignore file
        /// for typical folders/files that projects can safely ignore/unignore.
        /// </summary>
        public bool UsingPerforce;

        /// <summary>
        /// Load the Framework Settings instance in the project.
        /// There should only ever be one instance of the settings. The location
        /// is stored in EditorPrefs to speed up future checks.
        /// </summary>
        public static FrameworkSettings Load()
        {
            string settingslocation = FindFrameworkSettings();
            var frameworkSettings = AssetDatabase.LoadAssetAtPath<FrameworkSettings>(settingslocation);
            if (frameworkSettings == null)
            {
                frameworkSettings = CreateInstance<FrameworkSettings>();
                EditorAssetDirectoryUtility.CreateDirectoriesAndAsset(
                    frameworkSettings, settingslocation);
            }

            EditorPrefs.SetString(SettingsLocationKey, settingslocation);
            return frameworkSettings;
        }

        private static string FindFrameworkSettings()
        {
            string fromPrefs = EditorPrefs.GetString(SettingsLocationKey);
            if (!string.IsNullOrEmpty(fromPrefs) && File.Exists(fromPrefs))
            {
                return fromPrefs;
            }

            var searchResults = AssetDatabase.FindAssets("t:FrameworkSettings");
            if (searchResults.Length > 0)
            {
                return AssetDatabase.GUIDToAssetPath(searchResults[0]);
            }

            return DefaultFrameworkSettings;
        }

        /// <summary>
        /// Commonly used (documented) path to the Unity Framework.
        /// </summary>
        public const string DefaultFrameworkRoot = "Assets/Code/Framework";

        private const string SettingsLocationKey = "FrameworkSettingsPath";
        private const string DefaultFrameworkSettings = "Assets/Unity Framework.asset";

        public void OnAfterDeserialize()
        {
        }

        public void OnBeforeSerialize()
        {
            ModuleSettings.FrameworkRoot = FrameworkRoot;
        }
    }
}
