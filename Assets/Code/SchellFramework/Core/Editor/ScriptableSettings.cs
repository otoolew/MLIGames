//------------------------------------------------------------------------------
// Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
// Contact: William Roberts
//
// Created: 01/13/2016
//------------------------------------------------------------------------------

using UnityEditorInternal;
using UnityEngine;

namespace SG.Core
{
    /// <summary>
    /// Represents a settings objects that can be saved outside of the Assets directory within the project.
    /// Note that each ScriptableSettings object must have an FilePathAttribute attached that defines
    /// the location of the settings file within the project. Typically, the path will be "ProjectSettings/{Your Class Name}.asset".
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// 
    /// // Example of the settings object.
    /// [FilePath("ProjectSettings/FooManagerSettings.asset")]
    /// class FooManagerSettings : ScriptableSettings<FooManagerSettings>
    /// {
    ///     public bool enableFoo = true;
    ///     public Vector2 fooPosition = Vector2.Zero;
    /// }
    ///
    /// 
    /// // Window to display the settings within.
    /// class FooManagerSettingsWindow : EditorWindow
    /// {
    ///     private SerializedObject _serializedObject = null;
    ///         
    ///     protected void OnEnable()
    ///     {
    ///         _serializedObject = new SerializedObject(FooManagerSettings.Instance);
    ///     }
    ///         
    ///     protected void OnDisable()
    ///     {
    ///        if (_serializedObject != null)
    ///        {
    ///            _serializedObject.Dispose();
    ///                _serializedObject = null;
    ///        }
    ///      }
    ///         
    ///      protected void OnGUI()
    ///      {
    ///         _serializedObject.Update();
    ///         
    ///         // Part of SG.Core and is responsible for rendering the speciifed Serialized Object
    ///         EditorOnGUILayout.DrawPropertiesExcluding(_serializedObject, "m_Script");
    ///         
    ///         // Make sure to save all changes to disk if they happen!
    ///         if (_serializedObject.ApplyModifiedProperties())
    ///             FooManagerSettings.Instance.Save();
    ///       }
    /// }
    /// ]]>
    /// </example>
    /// <typeparam name="T"></typeparam>
    public class ScriptableSettings<T> : ScriptableObject where T : ScriptableSettings<T>
    {
        /// <summary>
        /// The initial version number.
        /// </summary>
        protected const int DEFAULT_VERSION = 1;

        [HideInInspector]
        private int _version = 0;


        private static T _instance;


        /// <summary>
        /// Returns the version this asset was saved as. This value can be compared to the "LatestVersion" property
        /// to determine if the asset must be upgraded due to any functionality changes.
        /// </summary>
        public int Version
        {
            get { return _version; }
            set { _version = value; }
        }


        /// <summary>
        /// Returns the current most "format" version of this scriptable object. This value is utilized when
        /// creating a new instance of the object. You can override this proprty in order to increment the 
        /// value if necassary.
        /// </summary>
        protected virtual int LatestVersion
        {
            get { return DEFAULT_VERSION; }
        }


        /// <summary>
        /// Retrieves an singleton instance of this settings object. The object will be loaded from disk if it exists.
        /// Otherwise, a new in instance will be created. It is your responsibility to call the "Save" method any time 
        /// the data is updated and needs to be persisted to disk. Note that a cached value will be returned after the
        /// initial invocation of this property.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    LoadOrCreateInstance();

                return _instance;
            }
        }


        /// <summary>
        /// Flushes an changes to the disk drive.
        /// </summary>
        public void Save()
        {
            InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] { this }, GetFilePath(), true);
        }


        /// <summary>
        /// Attempts to retrieve the 'FilePathAttribute' attached to this class so that we can determine where
        /// to save this scriptable object.
        /// </summary>
        /// <returns></returns>
        private static string GetFilePath()
        {
            System.Type type = typeof(T);
            FilePathAttribute filePath = System.Attribute.GetCustomAttribute(type, typeof(FilePathAttribute)) as FilePathAttribute;

            if (filePath == null)
            {
                throw new System.Exception(
                    string.Format(
                        "An FilePathAttribute must be attached to the {0} class.",
                        type.FullName 
                    )
                );
            }

            return filePath.Path;
        }


        /// <summary>
        /// Attempts to load the scriptable object from disk. If it cannot be loaded, a new
        /// instance will be created. Note that new instance will not be saved.
        /// </summary>
        private static void LoadOrCreateInstance()
        {
            Object[] contents = InternalEditorUtility.LoadSerializedFileAndForget(GetFilePath());

            if (contents != null)
            {
                for (int i = 0; i < contents.Length; i++)
                {
                    var instance = contents[i] as T;

                    if (instance != null)
                        _instance = instance;
                }
            }

            if (_instance == null)
            {
                _instance = CreateInstance<T>();
                _instance.InitialSetup();
            }

            // This is needed to ensure a bad reference is not hung onto when the play button is toggled.
            // This occurs because until attmepts to serialize the asset and it does not exist in the project.
            _instance.hideFlags = HideFlags.DontSave;
        }


        private void InitialSetup()
        {
            _version = LatestVersion;
        }
    }
}
