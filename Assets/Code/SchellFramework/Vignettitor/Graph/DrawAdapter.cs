//-----------------------------------------------------------------------------
//  Copyright © 2014 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   09/04/2014
//-----------------------------------------------------------------------------

using System;
using SG.Core.OnGUI;
using UnityEngine;

namespace SG.Vignettitor.Graph
{
    /// <summary>
    /// Specifies Drawing functions that may differ across editor and runtime 
    /// code. Vignettitor is not in an editor namespace and can therefore not 
    /// directly call any editor GUI functionality. Extending this class in an 
    /// editor namespace allows for the vignettitor draw EditorGUI in the 
    /// editor if the draw adapter is passed in by the container window.
    /// </summary>
    public class DrawAdapter
    {
        /// <summary>
        /// The types of cursors that may be requested.
        /// </summary>
        public enum MouseCursor
        {
            Arrow,
            Text,
            ResizeVertical,
            ResizeHorizontal,
            Link,
            SlideArrow,
            ResizeUpRight,
            ResizeUpLeft,
            MoveArrow,
            RotateArrow,
            ScaleArrow,
            ArrowPlus,
            ArrowMinus,
            Pan,
            Orbit,
            Zoom,
            FPS,
            CustomCursor,
            SplitResizeUpDown,
            SplitResizeLeftRight
        }

        /// <summary>
        /// The type that should be used for node edit views when there is 
        /// none specified for the specific node type.
        /// </summary>
        public virtual Type DefaultEditView { get { return null; } }

        /// <summary> Draw a connection between two points. </summary>
        /// <param name="start">Start point for the connection.</param>
        /// <param name="end">End point for the connection.</param>
        /// <param name="startTangent">
        /// Tangent at the start point (may not be used).
        /// </param>
        /// <param name="endTangent">
        /// Tangent at the end point (may not be used).
        /// </param>
        /// <param name="color">Color of the line.</param>
        /// <param name="width">Width of the line (may not be used).</param>
        public virtual void DrawConnection(Vector3 start, Vector3 end, Vector3 startTangent, Vector3 endTangent, Color color, float width)
        {
            OnGUIUtils.DrawLine(start, end, color);
        }

        /// <summary> Draw a user-editable text area. </summary>
        /// <param name="text">Text to display.</param>
        /// <param name="style">Style to draw the text area.</param>
        /// <param name="options">GUI layout options</param>
        /// <returns>The user entered value.</returns>
        public virtual string TextAreaLayout(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            return GUILayout.TextArea(text, style, options);
        }

        /// <summary>
        /// Tells the mouse to use a specific cursor when over the given rect.
        /// </summary>
        /// <param name="position">Rect to change the cursor.</param>
        /// <param name="mouse">Cursor type.</param>
        public virtual void AddCursorRect(Rect position, MouseCursor mouse) {}
    }
}
