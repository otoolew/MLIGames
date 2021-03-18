//------------------------------------------------------------------------------
// Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
// Contact: Max Golden
//
// Created: 11/14/2016 9:40 AM
//------------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace SG.Core.AssetIcons
{
    /// <summary>
    /// Contains InitializeOnLoadMethod to generate asset icons with the
    /// GenerateAssetIcon attribute and utility functions to support checking
    /// and creating those icons
    /// </summary>
    public static class AssetIconGenerator
    {
        private const string GIZMOS_PATH = "Assets/Gizmos/";
        private const string ICON_PATH_END = " icon.png";

        private static readonly Notify Log = NotifyManager.GetInstance("Core.AssetIcons");

        /// <summary>
        /// Gets the name of the asset icon for a given type from the root of
        /// the project directory
        /// </summary>
        /// <param name="type">Type whose icon name you want to get</param>
        /// <returns>A string corresponding to the full name of the icon from
        /// the root of the project directory</returns>
        public static string GetFullAssetIconName(Type type)
        {
            return GIZMOS_PATH + type.Name + ICON_PATH_END;
        }

        /// <summary>
        /// Checks to see if there is a file corresponding to the expected path
        /// of an asset icon for the given type
        /// </summary>
        /// <param name="type">Type whose icon you want to check</param>
        /// <returns>True if the icon exists, false otherwise</returns>
        public static bool HasAssetIcon(Type type)
        {
            return File.Exists(GetFullAssetIconName(type));
        }

        /// <summary>
        /// Creates the path Assets/Gizmos/ if it does not exist
        /// </summary>
        public static void CreateGizmosDirectoryIfNeeded()
        {
            if (!Directory.Exists(GIZMOS_PATH))
                Directory.CreateDirectory(GIZMOS_PATH);
        }

        /// <summary>
        /// Tries to generate an asset icon for all classes with the
        /// GenerateAssetIconAttribute if no icon currently exists
        /// </summary>
        [InitializeOnLoadMethod]
        public static void GenerateNewAssetIcons()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int i = 0; i < assemblies.Length; i++)
            {
                Type[] types = AssemblyUtility.GetTypes(assemblies[i]);

                for (int t = 0; t < types.Length; t++)
                {
                    IEnumerable<GenerateAssetIconAttribute> attributes =
                        types[t].GetCustomAttributes(typeof(GenerateAssetIconAttribute), true)
                        .Cast<GenerateAssetIconAttribute>();

                    foreach (GenerateAssetIconAttribute att in attributes)
                    {
                        if (!HasAssetIcon(types[t]))
                        {
                            if (GenerateAssetIcon(att.IconTexture, types[t]))
                                Log.Debug("Generated icon for '{0}'", types[t].Name);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates an asset icon corresponding to the given texture for the given type
        /// </summary>
        /// <param name="texture">Texture from which to create the icon</param>
        /// <param name="type">Type whose icon you want to generate</param>
        /// <returns>True if a new icon was successfully created, false otherwise</returns>
        public static bool GenerateAssetIcon(Texture2D texture, Type type)
        {
            string iconName = GetFullAssetIconName(type);

            try
            {
                CreateGizmosDirectoryIfNeeded();

                File.WriteAllBytes(iconName, texture.EncodeToPNG());
                AssetDatabase.ImportAsset(iconName);

                // Make the texture a sprite, turn of mipmaps, etc.
                TextureImporter importer = AssetImporter.GetAtPath(iconName) as TextureImporter;
                importer.textureType = TextureImporterType.Sprite;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();

                return true;
            }
            catch (Exception e)
            {
                Log.Error("Error generating asset icon for '{0}':\n{1}", type.Name, e.Message);
                return false;
            }
        }
    }
}