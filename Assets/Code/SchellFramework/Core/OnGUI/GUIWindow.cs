//-----------------------------------------------------------------------------
//  Copyright © 2012 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   01/15/2013
//-----------------------------------------------------------------------------

using UnityEngine;
using System;

namespace SG.Core.OnGUI
{
    /// <summary>
    /// A GUIWindow is a user-positionable window drawn with OnGUI that can be 
    /// collapsed and full-screened. It maintains it's own state, unlike the 
    /// standard OnGUI window calls.
    /// </summary>
    public class GUIWindow : MonoBehaviour
    {
        #region -- Private Fields ---------------------------------------------
        /// <summary>
        /// The ID for the window, generated from the name string.
        /// </summary>
        protected int windowID;

        /// <summary>
        /// The name of the window, used to display in the header and to 
        /// generate an ID.
        /// </summary>
        public string windowName;

        /// <summary>
        /// The original rectangle for the window, specified at construction.
        /// </summary>
        public Rect initialRect;

        /// <summary>The rectangle that the window was draw at last.</summary>
        public Rect drawRect;
        
        /// <summary>
        /// The last user-defined rectangle used to draw the window, this is 
        /// not updated during fullscreen.
        /// </summary>
        protected Rect userRect;

        /// <summary>Is the window collapsed or expanded?</summary>
        public bool isCollapsed;

        /// <summary>Should the window draw fullscreen?</summary>
        protected bool isFullScreen;

        /// <summary>
        /// The padding inside the window used to define the area that the 
        /// "DrawContent" function can use to draw.
        /// </summary>
        protected RectOffset padding = new RectOffset(-4, -4, -38, -4);
        #endregion -- Private Fields ------------------------------------------

        #region -- Properties -------------------------------------------------
        /// <summary>
        /// A Vector2 made available to store a scroll offset for a window.
        /// Note that the GUIWindow does not handle scrolling internally.
        /// </summary>
        public Vector2 Scroll { get; set; }

        /// <summary>
        /// The function to call to draw the core of the window content.
        /// </summary>
        public Action<bool, Rect> DrawWindowContent { get; set; }

        /// <summary>
        /// The function to call to draw additional elements in the header.
        /// </summary>
        public Action<bool, Rect> DrawHeaderContent { get; set; }

        /// <summary>
        /// The action to perform when the cloase button is pressed.
        /// </summary>
        public Action OnClosePressed { get; set; }
        #endregion -- Properties ----------------------------------------------

        #region -- Initialization / Destruction -------------------------------        
        /// <summary>
        /// Create a new GUIWindow.
        /// </summary>
        /// <param name="name">The name to display on the window.</param>
        /// <param name="rect">The original rectangle to draw in.</param>
        /// <param name="drawWindowCallback">The draw window callback.</param>
        /// <param name="drawHeaderCallback">The draw header callback.</param>
        /// <param name="closeCallback">The close callback.</param>
        public virtual void Init(string name, Rect rect, Action<bool, Rect> drawWindowCallback, 
            Action<bool, Rect> drawHeaderCallback, Action closeCallback)
        {
            drawRect = rect;
            initialRect = drawRect;
            windowName = name;
            windowID = windowName.GetHashCode();
            DrawHeaderContent = drawHeaderCallback;
            DrawWindowContent = drawWindowCallback;
            OnClosePressed = closeCallback;
        }

        #endregion -- Initialization / Destruction ----------------------------

        #region -- Drawing Methods --------------------------------------------

        public void OnGUI()
        {
            Draw();
        }

        /// <summary>
        /// Draw the draggable window with collapse and fullscreen buttons.
        /// </summary>
        public virtual void Draw()
        {
            if (isCollapsed)
            {
                drawRect = new Rect(drawRect.x, drawRect.y, drawRect.width, OnGUIUtils.COLLAPSED_WINDOW_HEIGHT);
                userRect = drawRect;
            }
            else
            {
                if (isFullScreen)
                {
                    drawRect = new Rect(0.0f, 0.0f, Screen.width, Screen.height);
                }
                else
                {
                    drawRect = new Rect(drawRect.x, drawRect.y, drawRect.width, initialRect.height);
                    userRect = drawRect;
                }
            }

            drawRect = GUI.Window(windowID, drawRect, WindowDraw, windowName);
        }

        /// <summary>
        /// Draw the collapse and fullscreen buttons.
        /// </summary>
        private void DrawHeader()
        {
            GUI.enabled = !isFullScreen;
            if (GUI.Button(new Rect(drawRect.width - GUI.skin.window.border.left / 2f - OnGUIUtils.HEADER_BUTTON_SPACE * 3, GUI.skin.window.border.left / 2f, OnGUIUtils.HEADER_BUTTON_SPACE, OnGUIUtils.HEADER_BUTTON_SPACE), isCollapsed ? "+" : "-"))            
                isCollapsed = !isCollapsed;
            GUI.enabled = true;

            if (GUI.Button(new Rect(drawRect.width - GUI.skin.window.border.left / 2f - OnGUIUtils.HEADER_BUTTON_SPACE * 2, GUI.skin.window.border.left / 2f, OnGUIUtils.HEADER_BUTTON_SPACE, OnGUIUtils.HEADER_BUTTON_SPACE), isFullScreen ? "_" : "[]"))
            {
                isCollapsed = false;
                isFullScreen = !isFullScreen;
                if (!isFullScreen)
                    drawRect = userRect;
            }

            GUI.enabled = OnClosePressed != null;
            if (GUI.Button(new Rect(drawRect.width - GUI.skin.window.border.left / 2f - OnGUIUtils.HEADER_BUTTON_SPACE * 1, GUI.skin.window.border.left / 2f, OnGUIUtils.HEADER_BUTTON_SPACE, OnGUIUtils.HEADER_BUTTON_SPACE), "x"))
            {
                if (OnClosePressed != null)
                    OnClosePressed();
            }
            GUI.enabled = true;
        }

        /// <summary>
        /// Draw the windows header and call the draw content method.
        /// </summary>
        /// <param name="id">The window ID to draw.</param>
        private void WindowDraw(int id)
        {
            DrawHeader();
            Rect headerRect = new Rect(
                GUI.skin.window.border.left,
                GUI.skin.window.border.left / 2f,
                drawRect.width - GUI.skin.window.border.left * 2 - OnGUIUtils.HEADER_BUTTON_SPACE * 3,
                OnGUIUtils.HEADER_BUTTON_SPACE);

            if (DrawHeaderContent != null)
                DrawHeaderContent(!isCollapsed, headerRect);

            GUILayout.Space(OnGUIUtils.BUTTON_HEIGHT / 2.0f);

            if (DrawWindowContent != null)
            {
                DrawWindowContent(!isCollapsed, padding.Add(
                    new Rect(0, 0, drawRect.width, drawRect.height)));
            }
            GUI.DragWindow(headerRect);
        }
        #endregion -- Drawing Methods -----------------------------------------
    }
}
