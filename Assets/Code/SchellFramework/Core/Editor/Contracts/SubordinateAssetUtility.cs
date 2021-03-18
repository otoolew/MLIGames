// ------------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//  Contact: Ryan Hipple
//  Created: 10/20/2015
// ------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SG.Core.Contracts
{
    /// <summary>
    /// Provides editor functions to work with ISubordinateAsset classes for 
    /// copying and deletion using serialized property helper functions.
    /// </summary>
    public static class SubordinateAssetUtility
    {
        /// <summary>
        /// Gets all ISubordinateAsset objects that are referenced by the 
        /// serialized property.
        /// </summary>
        /// <param name="sp">SerializedProperty to search recursively.</param>
        /// <returns>
        /// A mapping of SerializedProperty paths to ISubordinateAsset objects.
        /// </returns>
        public static SerializedPropertyMap<ISubordinateAsset> GetSubordinateAssets(SerializedProperty sp)
        {
            return SerializedObjectUtility.FindProperties<ISubordinateAsset>(sp);
        }

        /// <summary>
        /// Gets a deep copy of all ISubordinateAsset objects that are 
        /// referenced by the serialized property.
        /// </summary>
        /// <param name="sp">SerializedProperty to search recursively.</param>
        /// <returns>
        /// A mapping of SerializedProperty paths to ISubordinateAsset objects.
        /// </returns>
        public static SerializedPropertyMap<ISubordinateAsset> GetSubordinateAssetCopies(SerializedProperty sp)
        {
            Dictionary<int, ISubordinateAsset> remappings = 
                new Dictionary<int, ISubordinateAsset>();
            SerializedPropertyMap<ISubordinateAsset> map = 
                GetSubordinateAssets(sp);

            for (int e = 0; e < map.GetCount(); e++)
            {
                int hash = map.GetEntry(e).Object.GetHashCode();
                if (remappings.ContainsKey(hash))
                {
                    map.GetEntry(e).Object = remappings[hash];
                }
                else
                {
                    ISubordinateAsset newAsset = map.GetEntry(e).Object.DeepCopy();
                    remappings.Add(hash, newAsset);
                    map.GetEntry(e).Object = newAsset;
                }                
            }
            return map;        
        }

        /// <summary>
        /// Passes in a mapping of SerializedProperty paths to 
        /// ISubordinateAsset objects that will be populated in the destination
        /// SerializedObject. For each map entry, the ISubordinateAsset object
        /// will be duplicated and saved to disk under the save object.
        /// </summary>
        /// <param name="map">
        /// Mapping of property paths to ISubordinateAssets.
        /// </param>
        /// <param name="dest">
        /// SerializedProperty that will get be set to reference the data in 
        /// the map.
        /// </param>
        /// <param name="saveObject">
        /// Asset on disk to save new ISubordinateAsset objects under.
        /// </param>
        public static void PopulateSubordinateAssets(SerializedPropertyMap<ISubordinateAsset> map, SerializedObject dest, ScriptableObject saveObject)
        {
            Dictionary<int, ISubordinateAsset> remappings = 
                new Dictionary<int, ISubordinateAsset>();
            SerializedPropertyMap<ISubordinateAsset> copiedMap = 
                new SerializedPropertyMap<ISubordinateAsset>(map);

            for (int e = 0; e < copiedMap.GetCount(); e++)
            {
                int hash = copiedMap.GetEntry(e).Object.GetHashCode();
                if (remappings.ContainsKey(hash))
                {
                    copiedMap.GetEntry(e).Object = remappings[hash];
                }
                else
                {
                    // Make a deep copy of each unique entry and save it to 
                    // disk and to the copied map.
                    ISubordinateAsset asset = copiedMap.GetEntry(e).Object.DeepCopy();
                    copiedMap.GetEntry(e).Object = asset;
                    remappings.Add(hash, asset);
                    AssetDatabase.AddObjectToAsset(asset as Object, saveObject);
                }                
            }
            SerializedObjectUtility.PopulateSerializedProperties<ISubordinateAsset>(copiedMap, dest);                
        }

        /// <summary>
        /// Checks if a serialized property has any references to a 
        /// ISubordinateAsset.
        /// </summary>
        /// <param name="sp">Property to search.</param>
        /// <param name="asset">Asset to search for.</param>
        /// <returns>
        /// True if any property paths under teh given SerializedProperty 
        /// reference the given asset.
        /// </returns>
        public static bool IsUsed(SerializedProperty sp, ISubordinateAsset asset)
        {
            // This reimplements some implementation of FindProperties in order
            // to cut out early when a single result is found.
            string initalPath = sp.propertyPath;
            do
            {
                if (sp.type.StartsWith(SerializedObjectUtility.OBJECT_REFERENCE_TYPE_PREFIX))
                {
                    ISubordinateAsset na = sp.objectReferenceValue as ISubordinateAsset;
                    if (na != null && na == asset)
                        return true;
                }
            }
            while (sp.Next(true) && sp.propertyPath.StartsWith(initalPath));
            return false;
        }
    }
}
