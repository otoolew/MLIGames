// ------------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//  Contact: Ryan Hipple
//  Created: 10/17/2015
// ------------------------------------------------------------------------------

using SG.Core.OnGUI;
using SG.Vignettitor.Graph.Drawing;
using UnityEngine;

namespace SG.Vignettitor.Graph
{
    /// <summary>
    /// Popup to draw options for binding annotations to nodes.
    /// </summary>
    public class AnnotationBindPopup : PopupWindow
    {
        #region -- Constants --------------------------------------------------
        /// <summary> Width to draw the popup. </summary>
        public const float WIDTH = 160;

        /// <summary> Height to draw the popup. </summary>
        public const float HEIGHT = 110;
        #endregion -- Constants -----------------------------------------------

        #region -- Private Variables ------------------------------------------
        /// <summary>
        /// The annotation manager that the chouces will affect.
        /// </summary>
        private readonly AnnotationManager annotationManager;
        #endregion -- Private Variables ---------------------------------------

        /// <summary>
        /// Create a new popup to display options for annotation binding.
        /// </summary>
        /// <param name="annotationManager">
        /// The Annotation Manager that will be affected by the choices.
        /// </param>
        /// <param name="position">Position to draw the popup.</param>
        public AnnotationBindPopup(AnnotationManager annotationManager, Vector2 position)
            : base("", position)
        {
            this.annotationManager = annotationManager;
            rect.width = WIDTH;
            rect.height = HEIGHT;
        }

        protected override void Draw(int id)
        {
            OnGUIUtils.DrawBox(new Rect(0, 0, rect.width, rect.height), "", backgroundColor, Color.black);
            int boundCount = 0;
            if (annotationManager.AnnotationInEdit.BoundNodes != null)
                boundCount = annotationManager.AnnotationInEdit.BoundNodes.Count;

            int selectedCount = annotationManager.Editor.SelectionManager.AllSelected.Count;

            GUILayout.Label("Bound to " + boundCount + " nodes");

            GUI.enabled = selectedCount > 0;
            if (GUILayout.Button("Bind to Selected Nodes", OnGUIUtils.LeftTextButton))
            {
                annotationManager.BindOpenAnnotationToSelection();
                closePressed = true;
            }
            GUI.enabled = true;

            if (GUILayout.Button("Bind to Overlapped Nodes", OnGUIUtils.LeftTextButton))
            {
                annotationManager.BindOpenAnnotationToOverlap();
                closePressed = true;
            }

            GUI.enabled = boundCount > 0;
            if (GUILayout.Button("Clear Bound Nodes", OnGUIUtils.LeftTextButton))
            {
                annotationManager.ClearOpenAnnotationBind();
                closePressed = true;
            }
            if (GUILayout.Button("Select Bound Nodes", OnGUIUtils.LeftTextButton))
            {
                annotationManager.SelectOpenAnnotationBinds();
                closePressed = true;
            }
            GUI.enabled = true;
        }
    }
}
