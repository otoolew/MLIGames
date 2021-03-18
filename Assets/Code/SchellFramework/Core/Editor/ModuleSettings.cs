// ------------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Eric Policaro
//
//  Created: August 12 2015
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SG.Core
{
    /// <summary>
    /// Specifies how a module settings object should be loaded.
    /// </summary>
    public enum SettingsMode
    {
        /// <summary>
        /// Create a new instance of the settings asset. If an asset with
        /// the same name exists, overwrite it.
        /// </summary>
        Create,

        /// <summary>
        /// Create a new instance of the settings asset. If an asset with
        /// the same name exists, find a unique name to guarantee a 
        /// fresh asset is created.
        /// </summary>
        CreateUnique,

        /// <summary>
        /// Load an asset from the settings folder. If it does not exist, return null.
        /// </summary>
        Load,

        /// <summary>
        /// Load an asset from the settings folder. If it does not exist, create a new one.
        /// </summary>
        LoadOrCreate
    }

    /// <summary>
    /// ModuleSettings manages the loading and creation of <see cref="ScriptableObject"/>
    /// instances that are used as settings for modules.
    /// 
    /// Settings assets are placed in the project based on the framework's
    /// location and the name of the module. Recommended convention dictates 
    /// that the Framework code exists at "Assets/Framework/(Modules)". 
    /// Therefore, settings for a module named "Module" would be located 
    /// at "Assets/Framework/Module/Settings"
    /// 
    /// The Framework root location is controlled at a global level 
    /// by <see cref="ModuleSettings.FrameworkRoot"/>. This path is relative 
    /// to the Unity project.
    /// 
    /// <code language="CSharp" title="Settings Creation example">
    /// // This example creates (or overwrites) a settings object named NewCoreSettings
    /// var moduleSettings = new ModuleSettings("Core");
    /// CoreSettings instance = moduleSettings.Create{CoreSettings}();
    /// </code>
    /// 
    /// <code language="CSharp" title="Load a known settings object">
    /// var moduleSettings = new ModuleSettings("Core", "NotifySettings");
    /// // If NotifySettings does not yet exist, it will be created.
    /// var notifySettings = moduleSettings.Load{NotifySettings}(SettingsMode.LoadOrCreate);
    /// </code>
    /// </summary>
    public class ModuleSettings
    {
        /// <summary>
        /// Gets or sets the root directory of the Framework code,
        /// relative to the Unity project.
        /// 
        /// Example, if your modules are located at "Assets/Scripts/Framework/Core",
        /// the FrameworkRoot would be "Assets/Scripts/Framework".
        /// </summary>
        public static string FrameworkRoot { get; set; }

        /// <summary>
        /// Gets the default path to the Framework root.
        /// <see cref="ModuleSettings.FrameworkRoot"/>
        /// </summary>
        public static string DefaultFrameworkRoot
        {
            get { return "Assets/Code/Framework"; }
        }

        /// <summary>
        /// Get the path to settings folder for a module
        /// </summary>
        /// <param name="moduleName">Name of the module</param>
        /// <returns>Project relative path to the module's settings</returns>
        public static string GetSettingsPath(string moduleName)
        {
            return GetModuleSettingsPath(moduleName);
        }

        /// <summary>
        /// Gets the default name of the module settings folder.
        /// </summary>
        public static string DefaultSettingsFolder
        {
            get { return "Settings"; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleSettings"/> class.
        /// A default name is constructed for the asset: "NewModuleNameSettings.asset"
        /// </summary>
        /// <param name="moduleName">Name of the module</param>
        public ModuleSettings(string moduleName)
        {
            if (string.IsNullOrEmpty(moduleName))
                throw new ArgumentException("Module name must be specified (cannot be null or empty).");

            Subfolder = "";
            ModuleName = moduleName;
        }

        /// <summary>
        /// Gets or sets the name of the module that uses the settings object.
        /// </summary>
        public string ModuleName { get; private set; }

        /// <summary>
        /// Gets or sets a subfolder to place the asset. Settings are placed
        /// at {FrameworkRoot}/{ModuleName}/Settings/{Subfolder} (if subfolder 
        /// is not empty).
        /// </summary>
        public string Subfolder { get; set; }

        /// <summary>
        /// Gets or sets a name to use in place of the default asset name.
        /// This will be used if it is not empty or null.
        /// </summary>
        public string DefaultNameOverride { get; set; }
        
        private string DefaultAssetName
        {
            get { return "New" + ModuleName + "Settings"; }
        }

        private string PathToSettings
        {
            get { return GetModuleSettingsPath(ModuleName, Subfolder); }
        }

        /// <summary>
        ///     Loads an asset with the provided <see cref="SettingsMode"/>.
        /// </summary>
        /// <typeparam name="T">
        ///     Type of object to load
        /// </typeparam>
        /// <param name="mode">
        ///     A <see cref="SettingsMode"/> that specifies how to perform the load.
        /// </param>
        /// <param name="assetName">
        ///     Name of the asset to load.
        /// </param>
        /// <returns>
        ///     Instance of the object or null if can't be found;
        /// </returns>
        /// <Exception cref="ArgumentException">
        ///     Thrown if given mode is not valid
        /// </Exception>
        public T Load<T>(SettingsMode mode, string assetName) where T : ScriptableObject
        {
            switch (mode)
            {
                case SettingsMode.Create:
                {
                    return Create<T>(assetName);
                }
                case SettingsMode.CreateUnique:
                {
                    return CreateUnique<T>(assetName);
                }
                case SettingsMode.Load:
                {
                    return LoadAsset<T>(assetName);
                }
                case SettingsMode.LoadOrCreate:
                {
                    return LoadAsset<T>(assetName) ?? Create<T>(assetName);
                }
                default:
                {
                    throw new ArgumentException("Specified mode is not valid", "mode");
                }
            }
        }

        /// <summary>
        /// Loads all settings of type {{T}}.
        /// </summary>
        /// <typeparam name="T">Type of object to load</typeparam>
        /// <returns>Array of all settings objects of type T or an empty array
        /// if no settings of the given type exist.</returns>
        public T[] LoadAll<T>() where T : ScriptableObject
        {
            string settingsPath = GetModuleSettingsPath(ModuleName, Subfolder);
            if (!Directory.Exists(settingsPath))
                return new T[0];

            var result = new List<T>();
            var allFiles = Directory.GetFiles(settingsPath);
            foreach (var file in allFiles.Select(f => new FileInfo(f)))
            {
                var settings = Load<T>(SettingsMode.Load, file.Name);
                if (settings != null)
                    result.Add(settings);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Creates the settings object specified by type T with the default name.
        /// </summary>
        /// <typeparam name="T">Type of scriptable object to create</typeparam>
        /// <returns>Instance created</returns>
        public T Create<T>() where T : ScriptableObject
        {
            return Create<T>("");
        }

        /// <summary>
        /// Creates the settings object specified by type T with a specified name.
        /// </summary>
        /// 
        /// <param name="assetName">Name of the asset it; Iftaken, 
        /// the existing asset is replaced.</param>
        /// <typeparam name="T">Type of scriptable object to create</typeparam>
        /// <returns>Instance created</returns>
        public T Create<T>(string assetName) where T : ScriptableObject
        {
            return _Create<T>(false, assetName);
        }

        /// <summary>
        /// Creates the settings object specified by type T. The default asset
        /// name will be used and made unique if necessary.
        /// </summary>
        /// <typeparam name="T">Type of scriptable object to create</typeparam>
        /// <returns>Instance created</returns>
        public T CreateUnique<T>() where T : ScriptableObject
        {
            return CreateUnique<T>("");
        }

        /// <summary>
        /// Creates the settings object specified by type T. 
        /// </summary>
        /// <param name="assetName">Name of the asset. If the provided name
        /// is taken, it will be made unique.</param>
        /// <typeparam name="T">Type of scriptable object to create</typeparam>
        /// <returns>Instance created</returns>
        public T CreateUnique<T>(string assetName) where T : ScriptableObject
        {
            return _Create<T>(true, assetName);
        }

        public T CreateTemporary<T>(string name="") where T : ScriptableObject
        {
            var so = ScriptableObject.CreateInstance<T>();
            so.name = string.IsNullOrEmpty(name) ? DefaultAssetName : name;

            return so;
        }

        private T _Create<T>(bool isUnique, string assetName) where T : ScriptableObject
        {
            T settings = ScriptableObject.CreateInstance<T>();
            string path = GetAssetPath(isUnique, assetName);

            EditorAssetDirectoryUtility.CreateDirectoriesAndAsset(settings, path);
            return settings;
        }

        private T LoadAsset<T>(string assetName) where T : ScriptableObject
        {
            string path = GetAssetPath(false, assetName);
            try
            {
                return AssetDatabase.LoadAssetAtPath<T>(path);
            }
            catch (Exception ex)
            {
                Log.Error("Error loading asset: {0}\n{1}", path, ex.Message);
                return null;
            }
        }

        private string GetAssetPath(bool makeUnique, string assetName)
        {
            string fileName = GetFinalAssetName(
                assetName, PathToSettings, makeUnique);

            return AssetDirectoryUtility.NormalizePathSeparators(fileName);
        }

        private string GetFinalAssetName(string baseAssetName,
                                         string dir,
                                         bool makeUnique)
        {
            // ChangeExtension will guarantee that we have the proper extension
            // regardless of input. It's a slightly cleaner way than doing StartWith checks.
            string pathToAsset = Path.Combine(dir, ApplyNameDefaults(baseAssetName));
            string path = Path.ChangeExtension(pathToAsset, AssetExt);
            if (makeUnique)
            {
                path = EditorAssetDirectoryUtility.GenerateUniqueAssetPath(path);
            }

            return path;
        }

        private string ApplyNameDefaults(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                return string.IsNullOrEmpty(DefaultNameOverride)
                        ? DefaultAssetName
                        : DefaultNameOverride;
            }

            return assetName;
        }

        private static string GetModuleSettingsPath(string moduleName, string subfolder = "")
        {
            return AssetDirectoryUtility.CombineAssetPaths(
                    FrameworkRoot, moduleName, DefaultSettingsFolder, subfolder);
        }

        private const string AssetExt = ".asset";
        private static readonly Notify Log = NotifyManager.GetInstance("Core");
    }
}