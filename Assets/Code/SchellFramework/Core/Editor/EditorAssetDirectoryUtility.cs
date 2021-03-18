//-----------------------------------------------------------------
//  Copyright © 2012 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple, Eric Policaro
//  Date:   05/23/2010
//-----------------------------------------------------------------

using System.IO;
using UnityEditor;
using UnityEngine;

namespace SG.Core
{
    /// <summary>
    /// Editor-only utilities
    /// </summary>
    public static class EditorAssetDirectoryUtility
    {
        /// <summary>
        /// Creates all of the folders necssary to house the specified path, 
        /// and then creates the asset.
        /// </summary>
        /// <param name="asset">Asset to save to disk.</param>
        /// <param name="unityPath">
        /// A Unity path of the asset.
        /// (ex. "Assets/Resources/Animation/MyAnim.anim")
        /// This will create the Assets, Resources, and/or Animation directories if any does not exist.
        /// </param>
        public static void CreateDirectoriesAndAsset(Object asset, string unityPath)
        {
            var normalizedPath = AssetDirectoryUtility.NormalizePathSeparators(unityPath);
            var assetFileInfo = new FileInfo(normalizedPath);

            // FileInfo.Directory returns null if the file is at the root.
            if (assetFileInfo.Directory != null)
                assetFileInfo.Directory.Create();

            AssetDatabase.CreateAsset(asset, normalizedPath);
        }

        /// <summary>
        /// Creates all of the folders necessary for the specified path
        /// as a directory.
        /// </summary>
        /// <param name="unityPath">
        /// A Unity path for the directory.
        /// (ex. "Assets/Resources/Animation")
        /// This will create the Assets, Resources, and/or Animation directories if any does not exist.
        /// </param>
        public static void CreateDirectories(string unityPath)
        {
            string normalizedPath = AssetDirectoryUtility.NormalizePathSeparators(unityPath);
            DirectoryInfo assetDirectoryInfo = new DirectoryInfo(normalizedPath);
            assetDirectoryInfo.Create();
        }

        /// <summary>
        /// Creates a unique asset path using the provided path.
        /// If the asset doesn't exist, the original path is returned.
        /// </summary>
        /// <remarks>
        /// This is a more tolerant version of Unity's method which isn't
        /// well documented and returns an empty string if the path
        /// already exists.
        /// </remarks>
        /// <param name="path">Original path</param>
        /// <returns>Unique asset path.</returns>
        public static string GenerateUniqueAssetPath(string path)
        {
            string result = AssetDatabase.GenerateUniqueAssetPath(path);
            return string.IsNullOrEmpty(result) ? path : result;
        }
    }
}
