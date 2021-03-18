//------------------------------------------------------------------------------
// Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//
// Contact: Sam Polglase
// Created: January 2015
//------------------------------------------------------------------------------

using System.IO;
using SG.Core.IO;
using UnityEditor;
using UnityEngine;

namespace SG.Core
{
    /// <summary>
    /// A wizard to help duplicate multiple assets with similar naming structures.
    /// 
    /// Also note that the number of wildcards in the original search pattern and
    /// the new filename pattern need to match.
    /// 
    /// ex. Original Search Pattern: "*_punch_*.anim"
    ///     New Filename Pattern: "*_jab_*.anim"
    ///     
    /// If they don't match you will receive an error popup telling you to match your wildcards.
    /// </summary>
    public class DuplicationWizard : ScriptableWizard
    {
        /// <summary>
        /// If this is set to true the folder structure will be searched recursively when attempting to find
        /// matching assets.
        /// </summary>
        public bool SearchRecursively = true;

        /// <summary>
        /// This search pattern is used to find the source assets that should be duplicated.  You can include
        /// wildcards in the search pattern according to the standards of the Directory.GetFiles() 
        /// search pattern rules.
        /// </summary>
        public string OriginalSearchPattern;

        /// <summary>
        /// This filename pattern will determine what the new duplicated asset will be named.
        /// </summary>
        public string NewFilenamePattern;

        /// <summary>
        /// If set to true will do the search for assets based on the folder selected.  Otherwise the search
        /// will spider through the entire Unity project.
        /// </summary>
        public bool SelectionOnly = true;

        /// <summary>
        /// If set to true any files have the same name as the duplicate we will overwrite those files.
        /// </summary>
        public bool OverwriteExistingFiles;
        
        [MenuItem("Framework/Duplication Wizard...")]
        public static void CreateWizard()
        {
            DisplayWizard("Duplication Wizard", typeof(DuplicationWizard), "Close", "Duplicate");
        }

        public void OnWizardOtherButton()
        {
            DuplicateAssets();
        }

        public void OnWizardCreate()
        {
        }

        private void DuplicateAssets()
        {
            if (string.IsNullOrEmpty(OriginalSearchPattern))
            {
                EditorUtility.DisplayDialog("Problem", "Please include an original filename.", "Ok");
            }
            else if (string.IsNullOrEmpty(NewFilenamePattern))
            {
                EditorUtility.DisplayDialog("Problem", "Please include a new filename.", "Ok");
            }
            else
            {
                string originalPattern = AddExtensionWildcardIfMissing(OriginalSearchPattern);
                string newPattern = AddExtensionWildcardIfMissing(NewFilenamePattern);
                
                if (CountTargetChar(originalPattern, '*') != CountTargetChar(newPattern, '*'))
                {
                    EditorUtility.DisplayDialog("Problem", "Please make sure that the original search pattern and the new filename pattern have equal wildcard counts.", "Ok");
                }
                else
                {
                    string systemPath = GetFullSearchPath();

                    if (!string.IsNullOrEmpty(systemPath))
                    {
                        DuplicatePatternMatchersAtPath(originalPattern, newPattern, systemPath);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Problem", "Illegal system path please select a folder and try again.", "Ok");
                    }
                }
            }
        }

        private string GetFullSearchPath()
        {
            string fullpath = string.Empty;

            if (SelectionOnly)
            {
                if (Selection.activeObject != null)
                {
                    string assetPath = string.Empty;
                    try
                    {
                        // Private implementation of a filenaming function which puts the file at the selected path.
                        System.Type assetdatabase = typeof (AssetDatabase);
                        assetPath =
                            (string)
                                assetdatabase.GetMethod("GetUniquePathNameAtSelectedPath",
                                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                                    .Invoke(assetdatabase, new object[] {"test"});
                    }
                    catch
                    {
                    }

                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        fullpath = AssetDirectoryUtility.GetFullPath(Path.GetDirectoryName(assetPath));
                    }
                }
            }
            else
            {
                fullpath = Application.dataPath;
            }
            return fullpath;
        }

        private int CountTargetChar(string sourceStr, char searchChar)
        {
            int count = 0;
            for (int i = 0; i < sourceStr.Length; i++)
            {
                if (sourceStr[i] == searchChar)
                    count++;
            }

            return count;
        }

        private string AddExtensionWildcardIfMissing(string sourceStr)
        {
            string result = sourceStr;
            if (!result.Contains("."))
                result += ".*";

            return result;
        }

        private void DuplicatePatternMatchersAtPath(string originalPattern, string newPattern, string systemPath)
        {
            if (!string.IsNullOrEmpty(systemPath))
            {
                int count = 0;
                SearchOption so = (SelectionOnly && !SearchRecursively) ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories;
                string[] filePaths = SgFile.GetFilesWithExclusions(systemPath, originalPattern, new [] { "*.meta", "*.svn-base" }, so);

                Log.Trace("Found: {0} files at path: {1}", filePaths.Length, systemPath);
                for (int i = 0; i < filePaths.Length; i++)
                {
                    if (DuplicateAsset(originalPattern, newPattern, filePaths[i]))
                        count++;
                }

                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Success", string.Format("Duplicated {0} assets successfully.", count), "Ok");
            }
        }

        private bool DuplicateAsset(string originalPattern, string newPattern, string filePath)
        {
            Log.Trace("Found file: {0}", filePath);
            string localPath = AssetDirectoryUtility.GetProjectLocalPath(filePath);
            string localDirectory = Path.GetDirectoryName(localPath);
            string localFilename = Path.GetFileName(localPath);

            string newFilename = GenerateNewFilename(originalPattern, newPattern, localFilename);
            string newLocalPath = Path.Combine(localDirectory, newFilename);
            string newFullPath = AssetDirectoryUtility.GetFullPath(newLocalPath);
            bool doesAssetExist = File.Exists(newFullPath);

            Log.Trace("Does asset \"{0}\" already exist: {1}", newFullPath, doesAssetExist);

            if (OverwriteExistingFiles && doesAssetExist)
                AssetDatabase.DeleteAsset(newLocalPath);

            if (OverwriteExistingFiles || !doesAssetExist)
            {
                AssetDatabase.CopyAsset(localPath, newLocalPath);
                Log.Trace("Duplicating to: {0}", newLocalPath);
                return true;
            }

            Log.Trace("Skipping asset duplication to:" + newLocalPath);
            return false;
        }

        private static string GenerateNewFilename(string originalPattern, string newPattern, string localFilename)
        {
            string temp = localFilename;

            string[] originalChunks = originalPattern.Split(new [] { '*' }, System.StringSplitOptions.RemoveEmptyEntries);
            string[] newChunks = newPattern.Split(new [] { '*' }, System.StringSplitOptions.RemoveEmptyEntries);

            for (int j = 0; j < originalChunks.Length; j++)
                temp = temp.Replace(originalChunks[j], ",");

            string[] globalChunks = temp.Split(',');

            string newFilename = string.Empty;

            for (int j = 0; j < globalChunks.Length; j++)
            {
                newFilename += globalChunks[j];

                if (j < newChunks.Length)
                    newFilename += newChunks[j];
            }
            return newFilename;
        }

        private static readonly Notify Log = NotifyManager.GetInstance("Core");
    }
}
