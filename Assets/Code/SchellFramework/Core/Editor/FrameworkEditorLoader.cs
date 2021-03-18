//------------------------------------------------------------------------------
// Copyright © 2017 Schell Games, LLC. All Rights Reserved.
//
// Contact: Eric Policaro
//
// Created: June 2017
//------------------------------------------------------------------------------

using UnityEditor;

namespace SG.Core
{
    /// <summary>
    /// Loads the default notify values on editor load, and
    /// and takes care of the global framework settings.
    /// </summary>
    [InitializeOnLoad]
    public class FrameworkEditorLoader
    {
        /// <summary>
        /// Adds the update function to the editor application, 
        /// so that resources can be loaded for config variables
        /// </summary>
        static FrameworkEditorLoader()
        {
            EditorApplication.update += Update;
        }

        private static void Update()
        {
            InitializeFramework();
            EditorApplication.update -= Update;
        }

        /// <summary>
        /// Initialize all Core Framework systems.
        /// </summary>
        public static void InitializeFramework()
        {
            AssetDatabase.Refresh();

            var frameworkSettings = FrameworkSettings.Load();

            ModuleSettings.FrameworkRoot = frameworkSettings.FrameworkRoot;
            NotifySettingsEditor.LoadDefaultSettings();
            IgnoreLoader.CreateDefaultP4Ignore(frameworkSettings.UsingPerforce);
        }
    }
}
