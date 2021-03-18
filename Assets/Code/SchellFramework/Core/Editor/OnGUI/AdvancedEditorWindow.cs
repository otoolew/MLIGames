//-----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Eric Policaro
//  Date:   03/22/2016
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SG.Core.OnGUI
{
    /// <summary>
    /// This is an editor window with several usability improvements:
    /// - Framework to display modal popups in the window
    /// - Ability to defer actions until the end of an update loop instead of 
    ///   inside of OnGUI
    /// </summary>
    public class AdvancedEditorWindow : EditorWindow
    {
        /// <summary>
        /// Push a modal popup to the window to be displayed easch frame.
        /// </summary>
        /// <param name="drawAction">Function to draw the popup.</param>
        /// <param name="winTitle">Title of the popup window.</param>
        protected void OpenModalPopup(string winTitle, GUI.WindowFunction drawAction)
        {
            float x = position.width * .13f;
            float y = position.height * .13f;
            OpenModalPopup(winTitle, drawAction, new Rect(x, y, position.width - x * 2, position.height - y * 2));
        }

        /// <summary>
        /// Push a modal popup to the window to be displayed easch frame.
        /// </summary>
        /// <param name="drawAction">Function to draw the popup.</param>
        /// <param name="winTitle">Title of the popup window.</param>
        /// <param name="pos">Position to draw the popup.</param>
        /// <returns>
        /// The ID of the popup window. This should be saved so that it can be 
        /// used to close the popup later.
        /// </returns>
        protected void OpenModalPopup(string winTitle, GUI.WindowFunction drawAction, Rect pos)
        {
            var popup = new EditorWindowModalPopup(winTitle, drawAction, pos);
            ExecuteAtEndofFrame(() => _popups.Push(popup));
        }

        /// <summary>
        /// Close the top-most modal popup.
        /// </summary>
        /// <returns>
        /// <c>True</c> if there are more modal popups open;
        /// <c>False</c> otherwise.
        /// </returns>
        protected bool CloseModalPopup()
        {
            if (AnyPopups)
                ExecuteAtEndofFrame(() => _popups.Pop());

            return AnyPopups;
        }

        /// <summary>
        /// Update loop for the window. This will tick before
        /// any end of frame actions are fired.
        /// </summary>
        protected virtual void DoUpdate()
        {
        }

        /// <summary>
        /// Draws the editor window's GUI. These controls are
        /// disabled if any popups are visible.
        /// </summary>
        protected virtual void Draw()
        {
        }

        /// <summary>
        /// Executes the provided action at the end of the current frame.
        /// </summary>
        /// <param name="action">
        /// Action to fire. The provided action isonly executed one time.
        /// </param>
        protected void ExecuteAtEndofFrame(Action action)
        {
            _nextFrameActions.Add(action);
        }

        [UnityMessage]
        void Update()
        {
            DoUpdate();
            FireEndOfFrameActions();
        }

        [UnityMessage]
        void OnGUI()
        {
            try
            {
                GUI.enabled = !AnyPopups;

                Draw();

                GUI.enabled = true;
                if (AnyPopups)
                {
                    BeginWindows();
                    VisiblePopup.Draw();
                    EndWindows();
                }
            }
            catch (ExitGUIException)
            {
                // To support object fields.
            }
            catch (Exception e)
            {
                Close();
                Debug.LogException(e);
                throw;
            }
        }
        
        private void FireEndOfFrameActions()
        {
            _nextFrameActions.ForEach(a => a());
            _nextFrameActions.Clear();
        }

        private bool AnyPopups
        {
            get { return _popups.Count > 0; }
        }

        private EditorWindowModalPopup VisiblePopup
        {
            get { return _popups.Peek(); }
        }

        private readonly List<Action> _nextFrameActions = new List<Action>();
        private readonly Stack<EditorWindowModalPopup> _popups = new Stack<EditorWindowModalPopup>();

        /// <summary>
        /// Instructions for drawing a modal popup on top of this window.
        /// </summary>
        private class EditorWindowModalPopup
        {
            /// <summary> Internal ID of the window to draw.</summary>
            internal readonly int Id = UnityEngine.Random.Range(1, int.MaxValue - 1);

            /// <summary>Create a modal popup instruction.</summary>
            /// <param name="drawAction">
            /// Action that performs the draw operations.
            /// </param>
            /// <param name="title">Title text for the popup window.</param>
            /// <param name="position">
            /// Position to draw the popup window.
            /// </param>
            internal EditorWindowModalPopup(string title, GUI.WindowFunction drawAction, Rect position)
            {
                _drawAction = drawAction;
                _title = title;
                _position = position;
            }

            /// <summary>Draw the popup.</summary>
            internal void Draw()
            {
                _position = GUI.Window(Id, _position, _drawAction,
                    new GUIContent(_title));
            }

            private Rect _position;
            private readonly string _title;

            private readonly GUI.WindowFunction _drawAction;
        }
    }
}
