//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   11/14/2014
//-----------------------------------------------------------------------------

using SG.Core;
using SG.Vignettitor.VignetteData;
using UnityEditor;

namespace SG.Vignettitor.Editor
{
    /// <summary>
    /// Handles path changes to vignette files to keep internal structures up 
    /// to date.
    /// </summary>
    public class VignetteAssetPostprocessor : AssetPostprocessor
    {
        /// <summary>
        /// Gets a cleaner identifier for a file by removing the asset 
        /// extension an resources path prexix.
        /// </summary>
        /// <param name="path">Asset path to make more readable.</param>
        /// <returns>The asset path without extension and resource.</returns>
        public static string CleanName(string path)
        {
            return path.Replace(AssetDirectoryUtility.GENERAL_RESOURCE_DIRECTORY, "").
                Replace("."+AssetDirectoryUtility.GENERIC_ASSET_EXTENSION, "");
        }
        
        /// <summary>
        /// Renames graph assets to match the file name and updates the path 
        /// inside of the vignette path to match the asset path whenever a 
        /// vignette file is changed.
        /// </summary>
        /// <param name="importedAssets"></param>
        /// <param name="deletedAssets"></param>
        /// <param name="movedToAssetPaths"></param>
        /// <param name="movedFromAssetPaths"></param>
        static void OnPostprocessAllAssets(string[] importedAssets, 
            string[] deletedAssets, string[] movedToAssetPaths, string[] movedFromAssetPaths)
        {
            // Detecting Scene Moves
            for (int i = 0; i < movedToAssetPaths.Length; i++)
            {
                string toPath = movedToAssetPaths[i];
                string fromPath = movedFromAssetPaths[i];
                if (toPath.EndsWith(AssetDirectoryUtility.GENERIC_ASSET_EXTENSION) && 
                    fromPath.EndsWith(AssetDirectoryUtility.GENERIC_ASSET_EXTENSION) && 
                    toPath != fromPath)
                {
                    //Debug.Log("moved " + toPath);
                    VignetteGraph h = AssetDatabase.LoadAssetAtPath(toPath, typeof(VignetteGraph)) as VignetteGraph;
                    if (h != null)
                    {
                        h.VignettePath = CleanName(toPath);
                        h.VignetteGUID = AssetDatabase.AssetPathToGUID(toPath);
                        string name = System.IO.Path.GetFileNameWithoutExtension(h.VignettePath);
                        h.name = name;
                        h.CollectConnectedNodes();
                    }
                }
            }
            
            for (int i = 0; i < importedAssets.Length; i++)
            {
                if (importedAssets[i].EndsWith(AssetDirectoryUtility.GENERIC_ASSET_EXTENSION))
                {
                    VignetteGraph h = AssetDatabase.LoadAssetAtPath(importedAssets[i], typeof(VignetteGraph)) as VignetteGraph;
                    if (h != null)
                    {
                        h.VignettePath = CleanName(importedAssets[i]);
                        h.VignetteGUID = AssetDatabase.AssetPathToGUID(importedAssets[i]);
                        //string name = System.IO.Path.GetFileNameWithoutExtension(h.VignettePath);
                        // Changing the name here causes an infinite import loop
                        //h.name = name;
                        h.CollectConnectedNodes();
                    }
                }
            }
        }
    }
}
