// -----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Max Golden
//
//  Created: 12/7/2016 11:00 AM
// -----------------------------------------------------------------------------

using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using SG.Core;
using SG.Core.Templating;
using SG.Vignettitor.VignetteData;

namespace SG.Vignettitor.NodeMaker
{
    public class VignetteNodeModel
    {
        protected readonly Notify Log = NotifyManager.GetInstance<VignetteNodeModel>();

        public string classNamespace;
        public string creatorName;
        public string summary;
        public string logMessage;
        public string nodeName;
        public Type graphType;
        public OutputRule.RuleType ruleType;
        public int numChildren;
        public Color color;

        public VignetteNodeModel(string classNamespace, string creatorName, string nodeName, string summary,
                Type graphType, OutputRule.RuleType ruleType, int numChildren, Color color, string logMessage)
        {
            this.classNamespace = classNamespace;
            this.creatorName = creatorName;
            this.nodeName = nodeName;
            this.summary = summary;
            this.graphType = graphType;
            this.ruleType = ruleType;
            this.numChildren = numChildren;
            this.color = color;
            this.logMessage = logMessage;
        }

        // Child classes should override this to specify different templates
        protected virtual string TemplateFilename { get { return "VignetteNodeTemplate.txt"; } }

        /// <summary>
        /// Opens the template file specified by TemplateFilename and returns the entire text contents of the file
        /// </summary>
        public string GetTemplateText()
        {
            string[] templatePaths = Directory.GetFiles("Assets/", TemplateFilename, SearchOption.AllDirectories);
            if (templatePaths.Length == 1)
            {
                return File.ReadAllText(templatePaths[0]);
            }
            else if (templatePaths.Length == 0)
            {
                string message = string.Format("Could not find any files named '{0}'", TemplateFilename);
                throw new FileNotFoundException(message);
            }
            else  // Multiple files found with the same name
            {
                string message = string.Format("Template ambiguity: Found {0} files named '{1}'", templatePaths.Length, TemplateFilename);
                throw new IOException(message);
            }
        }

        /// <summary>
        /// Returns the name of the new file to be generated
        /// </summary>
        public string GetFileName()
        {
            // The node inherits from scriptable object, so the file should have its name
            return GetNodeClassName() + ".cs";
        }

        #region ------- Template replacement functions -------

        [TemplateKey("#(c)#")]
        public string copyright = "\u00A9";

        [TemplateKey(NodeTemplateStrings.CREATION_TIME)]
        public string GetCreationTimeString()
        {
            return DateTime.Now.ToString("G");  // "G" -> MM/DD/YYYY HH:MM:SS
        }

        [TemplateKey(NodeTemplateStrings.CREATOR_NAME)]
        public string GetCreatorName()
        {
            return creatorName;
        }

        [TemplateKey(NodeTemplateStrings.LOG_MESSAGE)]
        public string GetLogMessage()
        {
            return logMessage;
        }

        [TemplateKey(NodeTemplateStrings.NAMESPACE)]
        public string GetNamespace()
        {
            return classNamespace;
        }

        [TemplateKey(NodeTemplateStrings.NODE_GRAPH)]
        public Type GetGraphType()
        {
            return graphType;
        }

        [TemplateKey(NodeTemplateStrings.NODE_CLASS_NAME)]
        public string GetNodeClassName()
        {
            return GetCamelCaseName(nodeName) + "Node";
        }

        [TemplateKey(NodeTemplateStrings.VIEW_CLASS_NAME)]
        public string GetViewClassName()
        {
            return GetCamelCaseName(nodeName) + "View";
        }

        [TemplateKey(NodeTemplateStrings.RUNTIME_CLASS_NAME)]
        public string GetRuntimeClassName()
        {
            return GetCamelCaseName(nodeName) + "Runtime";
        }

        [TemplateKey(NodeTemplateStrings.NODE_MENU_PATH)]
        public string GetNodeMenuPath()
        {
            return GetSpacedName(nodeName) + " Node";
        }

        [TemplateKey(NodeTemplateStrings.OUTPUT_RULE)]
        public string GetOutputRuleString()
        {
            switch (ruleType)
            {
                case OutputRule.RuleType.Passthrough:
                    return "OutputRule.Passthrough()";
                case OutputRule.RuleType.Static:
                    return string.Format("OutputRule.Static({0})", numChildren);
                case OutputRule.RuleType.Variable:
                    return "OutputRule.Variable()";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [TemplateKey(NodeTemplateStrings.SUMMARY)]
        public string GetSummary()
        {
            return summary.Replace("\n", " ");
        }

        [TemplateKey(NodeTemplateStrings.VIEW_COLOR)]
        public string GetColorString()
        {
            return string.Format("new Color({0}f, {1}f, {2}f, {3}f)",
                color.r.ToString("0.00"),
                color.g.ToString("0.00"),
                color.b.ToString("0.00"),
                color.a.ToString("0.00"));
        }

        [TemplateKey(NodeTemplateStrings.YEAR)]
        public int GetYear()
        {
            return DateTime.Now.Year;
        }

        #endregion

        #region ------- String Helper Functions -------

        /// <summary>
        /// Return the base name (without 'node') in CamelCase
        /// </summary>
        protected static string GetCamelCaseName(string str)
        {
            return string.Join(string.Empty, GetCaptialWordArray(TrimNodeName(str)));
        }

        /// <summary>
        /// Return the base name (without 'node') With Spaces In Between Words
        /// </summary>
        protected static string GetSpacedName(string str)
        {
            return string.Join(" ", GetCaptialWordArray(TrimNodeName(str)));
        }

        /// <summary>
        /// Trim white space and remove trailing "node", if it exists
        /// </summary>
        protected static string TrimNodeName(string str)
        {
            string trimmedName = str.Trim();

            // Remove trailing "node", if it exists
            if (trimmedName.ToLower().EndsWith("node"))
                trimmedName = trimmedName.Substring(0, trimmedName.Length - 4).TrimEnd();

            return trimmedName;
        }

        /// <summary>
        /// Split a string along camelcase OR spaces and return the array of the words in the string
        /// with their first letter capitalised
        /// </summary>
        protected static string[] GetCaptialWordArray(string stringToSplit)
        {
            if (string.IsNullOrEmpty(stringToSplit))
                return new string[1] { string.Empty };

            // Attempt to split on spaces
            string[] wordArray = stringToSplit.Split(null);

            if (wordArray.Length == 1)  // No spaces
            {
                // If we only have one word, we may be in camel case; this regex adds spaces before single-caps words
                stringToSplit = Regex.Replace(stringToSplit, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");
                // Then try to split again
                wordArray = stringToSplit.Split(null);
            }

            for (int i = 0; i < wordArray.Length; i++)
            {
                wordArray[i] = CapitalizeFirstLetter(wordArray[i]);
            }

            return wordArray;
        }

        /// <summary>
        /// Return the string str with only the first letter capitalised
        /// </summary>
        protected static string CapitalizeFirstLetter(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            if (str.Length == 1)
                return str.ToUpper();

            return char.ToUpper(str[0]) + str.Substring(1).ToLower();
        }

        #endregion
    }
}
