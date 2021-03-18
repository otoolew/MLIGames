//-----------------------------------------------------------------
//  Copyright © 2010 Schell Games, LLC. All Rights Reserved. 
//
//  Author: William Roberts
//  Date:   04/22/2010
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;

namespace SG.Core.IO
{
    public delegate void CopyFileCallback(string src, string dest);

    /// <summary>
    /// Utility file I/O helpers for adding additional functionality.
    /// </summary>
    public static class SgFile
    {
        private const FileAttributes NotSpecial = 
            ~(FileAttributes.Archive | FileAttributes.ReadOnly | FileAttributes.Hidden);
   
        /// <summary>
        /// Recursively deletes the specified directory and it's contents. 
        /// This method is different than <see cref="System.IO.Directory.Delete(string)"/> 
        /// in that it will forcefully remove files marked as archive, read only,
        /// and hidden instead of raising an exception.
        /// </summary>
        /// <param name="path">Path to delete.</param>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown if the specified directory does not exist.
        /// </exception>
        public static void DeleteDirectory(string path)
        {
            DeleteDirectory(path, true);
        }

        /// <summary>
        /// Recursively deletes the specified directory and it's contents. 
        /// This method is different than <see cref="System.IO.Directory.Delete(string)"/> 
        /// in that it will forcefully remove files marked as archive, read only,
        /// and hidden instead of raising an exception.
        /// </summary>
        /// <param name="path">Path to delete.</param>
        /// <param name="throwDirectoryNotFoundException">
        /// If set to true an exception will be thrown if the folder to delete doesn't exist.  
        /// Otherwise it will silently fail.
        /// </param>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown if the specified directory does not exist and throwDirectoryNotFoundException is true.
        /// </exception>
        public static void DeleteDirectory(string path, bool throwDirectoryNotFoundException)
        {
            var root = new DirectoryInfo(path);
            if (!root.Exists)
            {
                if (!throwDirectoryNotFoundException)
                    return;

                throw new DirectoryNotFoundException("Directory does not exist! Aborting DeleteDirectory operation.");
            }

            var directories = new Stack<DirectoryInfo>();
            directories.Push(root);
            while (directories.Count > 0)
            {
                DirectoryInfo currDir = directories.Pop();
                currDir.Attributes = currDir.Attributes & NotSpecial;

                foreach (DirectoryInfo dir in currDir.GetDirectories())
                {
                    directories.Push(dir);
                }

                foreach (FileInfo fileInfo in currDir.GetFiles())
                {
                    fileInfo.Attributes = fileInfo.Attributes & NotSpecial;
                    fileInfo.Delete();
                }
            }

            root.Delete(true);
        }

        /// <summary>
        /// Searches for and deletes all <b>empty</b> directories recursively 
        /// within the given directory path. Non-empty directories are skipped.
        /// </summary>
        /// <param name="path">Directory to search.</param>
        /// <exception cref="DirectoryNotFoundException">
        /// Path does not exist.
        /// </exception>
        public static void CleanEmptyDirectories(string path)
        {
            var root = new DirectoryInfo(path);
            if (!root.Exists)
                throw new DirectoryNotFoundException("Directory does not exist! Aborting CleanEmptyDirectories operation.");

            var directories = new Stack<DirectoryInfo>();
            var depthStack = new Stack<DirectoryInfo>();
            DirectoryInfo currDir;
            directories.Push(root);
            while (directories.Count > 0)
            {
                currDir = directories.Pop();
                depthStack.Push(currDir);

                foreach (DirectoryInfo dir in currDir.GetDirectories())
                {
                    directories.Push(dir);
                }
            }

            while (depthStack.Count > 0)
            {
                currDir = depthStack.Pop();

                if (currDir.GetFileSystemInfos().Length == 0)
                {
                    currDir.Attributes = currDir.Attributes & NotSpecial;
                    currDir.Delete();
                }
            }
        }

