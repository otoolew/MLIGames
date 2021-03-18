// ------------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//  Contact: Ryan Hipple
//  Created: 05/05/2016
// ------------------------------------------------------------------------------

using SG.Core.OnGUI;
using SG.Vignettitor.Graph.Drawing;
using UnityEngine;

namespace SG.Vignettitor.Graph
{
    /// <summary>
    /// Popup that allows for a quick selection and focus on a node by 
    /// entering an ID.
    /// </summary>
    public class GoToNodePopup : PopupWindow
    {
        #region -- Constants --------------------------------------------------
        /// <summary> Width to draw the popup. </summary>
        public const float WIDTH = 180;

        /// <summary> Height to draw the popup. </summary>
        public const float HEIGHT = 80;
        #endregion -- Constants -----------------------------------------------

        #region -- Private Variables ------------------------------------------
        /// <summary> Editor to apply the focus to. </summary>
        private GraphEditor editor;

        /// <summary>
        /// Int of the last parse of the input string. -1 means there was no 
        /// valid number.
        /// </summary>
        private int parsedInt = -1;

        /// <summary>
        /// Input string from the user that will be parsed to an int.
        /// </summary>
        private string inputString = "";

        /// <summary> Error message to display. </summary>
        private string message = "";
        #endregion -- Private Variables ---------------------------------------

        /// <summary>
        /// Create a new popup to display options for annotation binding.
        /// </summary>
        /// <param name="annotationManager">
        /// The Annotation Manager that will be affected by the choices.
        /// </param>
        /// <param name="position">Position to draw the popup.</param>
        public GoToNodePopup(Vector2 position, GraphEditor editor)
            : base("", position - new Vector2(WIDTH/2.0f, HEIGHT/2.0f))
        {
            this.editor = editor;
            rect.width = WIDTH;
            rect.height = HEIGHT;
        }
                
        protected override void Draw(int id)
        {
            GUI.FocusControl("ComboBoxText");
            OnGUIUtils.DrawBox(new Rect(0, 0, rect.width, rect.height), "", backgroundColor, Color.black);

            GUILayout.Label("Go To Node By ID");

            GUILayout.BeginHorizontal();
            GUI.SetNextControlName("ComboBoxText");

            string lastString = inputString;
            inputString = GUILayout.TextField(inputString);
            if (!int.TryParse(inputString, out parsedInt))
            {
                inputString = lastString;
                parsedInt = -1;
            }

            GUILayout.EndHorizontal();
            GUILayout.Label(message);

            GUILayout.BeginHorizontal();
            GUI.enabled = parsedInt >= 0;
            if (GUILayout.Button("Go") || (Event.current.isKey && Event.current.keyCode == KeyCode.Return))
            {
                int index = editor.GetIndexByID(parsedInt);
                if (index >= 0)
                {
                    editor.FocusOnNode(index);
                    editor.SelectionManager.Clear();
                    editor.SelectionManager.AddToSelection(index);
                    closePressed = true;
                }
                else
                {
                    message = "No node with ID " + parsedInt + " found.";
                }                
            }
            GUI.enabled = true;
            
            if (GUILayout.Button("Cancel") || (Event.current.isKey && Event.current.keyCode == KeyCode.Escape))
            {
                closePressed = true;
            }
            GUILayout.EndHorizontal();

            if (Event.current.type == EventType.MouseDown)
                Event.current.Use();
        }
    }
}
