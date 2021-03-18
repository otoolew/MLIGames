// -----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Max Golden
//
//  Created: 12/8/2016 11:13 AM
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using SG.Core;
using SG.Core.Templating;
using SG.Vignettitor.VignetteData;

namespace SG.Vignettitor.NodeMaker
{
    /// <summary>
    /// Class that defines how the NodeMakerWindow displays draws fields and creates ndoes
    /// </summary>
    public class NodeMakerView
    {
        protected readonly Notify Log = NotifyManager.GetInstance<NodeMakerView>();

        #region ------- Defaults and Constants -------
        // An extending class should preserve parent template type ints
        // Also remember to override nodeTemplateTypeNames.get (to draw the dropdown properly)
        public enum NodeTemplateType : int
        {
            VignetteNode = 0
        }
        // Enums aren't extensible, so we need to manually populate a list of strings corresponding to the node names
        protected virtual string[] nodeTemplateTypeNames { get { return new string[] { "Vignette Node" }; } }  // Override this when adding new node types

        // Persistent field keys
        // PlayerPrefs are saved on a per-project basis, EditorPrefs persist across projects
        protected const string SAVE_DIRECTORY_KEY = "NodeMaker_SaveDirectory";  // PlayerPrefs
        protected const string NAMESPACE_KEY = "NodeMaker_LastNamespace";  // PlayerPrefs
        protected const string CREATOR_NAME_KEY = "NodeMaker_LastCreator";  // EditorPrefs

        // Default strings
        protected virtual string DefaultSaveDirectory { get { return "Assets/Code/Vignette/Nodes/"; } }
        protected virtual string DefaultNamespace { get { return string.Format("SG.{0}", Application.productName); } }

        protected Color DefaultNodeColor { get { return Color.gray; } }
        #endregion -----------------------------------

        #region ------- Editor Window Variables -------
        public virtual int DefaultLabelWidth { get { return 95; } }  // labelWidth set to this at the beginning of OnGUI
        protected string validationErrors;  // Tooltip text displayed over the disabled Create button if validation fails
        protected virtual Type GraphType { get { return typeof(VignetteGraph); } }
        // Disabled = greys out the field
        protected virtual bool SaveDirectoryIsDisabled { get { return false; } }
        protected virtual bool NamespaceIsDisabled { get { return false; } }
        // Not Shown = not drawn at all
        protected virtual bool ShowOutputRuleField { get { return true; } }
        protected virtual bool ShowNumChildrenField { get { return outputRuleType == OutputRule.RuleType.Static; } }

        // NodeType-related dictionaries (indexed on int to allow for extension by subclasses
        protected Dictionary<int, Color> defaultNodeColors;

        // Window events
        public event Action CloseWindow;

        /// <summary>
        /// Invocation method for the CloseWindow event
        /// </summary>
        protected void Close()
        {
            if (CloseWindow != null)
                CloseWindow.Invoke();
            else
                Log.Error("No CloseWindow listeners attached to the NodeMakerView!");
        }
        #endregion ------------------------------------

        #region ------- Node Field Variables -------
        protected string classNamespace;
        protected string creatorName;
        protected string nodeName;
        protected string summary;
        protected string logMessage;
        protected string saveDirectory;
        protected Color nodeColor;
        protected int currentTemplateType;  // NodeTemplateType casted to int, generally
        protected OutputRule.RuleType outputRuleType;
        protected int numChildren;
        #endregion -----------------------------

        #region ------- Initialization Methods -------
        public virtual void Initialize()
        {
            SetDefaultNodeColors();
            Reset();
        }

        /// <summary>
        /// Populate the defaultNodeColors dictionary with all of the default colors
        /// </summary>
        public virtual void SetDefaultNodeColors()
        {
            defaultNodeColors = new Dictionary<int, Color>();
            defaultNodeColors[(int)NodeTemplateType.VignetteNode] = DefaultNodeColor;
        }

        /// <summary>
        /// Reset variables that can be set to default or saved values
        /// </summary>
        public virtual void Reset()
        {
            saveDirectory = PlayerPrefs.GetString(SAVE_DIRECTORY_KEY, DefaultSaveDirectory);
            classNamespace = PlayerPrefs.GetString(NAMESPACE_KEY, DefaultNamespace);
            creatorName = EditorPrefs.GetString(CREATOR_NAME_KEY, string.Empty);
            nodeColor = defaultNodeColors[currentTemplateType];
        }
        #endregion -----------------------------------

        #region ------- OnGUI Draw Methods -------
        /// <summary>
        /// Header is intended for information that is not likely to change from node to node,
        /// such as the namespace and creator name
        /// </summary>
        public virtual void DrawHeader()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginDisabledGroup(SaveDirectoryIsDisabled);
            saveDirectory = EditorGUILayout.TextField("Save directory: ", saveDirectory);
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(NamespaceIsDisabled);
            classNamespace = EditorGUILayout.TextField("Namespace: ", classNamespace);
            EditorGUI.EndDisabledGroup();

            creatorName = EditorGUILayout.TextField("Creator name: ", creatorName);

            if (EditorGUI.EndChangeCheck())
            {
                PlayerPrefs.SetString(SAVE_DIRECTORY_KEY, saveDirectory);
                PlayerPrefs.SetString(NAMESPACE_KEY, classNamespace);
                EditorPrefs.SetString(CREATOR_NAME_KEY, creatorName);
            }
        }