        /// <summary>
        /// Forcefully removes the specified file from the file system.
        /// </summary>
        /// <param name="path">Path of the file to remove.</param>
        /// <returns>True if the file was removed false otherwise.</returns>
        public static bool ForceDeleteFile(string path)
        {
            var info = new FileInfo(path);
            if (info.Exists)
            {
                info.Attributes = info.Attributes & NotSpecial;
                info.Delete();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Recursively copies the contents of the source directory to the destination directory.
        /// The destination will automatically be created if it does not already exist.
        /// Copied files that are read only will be modified to be non read only.
        /// </summary>
        /// <param name="srcPath">Source Directory</param>
        /// <param name="destPath">Destination Directory</param>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown if the source directory does not exist.
        /// </exception>
        public static void CopyAll(string srcPath, string destPath)
        {
            CopyAll(srcPath, destPath, null);
        }

        /// <summary>
        /// Recursively copies the contents of the source directory to the destination directory.
        /// The destination will automatically be created if it does not already exist.
        /// Copied files that are read only will be modified to be non read only.
        /// </summary>
        /// <param name="srcPath">Source Directory</param>
        /// <param name="destPath">Destination Directory</param>
        /// <param name="exclusionPatterns">
        /// A list of Glob patterns representing files & directories to exclude from the copy
        /// operation. 
        /// 
        /// ex: new string[] {"*\.svn" } 
        /// This example demonstrates the exclusion of all subversion directories during the copy
        /// process. See <see cref="Glob"/> for more info on available pattern matching.
        /// </param>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown if the source directory does not exist.
        /// </exception>
        public static void CopyAll(string srcPath, string destPath, string[] exclusionPatterns)
        {
            CopyAll(srcPath, destPath, exclusionPatterns, null);
        }

        /// <summary>
        /// Recursively copies the contents of the source directory to the destination directory.
        /// The destination will automatically be created if it does not already exist.
        /// Copied files that are read only will be modified to be non read only.
        /// </summary>
        /// <param name="srcPath">Source Directory</param>
        /// <param name="destPath">Destination Directory</param>
        /// <param name="exclusionPatterns">
        /// A list of Glob patterns representing files & directories to exclude from the copy
        /// operation. 
        /// 
        /// ex: new string[] {"*\.svn" } 
        /// This example demonstrates the exclusion of all subversion directories during the copy
        /// process. See <see cref="Glob"/> for more info on available pattern matching.
        /// </param>
        /// <param name="copyCallBack">Callback that will fire after every file is copied.</param>
        /// <exception cref="System.IO.DirectoryNotFoundException">Source directory does not exist! Aborting CopyAll operation.</exception>
        public static void CopyAll(string srcPath, string destPath, string[] exclusionPatterns, CopyFileCallback copyCallBack)
        {
            var srcDirs = new Stack<DirectoryInfo>();
            var destDirs = new Stack<DirectoryInfo>();
            var srcInfo = new DirectoryInfo(srcPath);
            var destInfo = new DirectoryInfo(destPath);
            
            if (!srcInfo.Exists)
                throw new DirectoryNotFoundException("Source directory does not exist! Aborting CopyAll operation.");

            Glob[] exclusionList = null;
            if (exclusionPatterns != null)
            {
                exclusionList = new Glob[exclusionPatterns.Length];
                for (int i = 0; i < exclusionPatterns.Length; i++)
                    exclusionList[i] = new Glob(exclusionPatterns[i]);
            }

            srcDirs.Push(srcInfo);
            destDirs.Push(destInfo);
            while (srcDirs.Count > 0)
            {
                DirectoryInfo currDir = srcDirs.Pop();
                destInfo = destDirs.Pop();

                foreach (DirectoryInfo dir in currDir.GetDirectories())
                {
                    if (!ShouldExclude(exclusionList, dir.FullName))
                    {
                        srcDirs.Push(dir);
                        destDirs.Push(new DirectoryInfo(Path.Combine(destInfo.FullName, dir.Name)));
                    }
                }

                if (!destInfo.Exists)
                    destInfo.Create();

                foreach (FileInfo fileInfo in currDir.GetFiles())
                {
                    if (!ShouldExclude(exclusionList, fileInfo.FullName))
                    {
                        string filePath = Path.Combine(destInfo.FullName, fileInfo.Name);

                        fileInfo.Attributes = fileInfo.Attributes & ~FileAttributes.ReadOnly;
                        fileInfo.CopyTo(filePath, true);

                        if (copyCallBack != null)
                            copyCallBack(fileInfo.FullName, filePath);
                    }
                }
            }
        }

        /// <summary>
        /// An enhanced version of the standard Directory.GetFiles(...) static method that allows
        /// for multiple search patterns. Each search pattern must be separated by a semicolon.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="searchPatterns">
        /// Search patterns to use. ie: "*.txt;*.config;*xml" would return
        /// any file that is an text, configuration, or XML file.
        /// </param>
        /// <param name="searchOption">Determines the depth of the search.</param>
        /// <returns>An array of file paths that match the search.</returns>
        public static string[] GetFiles(string path, string[] searchPatterns, SearchOption searchOption)
        {
            if (searchPatterns == null || searchPatterns.Length == 0)
            {
                throw new ArgumentException("At least one search pattern must be included", "searchPatterns");
            }

            List<string> retVal = new List<string>();
            // TODO: Optimize this
            foreach (string wc in searchPatterns)
                retVal.AddRange(Directory.GetFiles(path, wc, searchOption));

            return retVal.ToArray();
        }

        /// <summary>
        /// Returns a listing of all files recursively found at the specified path excluding
        /// the files containing the specified file extensions.
        /// </summary>
        /// <param name="path">Path to search.</param>
        /// <param name="exclusionPatterns">
        /// A list of Glob patterns representing files & directories to exclude from the results.
        /// 
        /// ex: new string[] {"*.png" } 
        /// This example demonstrates the exclusion of all png files from the results.
        /// </param>
        /// <returns>A listing of files.</returns>
        public static string[] GetFilesWithExclusions(string path, string[] exclusionPatterns)
        {
            return GetFilesWithExclusions(path, exclusionPatterns, SearchOption.AllDirectories);
        }


        /// <summary>
        /// Returns a listing of all files found at the specified path excluding
        /// the files containing the specified file extensions.
        /// </summary>
        /// <param name="path">Path to search.</param>
        /// <param name="exclusionPatterns">
        /// A list of Glob patterns representing files & directories to exclude from the results.
        /// 
        /// ex: new string[] {"*.png" } 
        /// This example demonstrates the exclusion of all png files from the results.
        /// </param>
        /// <param name="option">Specifies whether to search the current directory,
        /// or the current directory and all subdirectories.</param>
        /// <returns>A listing of files.</returns>
        public static string[] GetFilesWithExclusions(string path, string[] exclusionPatterns, SearchOption option)
        {
            return GetFilesWithExclusions(path, "*.*", exclusionPatterns, option);
        }

        /// <summary>
        /// Returns a listing of all files found at the specified path excluding
        /// the files containing the specified file extensions.
        /// </summary>
        /// <param name="path">Path to search.</param>
        /// /// <param name="searchPattern">
        /// Search pattern to use. ie: "*.txt;*.config;*xml" would return
        /// any file that is an text, configuration, or XML file.
        /// </param>
        /// <param name="exclusionPatterns">
        /// A list of Glob patterns representing files & directories to exclude from the results.
        /// 
        /// ex: new string[] {"*.png" } 
        /// This example demonstrates the exclusion of all png files from the results.
        /// </param>
        /// <returns>A listing of files.</returns>
        public static string[] GetFilesWithExclusions(string path, string searchPattern, string[] exclusionPatterns, SearchOption option)
        {
            List<string> filtered = new List<string>();
            Glob[] exclusionList = null;

            if (exclusionPatterns != null)
            {
                exclusionList = new Glob[exclusionPatterns.Length];
                for (int i = 0; i < exclusionPatterns.Length; i++)
                    exclusionList[i] = new Glob(exclusionPatterns[i]);
            }

            string[] files = Directory.GetFiles(path, searchPattern, option);

            for (int i = 0; i < files.Length; i++)
            {
                if(!ShouldExclude(exclusionList, files[i]))
                    filtered.Add(files[i]);
            }

            return filtered.ToArray();
        }

        /// <summary>
        /// Helper function to determine if the test string should be excluded
        /// from the resulting operation.
        /// </summary>
        /// <param name="exclusionList">
        /// List of glob patterns, or null if none exist.
        /// </param>
        /// <param name="testString">String to test.</param>
        /// <returns>
        /// True if a match was found for the test string.
        /// </returns>
        private static bool ShouldExclude(Glob[] exclusionList, string testString)
        {
            if (exclusionList == null) // Nothing to test against, just exit.
                return false;

            for (int i = 0; i < exclusionList.Length; i++)
            {
                if (exclusionList[i].IsMatch(testString))
                    return true;
            }

            return false;
        }
    }
}