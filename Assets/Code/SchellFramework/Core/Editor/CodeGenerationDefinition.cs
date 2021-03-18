//------------------------------------------------------------------------------
// Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//
// Contact: Tim Sweeney
//
// Created: Month 2015
//------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace SG.Core
{
    [Serializable]
    public class CodeGenerationDefinition
    {
        [SerializeField]
        protected string ContactName = "Contact Name";

        [SerializeField]
        protected string TypeName = "TypeName";

        [SerializeField]
        protected string Directory = "Assets/Code/";

        [SerializeField]
        protected string Namespace = "SG.ProjectSpecific";

        [SerializeField]
        protected string CreatedTime;

        [SerializeField]
        protected MonoScript GeneratedScript;

        // string.Format Parameter semantics
        // 0 : user-supplied source string to the Code Gen as to what build this file
        // 1 : full class name of what built this file
        // 2 : user-supplied contact name
        // 3 : datetime of first creation
        // 4 : datetime of last generate
        // 5 : contents
        protected const string CODE_GEN_TEMPLATE =
@"// ------------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  AUTO-GENERATED
//  
//  Source: {0}
//  Generator: {1}
//  Contact: {2}
//
//  Created: {3}
//  Last Generated: {4}
// ------------------------------------------------------------------------------

{5}
";
        // Gets the qualified type name for this type
        // in the namespace of other code.
        public string TypeNameReference(CodeGenerationDefinition otherCodeGenerator)
        {
            string fullyQualified = string.Concat(Namespace, '.', TypeName);
            string searchPattern = string.Format(@"^{0}\.", otherCodeGenerator.Namespace);
            return Regex.Replace(fullyQualified, searchPattern, string.Empty);
        }

        /// <summary>
        /// Generate a code file with the supplied contents.
        /// </summary>
        /// <param name="source">Source reference for the standard header.</param>
        /// <param name="contents">Contents of the script (everything after the header).</param>
        protected void GenerateCode(string source, string contents)
        {
            // Clean supplied strings
            Namespace = CleanForNamespace(Namespace);
            TypeName = CleanForIdentifier(TypeName);

            // Determine output path via MonoScript reference or via string concatenation.
            string filepath;
            if (GeneratedScript)
            {
                filepath = AssetDatabase.GetAssetPath(GeneratedScript);
                Directory = string.Concat(Path.GetDirectoryName(filepath), '/');

            }
            else
            {
                filepath = string.Concat(Directory, TypeName, ".cs");
                // This should work if this is the 2nd+ time generating the same script and will
                // make finding the output path later easier.
                GeneratedScript = AssetDatabase.LoadAssetAtPath<MonoScript>(filepath);
            }

            // Generate datetime strings
            string now = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            CreatedTime = File.Exists(filepath) ? new FileInfo(filepath).CreationTime.ToString(CultureInfo.InvariantCulture) : now;

            string codeGen = string.Format(CODE_GEN_TEMPLATE, source, GetType().FullName,
                ContactName, CreatedTime, now, contents);

            if (File.Exists(filepath))
            {
                // Check for read-only flag
                // TODO: replace with a proper method for checking out the file
                FileAttributes attributes = File.GetAttributes(filepath);
                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    //bool forceWrite = EditorUtility.DisplayDialog("Generate Code",
                    //    string.Format(
                    //        "Generated code destination {0} is not read-only (probably not checked out). Force generation by removing read-only flag?",
                    //        filepath), "Force Generation", "Cancel");
                    //if (!forceWrite)
                    //    return;
                    File.SetAttributes(filepath, attributes & ~FileAttributes.ReadOnly);
                }
            }

            // If directories do not exist, create them.
            EditorAssetDirectoryUtility.CreateDirectories(Path.GetDirectoryName(filepath));

            File.WriteAllText(filepath, codeGen);
            AssetDatabase.ImportAsset(filepath);
        }

        // Namespaces cannot start with a digit or '.' or contain non Word characters apart from the '.' separator.
        protected static readonly Regex CleanForNamespaces = new Regex(@"^(?=\d)|(^\.)|[\W-[\.]]", RegexOptions.Compiled);

        // Identifiers cannot start with a digit or contain non Word characters.
        protected static readonly Regex CleanForIdentifiers = new Regex(@"^(?=\d)|\W", RegexOptions.Compiled);

        protected static string CleanForNamespace(string raw)
        {
            return string.IsNullOrEmpty(raw) ? "Namespace" : CleanForNamespaces.Replace(raw, "_");
        }

        protected static string CleanForIdentifier(string raw)
        {
            return string.IsNullOrEmpty(raw) ? "_identifier" : CleanForIdentifiers.Replace(raw, "_");
        }
    }
}
