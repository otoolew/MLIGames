//-----------------------------------------------------------------
//  Copyright © 2012 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   05/23/2010
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace SG.Core
{
    public static class AssetDirectoryUtility
    {
        #region Constants
        /// <summary>
        /// The extension used by Unity for meta files.
        /// </summary>
        public const string META_EXTENSION = ".meta";


        /// <summary>
        /// The standard path for resources
        /// </summary>
        public const string GENERAL_RESOURCE_DIRECTORY = "Assets/Resources/";


        /// <summary>
        /// The character used to separate directories in Unity's pathing.
        /// </summary>
        public const char UNITY_DIRECTORY_SEPARATOR = '/';

        public const string GENERIC_ASSET_EXTENSION = "asset";
        #endregion

        /// <summary>
        /// Gets all files in the Asset directory matching the given pattern. This is a recursive search.
        /// </summary>
        /// <param name="assetFolder">
        /// Asset folder to search.
        /// Paths are project relative (ex. "Assets/Framework") will search the Assets/Framework directory only.
        /// </param>
        /// <param name="searchPattern">Search pattern</param>
        /// <param name="trimPaths">[Default: true] If true, trims file paths to project local paths. 
        /// <see cref="AssetDirectoryUtility.GetProjectLocalPath"/>
        /// </param>
        /// <returns>The asset file paths</returns>
        public static string[] GetAssetFiles(string assetFolder, string searchPattern, bool trimPaths = true)
        {
            if (!Directory.Exists(assetFolder))
            {
                return new string[] { };
            }

            string[] paths = Directory.GetFiles(assetFolder, searchPattern, SearchOption.AllDirectories);
            if (!trimPaths)
                return paths;

            var trimmedPaths = new List<string>();
            foreach (var path in paths)
            {
                trimmedPaths.Add(GetProjectLocalPath(path));
            }

            return trimmedPaths.ToArray();
        }

        /// <summary>
        /// Determines whether the file specified by the given path is a Unity meta file.
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns><c>true</c>, file is a meta file; <c>false</c> otherwise</returns>
        public static bool IsMetaFile(string path)
        {
            return path.EndsWith(META_EXTENSION);
        }

        /// <summary>
        /// Determines whether the given path is svn meta data.
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns><c>true</c>, path is svn meta data; <c>false</c> otherwiese</returns>
        public static bool IsSVNFile(string path)
        {
            return path.Contains(".svn");
        }


        /// <summary>
        /// The name of the Asset directory inside of the Unity project.  This 
        /// is typically "Assets".
        /// </summary>
        public static string AssetDirectory
        {
            get { return Path.GetFileName(Application.dataPath); }
        }


        /// <summary>
        /// Returns a path relative to the Unity project's Assets/Resources directory without extension.
        /// Usable for converting between an AssetDatabase path and a Resources path.
        /// Only applies if path is actually inside Assets/Resources and not another Resources directory.
        /// </summary>
        /// <param name="unityPath">
        /// A Unity path to a file as expected by AssetDatabase.
        /// (ex. "Assets/Resources/Animation/MyAnim.anim")
        /// </param>
        /// <returns>
        /// The path to the specified file, as usable by a Unity resources relative to the Unity project's
        /// Assets directory. (ex: "Animation/MyAnim.anim").
        /// </returns>
        public static string GetResourceRelativePath(string unityPath)
        {
            int extension = unityPath.LastIndexOf('.');
            string result = unityPath
                .Substring(0, unityPath.Length - (unityPath.Length - extension))
                .Replace(GENERAL_RESOURCE_DIRECTORY, string.Empty);
            return result;
        }

        /// <summary>
        /// Normalizes the path separators so that they all match <see cref="Path.DirectorySeparatorChar"/>.
        /// </summary>
        /// <param name="path">The path to normalize</param>
        /// <returns>Unity compatible file path</returns>
        public static string NormalizePathSeparators(string path)
        {
            return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Returns a path relative to the Unity project directory.
        /// </summary>
        /// <param name="fullPath">
        /// A full path to a file.
        /// (ex. "C:\working\UnityProj\Assets\Art\Animations\MyAnim.anim")
        /// </param>
        /// <returns>
        /// The path to the specified file, relative to the Unity project 
        /// directory. (ex: "Assets/Art/Animation/MyAnim.anim").
        /// </returns>
        public static string GetProjectLocalPath(string fullPath)
        {
            string result = fullPath
                .Replace(Path.DirectorySeparatorChar, UNITY_DIRECTORY_SEPARATOR)
                .Replace(Application.dataPath, AssetDirectory)
                .TrimStart(UNITY_DIRECTORY_SEPARATOR);
            return result;
        }

        /// <summary>
        /// Combine a list of path components into a single path using the
        /// correct separator for unity assets.
        /// </summary>
        /// <param name="pathComponents">A comma-separated list of strings to
        /// combine into a single path.</param>
        /// <returns></returns>
        public static string CombineAssetPaths(params string[] pathComponents)
        {
            if (pathComponents.Length < 1)
                throw new System.ArgumentException("At least one component must be provided!");

            var combinedPath = new StringBuilder(pathComponents[0]);
            for (int i = 1; i < pathComponents.Length; i++)
            {
                combinedPath.Append("/"); // unity wants forward slashes in asset paths
                combinedPath.Append(pathComponents[i]);
            }
            return combinedPath.ToString();
        }



        /// <summary>
        /// Returns the full path of the specified UnityAssets-relative path.
        /// </summary>
        /// <param name="localPath">
        /// The path to a file inside of Unity's Assets directory. 
        /// (ex: "Art/Animations/MyAnim.anim")
        /// </param>
        /// <returns>
        /// The full path to the specified local path. 
        /// (ex. "C:\working\UnityProj\Assets\Art\Animations\MyAnim.anim")
        /// </returns>
        public static string GetFullPath(string localPath)
        {
            return Path.Combine(Application.dataPath, localPath);
        }
    }
}
