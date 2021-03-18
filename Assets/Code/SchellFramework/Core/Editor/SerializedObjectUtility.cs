// ------------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//  Contact: Ryan Hipple
//  Created: 10/21/2015
// ------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SG.Core
{
    /// <summary>
    /// A mapping of SerializedProperty paths to an asset of type T that is 
    /// referenced at that path.
    /// </summary>
    /// <typeparam name="T">The type of asset in the mapping.</typeparam>
    public class SerializedPropertyMap<T>
    {
        /// <summary>
        /// A SerializedPropertyMap entry, linking a SerializedProperty path 
        /// to an object.
        /// </summary>
        public class Entry
        {
            /// <summary> SerializedProperty Path. </summary>
            public string Path;

            /// <summary> Object at the Path. </summary>
            public T Object;

            /// <summary> Create a new Entry. </summary>
            /// <param name="path">SerializedProperty Path.</param>
            /// <param name="obj">Object at the Path.</param>
            public Entry(string path, T obj)
            {
                Path = path;
                Object = obj;
            }

            /// <summary>
            /// Copy constructor. This will not make a deep copy of Object.
            /// </summary>
            /// <param name="other">Entry to copy from.</param>
            public Entry(Entry other)
            {
                Path = other.Path;
                Object = other.Object;
            }
        }

        /// <summary> The list of entries in the map. </summary>
        private readonly List<Entry> entries = new List<Entry>();

        public SerializedPropertyMap()
        { }

        /// <summary>
        /// Copy constructor. This will make a full copy of the entry list, 
        /// but the objects in the list will not be a deep copy.
        /// </summary>
        /// <param name="other">SerializedPropertyMap to copy.</param>
        public SerializedPropertyMap(SerializedPropertyMap<T> other)
        {
            entries = new List<Entry>();
            for (int i = 0; i < other.entries.Count; i++)
                entries.Add(new Entry(other.entries[i]));
        }

        /// <summary> Add a path to object entry to the map. </summary>
        /// <param name="path">SerializedProperty path.</param>
        /// <param name="o">Object at the path.</param>
        public void Add(string path, T o)
        {
            entries.Add(new Entry(path, o));
        }

        /// <summary> Gets how many entries are in the map. </summary>
        /// <returns></returns>
        public int GetCount()
        { return entries.Count; }

        /// <summary> Get the entry at the given index. </summary>
        /// <param name="i">Index of teh map entry.</param>
        /// <returns>Map entry.</returns>
        public Entry GetEntry(int i)
        { return entries[i]; }
    }

    /// <summary>
    /// Provides search and populate functionality for SerializedObjects and 
    /// SerializedProperties in the editor.
    /// </summary>
    public static class SerializedObjectUtility
    {        
        /// <summary>
        /// When serialized properties are referencing objects, the type string
        /// used for the property is is formated as follows to indicate it is 
        /// a pointer to the object.
        /// </summary>
        public const string OBJECT_REFERENCE_TYPE_PREFIX = "PPtr<$";

        /// <summary>
        /// Writes a set of object data into corresponding named properties of 
        /// a given SerializedObject.
        /// </summary>
        /// <typeparam name="T">
        /// The type of data in the SerializedPropertyMap. This may be 
        /// specified as a class or an interface.
        /// </typeparam>
        /// <param name="map">
        /// A mapping indicating property names to look for and the data to 
        /// write into the property's objectReferenceValue field.
        /// </param>
        /// <param name="dest">
        /// The SerializedObject to search for property names and to add the 
        /// data references to.
        /// </param>
        public static void PopulateSerializedProperties<T>(SerializedPropertyMap<T> map, SerializedObject dest) where T : class
        {
            for (int i = 0; i < map.GetCount(); i++)
            {
                SerializedPropertyMap<T>.Entry entry = map.GetEntry(i);

                SerializedProperty dsp = dest.FindProperty(entry.Path);
                if (dsp != null)
                {
                    Object obj = entry.Object as Object;

                    dsp.objectReferenceValue = obj;
                    Undo.RegisterCreatedObjectUndo(obj, "Field Set");
                    dest.ApplyModifiedProperties();
                }
                else
                    Log.Warning("Failed to copy to field " + entry.Path);
            }
        }

        /// <summary>
        /// Finds all SerializedProperty under the given SerializedProperty 
        /// that reference an Object of the given type. Only things inheriting
        /// from UnityEngine.Object may be returned since this checks the
        /// objectReferenceValue field of the serialized property.
        /// </summary>
        /// <typeparam name="T">
        /// The type to search for. This may be an interface or a class, only 
        /// data that inherits from UnityEngine.Object will be valid.
        /// </typeparam>
        /// <param name="sp">
        /// The serialized property to begin the search from. This is typically
        /// made as an iterator from a SerializedObject.
        /// </param>
        /// <returns>
        /// A map of property paths to the data serialized at those paths of 
        /// the specified type.
        /// </returns>
        public static SerializedPropertyMap<T> FindProperties<T>(SerializedProperty sp) where T : class
        {
            SerializedPropertyMap<T> result = new SerializedPropertyMap<T>();
            string initalPath = sp.propertyPath;
            do
            {
                if (sp.type.StartsWith(OBJECT_REFERENCE_TYPE_PREFIX))
                {
                    if (sp.objectReferenceValue is T)
                        result.Add(sp.propertyPath, sp.objectReferenceValue as T);
                }
            }
            while (sp.Next(true) && sp.propertyPath.StartsWith(initalPath));
            return result;
        }

        private static readonly Notify Log = NotifyManager.GetInstance("Core");
    }
}
