// -----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Max Golden
//
//  Created: 12/1/2016 4:47 PM
// -----------------------------------------------------------------------------

namespace SG.Vignettitor.NodeMaker
{
    public static class NodeTemplateStrings
    {
        public const string CREATION_TIME = "#CREATION_TIME#";  // Month/Day/Year Time
        public const string CREATOR_NAME = "#CREATOR_NAME#";  // Name listed as the contact for the file
        public const string LOG_MESSAGE = "#LOG_MESSAGE#";  // Message logged when passing through the node in graph execution
        public const string NAMESPACE = "#NAMESPACE#";  // Namespace of the new classes
        public const string NODE_CLASS_NAME = "#NODE_CLASS_NAME#";  // Name of the node data class
        public const string VIEW_CLASS_NAME = "#VIEW_CLASS_NAME#";  // Name of the node view class
        public const string RUNTIME_CLASS_NAME = "#RUNTIME_CLASS_NAME#";  // Name of the node runtime class
        public const string NODE_GRAPH = "#NODE_GRAPH#";  // Type of VignetteGraph for the node
        public const string OUTPUT_RULE = "#OUTPUT_RULE#";  // e.g. OutputRule.Passthrough()
        public const string VIEW_COLOR = "#VIEW_COLOR#";  // e.g. new Color(1f, 0f, 1f)
        public const string NODE_MENU_PATH = "#NODE_MENU_PATH#";  // Spaced name without "Node"
        public const string SUMMARY = "#SUMMARY#";  // Class summary above the node class
        public const string YEAR = "#YEAR#";  // Year created (for the ©)
    }
}