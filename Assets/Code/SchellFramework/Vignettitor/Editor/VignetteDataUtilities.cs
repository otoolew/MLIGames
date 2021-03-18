//-----------------------------------------------------------------------------
//  Copyright © 2013 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   02/07/2013
//-----------------------------------------------------------------------------

using SG.Vignettitor.VignetteData;
using UnityEditor;
using UnityEngine;

namespace SG.Vignettitor.Editor
{
    /// <summary>
    /// Provides a set of functions to modify and save vignette data objects.
    /// </summary>
    public static class VignetteDataUtilities
    {
        /// <summary>
        /// Writes out a vignette graph to disk as a single file. The output 
        /// file will contain serialized versions of all VignetteNodes in the 
        /// graph.
        /// </summary>
        /// <param name="head">The node to write to disk.</param>
        /// <param name="path">The path in the project to save to.</param>
        /// <param name="saveAssets">If true, the asset database will be saved
        /// after the vignette is created.</param>
        /// <param name="overrideName">If true, override the name of the
        /// serialized asset with the vignette head's name.</param>
        public static void SerializeVignette(VignetteGraph head,
                                             string path,
                                             bool saveAssets=true,
                                             bool overrideName=false)
        {
            string savedName = null;
            if (overrideName)
            {
                savedName = head.name;
            }

            // convert path to use unity directory separator character
            path = path.Replace(@"\", "/");

            AssetDatabase.CreateAsset(head, path);
            head.CollectConnectedNodes();
            VignetteNode[] nodes = head.GetAllNodes();

            for (int i = 0; i < nodes.Length; i++)
            {
                AssetDatabase.AddObjectToAsset(nodes[i], path);
            }
            if (overrideName)
            {
                head.name = savedName;
            }

            EditorUtility.SetDirty(head);
            if (saveAssets)
            {
                // Save and update the editor.
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// For a given selection, this will return the first found instance of 
        /// an object of the given type.
        /// </summary>
        /// <typeparam name="T">The type of object to look for.</typeparam>
        /// <returns>
        /// The first object in the selection of type T, or null if no object 
        /// is found.
        /// </returns>
        public static T FindObjectInSelection<T>() where T:ScriptableObject
        {
            // If no assets are selected, attempt to build all of the assets
            if (Selection.GetFiltered(typeof(Object), SelectionMode.Assets).Length == 0)
                return null;

            Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);

            if (objs.Length == 0)
                return null;

            for (int index = 0; index < objs.Length; index++)
            {
                Object o = objs[index];
                // Object main = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(o));
                AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(o));
                T asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(o), typeof (T)) as T;
                if (asset != null)
                    return asset;
            }
            return null;
        }
    }
}
