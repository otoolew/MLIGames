// ------------------------------------------------------------------------------
//  Copyright © 2015 Schell Games, LLC. All Rights Reserved.
//  Contact: Ryan Hipple
//  Created: 10/16/2015
// ------------------------------------------------------------------------------

using SG.Core.OnGUI;
using UnityEngine;

namespace SG.Vignettitor.Graph.Drawing
{
    /// <summary>
    /// Window that may be drawn in the vignettitor to display content and has 
    /// a close button.
    /// </summary>
    public class PopupWindow
    {
        #region -- Constants --------------------------------------------------
        /// <summary> Default width for a popup. </summary>
        public const int DEFAULT_WIDTH = 300;

        /// <summary> Default height for a popup. </summary>
        public const int DEFAULT_HEIGHT = 200;
        #endregion -- Constants -----------------------------------------------

        #region -- Protected Variables ----------------------------------------
        /// <summary> Scroll offset for the content. </summary>
        protected Vector2 scroll;

        /// <summary> Rect to draw the window in. </summary>
        protected Rect rect;

        /// <summary> ID to pass to the OnGUI window system. </summary>
        protected int windowID;

        /// <summary> Was close pressed on the last draw? </summary>
        protected bool closePressed;

        /// <summary> Text to display in the header of the popup. </summary>
        protected string headerText = "";
        
        /// <summary> Color for the window background. </summary>
        protected Color backgroundColor = new Color32(50, 50, 50, 255);

        /// <summary>
        /// Should this window be brought to the front right after drawing?
        /// </summary>
        protected virtual bool forceToFront { get; set; }
        #endregion -- Protected Variables -------------------------------------
        
        #region -- Public Methods ---------------------------------------------
        /// <summary>
        /// Create a new popup window that can display content and a close 
        /// button.
        /// </summary>
        /// <param name="headerText">Text for the header of the popup.</param>
        /// <param name="position">Where to draw the top corner.</param>
        public PopupWindow(string headerText, Vector2 position)
        {
            rect = new Rect(position.x, position.y, DEFAULT_WIDTH, DEFAULT_HEIGHT);
            this.headerText = headerText;
            windowID = GetHashCode();
        }

        /// <summary>
        /// Manually set the rectabgle to draw the window.
        /// </summary>
        /// <param name="r">Where to draw.</param>
        public void SetRect(Rect r)
        {
            rect = r;
        }

        /// <summary> Draw the popup window. </summary>
        /// <returns>Returns true if the close button was pressed.</returns>
        public virtual bool Draw()
        {
            if (Event.current.type == EventType.MouseDown)
                return true;
            GUI.Window(windowID, rect, Draw, "", GUI.skin.textArea);
            if (forceToFront)
                GUI.BringWindowToFront(windowID);
            return closePressed;
        }
        #endregion -- Public Methods ------------------------------------------

        #region -- Protected Methods ------------------------------------------
        /// <summary>
        /// Definition of the full drawing sequence for the window.
        /// </summary>
        /// <param name="id">ID for the OnGUI system.</param>
        protected virtual void Draw(int id)
        {
            OnGUIUtils.DrawBox(new Rect(0, 0, rect.width, rect.height), "", backgroundColor, Color.black);
            DrawHeader();
            GUILayout.Space(10);
            scroll = GUILayout.BeginScrollView(scroll);
            DrawContent();
            GUILayout.EndScrollView();
        }

        /// <summary>
        /// Draw the header, including the title and close button.
        /// </summary>
        protected virtual void DrawHeader()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(headerText, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(rect.x - 50));
            GUILayout.FlexibleSpace();
            Color oldColor = GUI.color;
            GUI.color = new Color(.8f, .3f, .3f, 1f);
            if (GUILayout.Button("x", GUILayout.ExpandWidth(false)))
                closePressed = true;
            GUI.color = oldColor;
            GUILayout.EndHorizontal();
        }

        /// <summary> Override to specify what content to draw. </summary>
        protected virtual void DrawContent()
        {
            //
        }
        #endregion -- Protected Methods ---------------------------------------
    }
}