        /// <summary>
        /// The body contains the node-specific fields. If/when extending the window GUI, this is
        /// almost certainly the place to extend/replace the default OnGUI calls
        /// </summary>
        public virtual void DrawBody()
        {
            nodeName = EditorGUILayout.TextField(new GUIContent("Node name: ", "Name of the new node"), nodeName);
            nodeColor = EditorGUILayout.ColorField(new GUIContent("Node color: ", "Color of the node in Vignettitor"), nodeColor);

            EditorGUI.BeginChangeCheck();
            int oldTemplateType = currentTemplateType;
            currentTemplateType = EditorGUILayout.Popup(new GUIContent("Template type: ", "Template to use for the new node"), currentTemplateType, StringsToGUIContents(nodeTemplateTypeNames));
            if (EditorGUI.EndChangeCheck())
                OnTemplateTypeChanged(oldTemplateType);
            if (ShowOutputRuleField)
                outputRuleType = (OutputRule.RuleType)EditorGUILayout.EnumPopup(
                    new GUIContent("Output rule: ",
                    "Passthrough: 1 or 0 children\nStatic: Children fixed at a specific number\nVariable: Any number of children"),
                    outputRuleType);
            if (ShowNumChildrenField)
                numChildren = EditorGUILayout.IntField(new GUIContent("Child count: ", "Number of children required by the node"), numChildren);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(new GUIContent("Class summary:", "Describe what the node should do to the implementing engineer"));
            summary = EditorGUILayout.TextArea(summary, GUILayout.ExpandWidth(true), GUILayout.MaxHeight(125));
            logMessage = EditorGUILayout.TextField(new GUIContent("Log message: ", "What is logged to the console when passing through the node"), logMessage);
        }

        /// <summary>
        /// Method to draw the buttons at the bottom of the window
        /// </summary>
        public virtual void DrawButtons()
        {
            EditorGUILayout.BeginHorizontal();
            // Cancel button
            if (GUILayout.Button("Cancel"))
                Close();

            // Create button (disabled if not valid)
            bool isValid = Validate();
            EditorGUI.BeginDisabledGroup(!isValid);

            GUIContent buttonLabel = new GUIContent("Create");

            if (!isValid)
                buttonLabel.tooltip = validationErrors;

            if (GUILayout.Button(buttonLabel))
                CreateNode();

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Called when the user changes the nodeType variable
        /// </summary>
        /// <param name="oldNodeType">Node type before the change</param>
        protected virtual void OnTemplateTypeChanged(int oldNodeType) { }

        /// <summary>
        /// Utility method to transform an array of string into an array of GUIContent
        /// </summary>
        /// <param name="strings">Array of strings to GUIfy</param>
        /// <returns>Array of GUIContents corresponding to the string at the same index of the input array</returns>
        protected GUIContent[] StringsToGUIContents(string[] strings)
        {
            int numStrings = strings.SafeLength();
            GUIContent[] contents = new GUIContent[numStrings];
            for (int i = 0; i < numStrings; i++)
            {
                contents[i] = new GUIContent(strings[i]);
            }
            return contents;
        }
        #endregion --------------------------------

        #region ------- Node Creation Methods -------
        /// <summary>
        /// Called when determining whether the fields contain enough valid data to create
        /// a new node
        /// </summary>
        /// <returns>True if the fields are valid, false if they are not</returns>
        protected virtual bool Validate()
        {
            bool valid = true;
            List<string> errors = new List<string>();

            if (SgString.IsNullOrWhiteSpace(classNamespace))
            {
                valid = false;
                errors.Add("No namespace specified");
            }

            if (SgString.IsNullOrWhiteSpace(creatorName))
            {
                valid = false;
                errors.Add("No creator name added");
            }

            if (SgString.IsNullOrWhiteSpace(nodeName))
            {
                valid = false;
                errors.Add("No node name specified");
            }

            if (SgString.IsNullOrWhiteSpace(summary))
            {
                valid = false;
                errors.Add("No class summary provided");
            }

            validationErrors = string.Join("\n", errors.ToArray());
            return valid;
        }

        /// <summary>
        /// Instatiate the proper template with the view's current data for a given template type
        /// </summary>
        /// <param name="templateType">Template Type to create</param>
        /// <param name="template">Instantiated template engine</param>
        /// <param name="newNodePath">Project path at which the new node should be created</param>
        protected virtual void SetUpTemplate(int templateType, out TemplateEngine template, out string newNodePath)
        {
            switch (templateType)
            {
                case (int)NodeTemplateType.VignetteNode:
                    VignetteNodeModel nodeModel = new VignetteNodeModel(classNamespace, creatorName, nodeName,
                            summary, GraphType, outputRuleType, numChildren, nodeColor, logMessage);
                    template = new TemplateEngine(nodeModel.GetTemplateText());
                    template.ApplyModel(nodeModel);
                    if (saveDirectory.Length > 0 && saveDirectory[saveDirectory.Length - 1] != '/')
                        saveDirectory += "/";
                    newNodePath = saveDirectory + nodeModel.GetFileName();
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Set up the selected template with the given node data, render the template and
        /// write the output to disk, then close the window.
        /// </summary>
        protected virtual void CreateNode()
        {
            TemplateEngine template = null;
            string newNodePath = string.Empty;
            SetUpTemplate(currentTemplateType, out template, out newNodePath);

            Directory.CreateDirectory(SgString.IsNullOrWhiteSpace(newNodePath) ? DefaultSaveDirectory : Path.GetDirectoryName(newNodePath));  // Creates if DNE

            File.WriteAllText(newNodePath, template.Render());
            AssetDatabase.ImportAsset(newNodePath);
            Log.Debug("Created new node at '{0}'", newNodePath);

            Close();
        }
        #endregion ----------------------------------
    }
}