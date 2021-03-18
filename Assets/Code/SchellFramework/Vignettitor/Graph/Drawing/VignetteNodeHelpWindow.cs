// ------------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//  Contact: Ryan Hipple
//  Created: 10/16/2015
// ------------------------------------------------------------------------------

using System;
using SG.Vignettitor.VignetteData;
using UnityEngine;

namespace SG.Vignettitor.Graph.Drawing
{
    /// <summary>
    /// Draws an OnGUI window that displays help text for a vignette node type
    /// based on NodeHelpAttribute.
    /// </summary>
    public class VignetteNodeHelpWindow : PopupWindow
    {
        #region -- Constants --------------------------------------------------
        private const string NO_HELP_ATTRIB_ERROR = "No help information available. Add a NodeHelpAttribute to your VignetteNode class definition to add help text.";
        private const string TITLE_SUFFIX = " Node Help";
        #endregion -- Constants -----------------------------------------------

        #region -- Private Variables ------------------------------------------
        private readonly string helpText;
        #endregion -- Private Variables ---------------------------------------

        public VignetteNodeHelpWindow(string nodeDisplayName, Type nodeType, Vector2 position)
            : base(nodeDisplayName + TITLE_SUFFIX, position)
        {
            NodeHelpAttribute[] attribs = nodeType.GetCustomAttributes(typeof(NodeHelpAttribute), false) as NodeHelpAttribute[];
            if (attribs != null && attribs.Length > 0)
                helpText = attribs[0].HelpText;
            else
                helpText = NO_HELP_ATTRIB_ERROR;
        }

        protected override bool forceToFront { get { return true; }}

        protected override void DrawContent()
        {
            base.DrawContent();
            GUILayout.TextArea(helpText);
        }
    }
}
